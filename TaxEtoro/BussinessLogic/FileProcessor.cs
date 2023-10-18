﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Calculations.Dto;
using Calculations.Interfaces;
using Database.DataAccess.Interfaces;
using Database.Entities.Database;
using Database.Enums;
using ExcelReader.Dto;
using ExcelReader.Interfaces;
using ExcelReader.Statics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ResultsPresenter.Interfaces;
using TaxCalculatingService.Interfaces;

namespace TaxCalculatingService.BussinessLogic;

internal sealed class FileProcessor : IFileProcessor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly IFileDataAccess _fileDataAccess;
    private readonly ILogger<FileProcessor> _logger;
    private readonly IExchangeRatesLocker _exchangeRatesLocker;
    private readonly Stopwatch _stopwatch;

    public FileProcessor(IServiceProvider serviceProvider,
        IConfiguration configuration,
        IFileDataAccess fileDataAccess,
        ILogger<FileProcessor> logger,
        IExchangeRatesLocker exchangeRatesLocker)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _fileDataAccess = fileDataAccess;
        _logger = logger;
        _exchangeRatesLocker = exchangeRatesLocker;
        _stopwatch = new Stopwatch();
    }

    private async Task ProcessSingleFile(Guid operation, CancellationToken token)
    {
        var fileEntity = await _fileDataAccess.GetInputFileDataAsync(operation);

        if (fileEntity.InputFileContent == null)
        {
            _logger.LogWarning("File {FileEntityInputFileName} was not found, marking as deleted.",
                fileEntity.InputFileName);
            await _fileDataAccess.SetAsDeletedAsync(operation);
            return;
        }

        await ProcessFile(fileEntity, operation)
            .ContinueWith(async _ => await RemoveFile(fileEntity), token);
    }

    public async Task ProcessFiles(CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return;
        }


        var numberOfOperations = await _fileDataAccess.GetOperationsToProcessNumberAsync();

        if (numberOfOperations == 0)
        {
            _logger.LogInformation("No pending operations detected waiting for new operation");
            return;
        }

        if (!_stopwatch.IsRunning)
        {
            _stopwatch.Restart();
        }

        do
        {
            IList<Guid> operations = await _fileDataAccess.GetOperationsToProcessAsync();
            await Parallel.ForEachAsync(operations, token,
                async (operation, cancellationToken) => { await ProcessSingleFile(operation, cancellationToken); });

            numberOfOperations = await _fileDataAccess.GetOperationsToProcessNumberAsync();
        } while (numberOfOperations > 0);

        _stopwatch.Stop();
        TimeSpan stopwatchResult = _stopwatch.Elapsed;

        _logger.LogInformation($"Calculation took {stopwatchResult:m\\:ss\\.fff}");

        _exchangeRatesLocker.ClearLockers();
    }

    private async Task RemoveFile(FileEntity file)
    {
        await _fileDataAccess.RemoveFileContentAsync(file.InputFileName);
        _logger.LogInformation($"File: {file.InputFileName} was deleted");
    }

    private Task ProcessFile(FileEntity fileEntity, Guid operation)
    {
        Task task = Task.Run(async () =>
        {
            try
            {
                var fileContent = new MemoryStream(fileEntity.InputFileContent);
                await _fileDataAccess.SetAsInProgressAsync(operation);
                await using AsyncServiceScope scope = _serviceProvider.CreateAsyncScope();
                var versionData = scope.ServiceProvider.GetService<ICurrentVersionData>();
                versionData.FileVersion = fileEntity.FileVersion;
                CalculationResultDto dto = await Calculate(fileEntity.InputFileName, fileContent, scope, operation);
                await PresentCalculationResults(dto, fileContent, scope, operation);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error during processing file {fileEntity.InputFileName} ");
            }
        });
        return task;
    }

    private async Task<CalculationResultDto> PerformCalculations(string fileName, MemoryStream fileContent,
        AsyncServiceScope scope)
    {
        var reader = scope.ServiceProvider.GetService<IExcelDataExtractor>();
        var taxCalculations = scope.ServiceProvider.GetService<ITaxCalculations>();

        _logger.LogInformation($"Started processing of the file {fileName}");
        await reader.ImportDataFromExcel(fileContent);
        CalculationResultDto result = await taxCalculations.CalculateTaxes();

        return result;
    }

    private async Task PresentCalculationResults(CalculationResultDto result, MemoryStream fileContent,
        AsyncServiceScope scope, Guid operationGuid)
    {
        var fileWriter = scope.ServiceProvider.GetService<IFileWriter>();

        var fileName = await fileWriter.PresentData(operationGuid, fileContent, result);

        _logger.LogInformation($"Created results in {fileName}");
    }

    private async Task<CalculationResultDto> Calculate(string fileName, MemoryStream fileContent,
        AsyncServiceScope scope, Guid operation)
    {
        CalculationResultDto dto = await PerformCalculations(fileName, fileContent, scope);
        var dtoString = JsonConvert.SerializeObject(dto);
        await _fileDataAccess.SetAsCalculatedAsync(operation, dtoString);
        return dto;
    }
}