﻿using System;
using System.Threading.Tasks;

namespace TaxEtoro.Interfaces
{
    public interface IActionPerformer : IAsyncDisposable
    {
        public Task PerformCalculationsAndWriteResultsPeriodically(IServiceProvider serviceProvider);
    }

}