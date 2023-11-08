﻿using System;
using Database.Entities.Database;
using Microsoft.EntityFrameworkCore;

namespace Database
{
    internal sealed class ApplicationDbContext : DbContext
    {
        public DbSet<ExchangeRateEntity> ExchangeRates { get; set; }
        public DbSet<FileEntity> FileEntities { get; set; }
        private static bool _isMigrated;
        private static readonly object Locker = new();

        public ApplicationDbContext()
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string isDocker = Environment.GetEnvironmentVariable("IsDocker");

            if (isDocker == "true")
            {
                optionsBuilder.UseNpgsql(@"Host=postgres-db;Username=docker;Password=docker;Database=postgres");
            }
            else
            {
                optionsBuilder.UseNpgsql(@"Host=localhost;Username=docker;Password=docker;Database=postgres");
            }
            
        }

        internal void MigrateDatabase()
        {
            lock (Locker)
            {
                if (!_isMigrated)
                {
                    Database.Migrate();
                    _isMigrated = true;
                }
            }
        }
    }
}