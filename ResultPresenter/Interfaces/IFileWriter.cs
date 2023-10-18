﻿using Calculations.Dto;

namespace ResultsPresenter.Interfaces
{
    public interface IFileWriter
    {
        Task<string> PresentData(Guid operationId, MemoryStream inputFileContent,
            CalculationResultDto calculationResultDto);
    }
}