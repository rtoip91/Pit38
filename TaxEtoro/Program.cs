﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TaxEtoro.BussinessLogic;
using TaxEtoro.Interfaces;
using TaxEtoro.Statics;

namespace TaxEtoro
{
    class Program
    {
        private static IServiceProvider Services { get; set; }

        static Program()
        {
            Services = ServiceRegistration.Register();
        }

        static async Task Main(string[] args)
        {
            await using var scope = Services.CreateAsyncScope();
            var actionPerformer = new ActionPerformer();
            var timer = new Stopwatch();

            timer.Start();

            await actionPerformer.PerformCalculationsAndWriteResults();

            timer.Stop();

            TimeSpan timeTaken = timer.Elapsed;
            Console.WriteLine($"Time taken: {timeTaken:m\\:ss\\.fff}\n");
        }
    }
}