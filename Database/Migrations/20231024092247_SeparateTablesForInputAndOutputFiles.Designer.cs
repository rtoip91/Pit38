﻿// <auto-generated />
using System;
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Database.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20231024092247_SeparateTablesForInputAndOutputFiles")]
    partial class SeparateTablesForInputAndOutputFiles
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Database.Entities.Database.ExchangeRateEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .HasColumnType("text");

                    b.Property<string>("Currency")
                        .HasColumnType("text");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<decimal>("Rate")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.HasIndex("Code", "Date");

                    b.ToTable("ExchangeRates");
                });

            modelBuilder.Entity("Database.Entities.Database.FileEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("CalculationResultFileContentId")
                        .HasColumnType("integer");

                    b.Property<string>("CalculationResultFileName")
                        .HasColumnType("text");

                    b.Property<string>("CalculationResultJson")
                        .HasColumnType("text");

                    b.Property<int>("FileVersion")
                        .HasColumnType("integer");

                    b.Property<int?>("InputFileContentId")
                        .HasColumnType("integer");

                    b.Property<string>("InputFileName")
                        .HasColumnType("text");

                    b.Property<Guid>("OperationGuid")
                        .HasColumnType("uuid");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<DateTime>("StatusChangeDate")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("CalculationResultFileContentId");

                    b.HasIndex("InputFileContentId");

                    b.HasIndex("OperationGuid")
                        .IsUnique();

                    b.ToTable("File");
                });

            modelBuilder.Entity("Database.Entities.Database.InputFileContentEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<byte[]>("FileContent")
                        .HasColumnType("bytea");

                    b.HasKey("Id");

                    b.ToTable("InputFileContent");
                });

            modelBuilder.Entity("Database.Entities.Database.ResultFileContentEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<byte[]>("FileContent")
                        .HasColumnType("bytea");

                    b.HasKey("Id");

                    b.ToTable("ResultFileContent");
                });

            modelBuilder.Entity("Database.Entities.Database.FileEntity", b =>
                {
                    b.HasOne("Database.Entities.Database.ResultFileContentEntity", "CalculationResultFileContent")
                        .WithMany()
                        .HasForeignKey("CalculationResultFileContentId");

                    b.HasOne("Database.Entities.Database.InputFileContentEntity", "InputFileContent")
                        .WithMany()
                        .HasForeignKey("InputFileContentId");

                    b.Navigation("CalculationResultFileContent");

                    b.Navigation("InputFileContent");
                });
#pragma warning restore 612, 618
        }
    }
}
