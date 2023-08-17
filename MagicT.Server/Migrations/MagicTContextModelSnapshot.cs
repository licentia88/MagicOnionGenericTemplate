﻿// <auto-generated />
using System;
using MagicT.Server.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MagicT.Server.Migrations
{
    [DbContext(typeof(MagicTContext))]
    partial class MagicTContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MagicT.Shared.Models.Base.AUTHORIZATIONS_BASE", b =>
                {
                    b.Property<int>("AB_ROWID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AB_ROWID"));

                    b.Property<string>("AB_AUTH_TYPE")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AB_DESCRIPTION")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("AB_USER_REFNO")
                        .HasColumnType("int");

                    b.HasKey("AB_ROWID");

                    b.HasIndex("AB_USER_REFNO");

                    b.ToTable("AUTHORIZATIONS_BASE");

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("MagicT.Shared.Models.Base.USERS_BASE", b =>
                {
                    b.Property<int>("UB_ROWID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UB_ROWID"));

                    b.Property<string>("UB_FULLNAME")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("UB_IS_ACTIVE")
                        .HasColumnType("bit");

                    b.Property<string>("UB_PASSWORD")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UB_TYPE")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UB_ROWID");

                    b.ToTable("USERS_BASE");

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("MagicT.Shared.Models.FAILED_TRANSACTIONS_LOG", b =>
                {
                    b.Property<int>("FTL_ROWID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FTL_ROWID"));

                    b.Property<DateTime>("FTL_DATE")
                        .HasColumnType("datetime2");

                    b.Property<string>("FTL_ERROR")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("FTL_ROWID");

                    b.ToTable("FAILED_TRANSACTIONS_LOG");
                });

            modelBuilder.Entity("MagicT.Shared.Models.TestModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("TestModel");
                });

            modelBuilder.Entity("MagicT.Shared.Models.PERMISSIONS", b =>
                {
                    b.HasBaseType("MagicT.Shared.Models.Base.AUTHORIZATIONS_BASE");

                    b.ToTable("PERMISSIONS");
                });

            modelBuilder.Entity("MagicT.Shared.Models.ROLES", b =>
                {
                    b.HasBaseType("MagicT.Shared.Models.Base.AUTHORIZATIONS_BASE");

                    b.ToTable("ROLES");
                });

            modelBuilder.Entity("MagicT.Shared.Models.USERS", b =>
                {
                    b.HasBaseType("MagicT.Shared.Models.Base.USERS_BASE");

                    b.Property<string>("U_EMAIL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("U_NAME")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("U_PHONE_NUMBER")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("U_SURNAME")
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("USERS");
                });

            modelBuilder.Entity("MagicT.Shared.Models.Base.AUTHORIZATIONS_BASE", b =>
                {
                    b.HasOne("MagicT.Shared.Models.USERS", null)
                        .WithMany("AUTHORIZATIONS_BASE")
                        .HasForeignKey("AB_USER_REFNO")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MagicT.Shared.Models.PERMISSIONS", b =>
                {
                    b.HasOne("MagicT.Shared.Models.Base.AUTHORIZATIONS_BASE", null)
                        .WithOne()
                        .HasForeignKey("MagicT.Shared.Models.PERMISSIONS", "AB_ROWID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MagicT.Shared.Models.ROLES", b =>
                {
                    b.HasOne("MagicT.Shared.Models.Base.AUTHORIZATIONS_BASE", null)
                        .WithOne()
                        .HasForeignKey("MagicT.Shared.Models.ROLES", "AB_ROWID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MagicT.Shared.Models.USERS", b =>
                {
                    b.HasOne("MagicT.Shared.Models.Base.USERS_BASE", null)
                        .WithOne()
                        .HasForeignKey("MagicT.Shared.Models.USERS", "UB_ROWID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MagicT.Shared.Models.USERS", b =>
                {
                    b.Navigation("AUTHORIZATIONS_BASE");
                });
#pragma warning restore 612, 618
        }
    }
}
