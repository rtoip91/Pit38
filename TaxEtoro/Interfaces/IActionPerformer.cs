﻿using Calculations.Dto;
using System;
using System.Threading.Tasks;

namespace TaxEtoro.Interfaces
{
    internal interface IActionPerformer : IAsyncDisposable
    {
        public Task PerformCalculationsAndWriteResults();
        void OnAppClose(object sender, EventArgs e);
    }
}