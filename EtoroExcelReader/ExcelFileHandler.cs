﻿using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Database.Entities.InMemory;
using ExcelReader.Dictionaries.V2021;
using ExcelReader.Dto;
using ExcelReader.Factory;
using ExcelReader.Interfaces;
using ExcelReader.Statics;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Export.ToDataTable;

namespace ExcelReader
{
    internal sealed class ExcelFileHandler : IExcelFileHandler
    {
        private readonly ILogger<ExcelFileHandler> _logger;
        private readonly IRowToEntityConverter _converter;

        public ExcelFileHandler(ILogger<ExcelFileHandler> logger, IConverterFactory converterFactory)
        {
            _logger = logger;
            _converter = converterFactory.GetConverter();
        }
        public async Task<ExtractedDataDto> ExtractDataFromExcel(string directory, string fileName)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var filePath = FileInputUtil.GetFileInfo(directory, fileName).FullName;
            FileInfo fileInfo = new FileInfo(filePath);

            if (fileInfo.Extension != ".xlsx")
            {
                throw new Exception("Wrong file type");
            }

            try
            {
                using ExcelPackage package = new ExcelPackage();
                await package.LoadAsync(fileInfo);

                ExtractedDataDto extractedDataDto = new ExtractedDataDto();

                DataTable closedPositionsDataTable =
                    await CreateDataTableAsync(package, ExcelSpreadsheetsV2021.ClosedPositions);
                DataTable transactionReportsDataTable =
                    await CreateDataTableAsync(package, ExcelSpreadsheetsV2021.TransactionReports);
                DataTable dividendsDataTable = await CreateDataTableAsync(package, ExcelSpreadsheetsV2021.Dividends);

                Task extractClosedPositions =
                    ExtractClosedPositionsAsync(closedPositionsDataTable, extractedDataDto, fileName);
                Task extractTransactionReports =
                    ExtractTransactionReportsAsync(transactionReportsDataTable, extractedDataDto, fileName);
                Task extractDividends = ExtractDividendsAsync(dividendsDataTable, extractedDataDto, fileName);

                await Task.WhenAll(extractClosedPositions, extractTransactionReports, extractDividends);
                return extractedDataDto;
            }
            catch (Exception)
            {
                throw new Exception("Wrong file content");
            }
        }

        private async Task<DataTable> CreateDataTableAsync(ExcelPackage package, int worksheetId)
        {
            return await Task.Run((() =>
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[worksheetId];

                string name = worksheet.Name.Trim();

                ExcelAddressBase dimension = worksheet.Dimension;
                int numberOfRows = dimension.Rows;
                int numberOfColumns = dimension.Columns;

                ToDataTableOptions options = ToDataTableOptions.Create();
                options.DataTableName = name;
                options.FirstRowIsColumnNames = true;


                DataTable dataTable = worksheet.Cells[1, 1, numberOfRows, numberOfColumns].ToDataTable(options);
                return dataTable;
            }));
        }

        private async Task ExtractClosedPositionsAsync(DataTable dataTable,
            ExtractedDataDto extractedData, string fileName)
        {
            await Task.Run(() =>
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    ClosedPositionEntity closedPosition = _converter.ToClosedPositionEntity(row);
                    extractedData.ClosedPositions.Add(closedPosition);
                }

                _logger.LogDebug("[{FileName}] added {RowsCount} closed positions", fileName, dataTable.Rows.Count);
                dataTable.Rows.Clear();
            });
        }

        private async Task ExtractDividendsAsync(DataTable dataTable,
            ExtractedDataDto extractedData, string fileName)
        {
            await Task.Run(() =>
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    DividendEntity dividend = _converter.ToDividendEntity(row);
                    extractedData.Dividends.Add(dividend);
                }

                _logger.LogDebug("[{FileName}] added {RowsCount} dividend positions", fileName, dataTable.Rows.Count);
                dataTable.Rows.Clear();
            });
        }

        private async Task ExtractTransactionReportsAsync(DataTable dataTable,
            ExtractedDataDto extractedData, string fileName)
        {
            await Task.Run(() =>
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    TransactionReportEntity transactionReport = _converter.ToTransactionReportEntity(row);
                    extractedData.TransactionReports.Add(transactionReport);
                }

                _logger.LogDebug("[{FileName}] added {RowsCount} transaction reports", fileName, dataTable.Rows.Count);
                dataTable.Rows.Clear();
            });
        }
    }
}
