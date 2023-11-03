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
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("MagicT.Shared.Models.AUDIT_D", b =>
                {
                    b.Property<int>("AD_ROWID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AD_ROWID"), 1L, 1);

                    b.Property<int>("AD_CURRENT_USER")
                        .HasColumnType("int");

                    b.Property<DateTime>("AD_DATE")
                        .HasColumnType("datetime2");

                    b.Property<int>("AD_M_REFNO")
                        .HasColumnType("int");

                    b.Property<string>("AD_NEW_VALUE")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AD_OLD_VALUE")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AD_PARENT_NAME")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AD_PRIMARY_KEY")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AD_PROPERTY_NAME")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AD_TYPE")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AD_ROWID");

                    b.HasIndex("AD_M_REFNO");

                    b.ToTable("AUDIT_D");
                });

            modelBuilder.Entity("MagicT.Shared.Models.AUDIT_M", b =>
                {
                    b.Property<int>("AM_ROWID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AM_ROWID"), 1L, 1);

                    b.Property<string>("AM_TABLE_NAME")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AM_ROWID");

                    b.ToTable("AUDIT_M");
                });

            modelBuilder.Entity("MagicT.Shared.Models.AUDIT_TYPES", b =>
                {
                    b.Property<string>("AT_CODE")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AT_DESC")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AT_CODE");

                    b.ToTable("AUDIT_TYPES");
                });

            modelBuilder.Entity("MagicT.Shared.Models.Base.AUTHORIZATIONS_BASE", b =>
                {
                    b.Property<int>("AB_ROWID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AB_ROWID"), 1L, 1);

                    b.Property<string>("AB_AUTH_TYPE")
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

                    b.Property<int>("UR_ROLE_REFNO")
                        .HasColumnType("int");

                    b.Property<int>("UR_USER_REFNO")
                        .HasColumnType("int");

                    b.HasKey("UR_ROWID");

                    b.HasIndex("UR_ROLE_REFNO");

                    b.HasIndex("UR_USER_REFNO");

                    b.ToTable("USER_ROLES");
                });

            modelBuilder.Entity("MagicT.Shared.Models.PERMISSIONS", b =>
                {
                    b.HasBaseType("MagicT.Shared.Models.Base.AUTHORIZATIONS_BASE");

                    b.Property<string>("PER_PERMISSION_NAME")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PER_ROLE_REFNO")
                        .HasColumnType("int");

                    b.HasIndex("PER_ROLE_REFNO");

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
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("U_NAME")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("U_PHONE_NUMBER")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.Property<string>("U_SURNAME")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.ToTable("USERS");
                });

            modelBuilder.Entity("MagicT.Shared.Models.SUPER_USER", b =>
                {
                    b.HasBaseType("MagicT.Shared.Models.USERS");

                    b.ToTable("SUPER_USER");
                });

            modelBuilder.Entity("MagicT.Shared.Models.AUDIT_D", b =>
                {
                    b.HasOne("MagicT.Shared.Models.AUDIT_M", null)
                        .WithMany("AUDIT_D")
                        .HasForeignKey("AD_M_REFNO")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MagicT.Shared.Models.USER_ROLES", b =>
                {
                    b.HasOne("MagicT.Shared.Models.Base.AUTHORIZATIONS_BASE", "AUTHORIZATIONS_BASE")
                        .WithMany()
                        .HasForeignKey("UR_ROLE_REFNO")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MagicT.Shared.Models.Base.USERS_BASE", null)
                        .WithMany("USER_ROLES")
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

            modelBuilder.Entity("MagicT.Shared.Models.SUPER_USER", b =>
                {
                    b.HasOne("MagicT.Shared.Models.USERS", null)
                        .WithOne()
                        .HasForeignKey("MagicT.Shared.Models.SUPER_USER", "UB_ROWID")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MagicT.Shared.Models.AUDIT_M", b =>
                {
                    b.Navigation("AUDIT_D");
                });

            modelBuilder.Entity("MagicT.Shared.Models.Base.USERS_BASE", b =>
                {
                    b.Navigation("USER_ROLES");
                });

            modelBuilder.Entity("MagicT.Shared.Models.ROLES", b =>
                {
                    b.Navigation("PERMISSIONS");
                });
#pragma warning restore 612, 618
        }
    }
}
