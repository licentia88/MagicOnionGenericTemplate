﻿// <auto-generated />
using System;
using MagicT.Server.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MagicT.Server.Migrations
{
    [DbContext(typeof(MagicTContext))]
    [Migration("20230829194138_roleName")]
    partial class roleName
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("MagicT.Shared.Models.Base.AUTHORIZATIONS_BASE", b =>
                {
                    b.Property<int>("AB_ROWID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AB_ROWID"), 1L, 1);

                    b.Property<string>("AB_AUTH_TYPE")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AB_DESCRIPTION")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AB_NAME")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AB_ROWID");

                    b.ToTable("AUTHORIZATIONS_BASE");
                });

            modelBuilder.Entity("MagicT.Shared.Models.Base.USERS_BASE", b =>
                {
                    b.Property<int>("UB_ROWID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UB_ROWID"), 1L, 1);

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
                });

            modelBuilder.Entity("MagicT.Shared.Models.FAILED_TRANSACTIONS_LOG", b =>
                {
                    b.Property<int>("FTL_ROWID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FTL_ROWID"), 1L, 1);

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

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("TestModel");
                });

            modelBuilder.Entity("MagicT.Shared.Models.USER_ROLES", b =>
                {
                    b.Property<int>("UR_ROWID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UR_ROWID"), 1L, 1);

                    b.Property<int?>("AB_ROWID")
                        .HasColumnType("int");

                    b.Property<int>("UR_AUTH_CODE")
                        .HasColumnType("int");

                    b.Property<int>("UR_USER_REFNO")
                        .HasColumnType("int");

                    b.HasKey("UR_ROWID");

                    b.HasIndex("AB_ROWID");

                    b.HasIndex("UR_USER_REFNO");

                    b.ToTable("USER_ROLES");
                });

            modelBuilder.Entity("MagicT.Shared.Models.PERMISSIONS", b =>
                {
                    b.HasBaseType("MagicT.Shared.Models.Base.AUTHORIZATIONS_BASE");

                    b.Property<string>("PER_METHOD_NAME")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PER_ROLE_NAME")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PER_ROLE_REFNO")
                        .HasColumnType("int");

                    b.HasIndex("PER_ROLE_REFNO");

                    b.ToTable("PERMISSIONS");
                });

            modelBuilder.Entity("MagicT.Shared.Models.ROLES", b =>
                {
                    b.HasBaseType("MagicT.Shared.Models.Base.AUTHORIZATIONS_BASE");

                    b.Property<string>("RL_SERVICE_NAME")
                        .HasColumnType("nvarchar(max)");

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

            modelBuilder.Entity("MagicT.Shared.Models.USER_ROLES", b =>
                {
                    b.HasOne("MagicT.Shared.Models.Base.AUTHORIZATIONS_BASE", "AUTHORIZATIONS_BASE")
                        .WithMany()
                        .HasForeignKey("AB_ROWID");

                    b.HasOne("MagicT.Shared.Models.Base.USERS_BASE", null)
                        .WithMany("USER_AUTHORIZATIONS")
                        .HasForeignKey("UR_USER_REFNO")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AUTHORIZATIONS_BASE");
                });

            modelBuilder.Entity("MagicT.Shared.Models.PERMISSIONS", b =>
                {
                    b.HasOne("MagicT.Shared.Models.Base.AUTHORIZATIONS_BASE", null)
                        .WithOne()
                        .HasForeignKey("MagicT.Shared.Models.PERMISSIONS", "AB_ROWID")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.HasOne("MagicT.Shared.Models.ROLES", null)
                        .WithMany("PERMISSIONS")
                        .HasForeignKey("PER_ROLE_REFNO")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MagicT.Shared.Models.ROLES", b =>
                {
                    b.HasOne("MagicT.Shared.Models.Base.AUTHORIZATIONS_BASE", null)
                        .WithOne()
                        .HasForeignKey("MagicT.Shared.Models.ROLES", "AB_ROWID")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MagicT.Shared.Models.USERS", b =>
                {
                    b.HasOne("MagicT.Shared.Models.Base.USERS_BASE", null)
                        .WithOne()
                        .HasForeignKey("MagicT.Shared.Models.USERS", "UB_ROWID")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MagicT.Shared.Models.Base.USERS_BASE", b =>
                {
                    b.Navigation("USER_AUTHORIZATIONS");
                });

            modelBuilder.Entity("MagicT.Shared.Models.ROLES", b =>
                {
                    b.Navigation("PERMISSIONS");
                });
#pragma warning restore 612, 618
        }
    }
}
