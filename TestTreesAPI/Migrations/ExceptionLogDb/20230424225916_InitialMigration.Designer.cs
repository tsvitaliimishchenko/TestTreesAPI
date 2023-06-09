﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TestTreesAPI.DataAccess;

#nullable disable

namespace TestTreesAPI.Migrations.ExceptionLogDb
{
    [DbContext(typeof(ExceptionLogDbContext))]
    [Migration("20230424225916_InitialMigration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("TestTreesAPI.Models.ExceptionLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("EventId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ExceptionLogs");
                });

            modelBuilder.Entity("TestTreesAPI.Models.ExceptionLog", b =>
                {
                    b.OwnsOne("TestTreesAPI.Models.ExceptionLogData", "Data", b1 =>
                        {
                            b1.Property<int>("ExceptionLogId")
                                .HasColumnType("int");

                            b1.Property<string>("BodyParameters")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Message")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("QueryParameters")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("StackTrace")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("ExceptionLogId");

                            b1.ToTable("ExceptionLogs");

                            b1.WithOwner()
                                .HasForeignKey("ExceptionLogId");
                        });

                    b.Navigation("Data")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
