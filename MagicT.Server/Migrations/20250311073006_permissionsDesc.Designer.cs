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
    [Migration("20250311073006_permissionsDesc")]
    partial class permissionsDesc
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MagicT.Shared.Models.AUDIT_BASE", b =>
                {
                    b.Property<int>("AB_ROWID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AB_ROWID"));

                    b.Property<DateTime>("AB_DATE")
                        .HasColumnType("datetime2");

                    b.Property<string>("AB_END_POINT")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AB_METHOD")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AB_SERVICE")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AB_TYPE")
                        .HasColumnType("int");

                    b.Property<int>("AB_USER_ID")
                        .HasColumnType("int");

                    b.HasKey("AB_ROWID");

                    b.HasIndex("AB_DATE", "AB_TYPE", "AB_USER_ID", "AB_SERVICE", "AB_METHOD");

                    b.ToTable("AUDIT_BASE");

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("MagicT.Shared.Models.AUDIT_RECORDS_D", b =>
                {
                    b.Property<int>("ARD_ROWID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ARD_ROWID"));

                    b.Property<bool>("ARD_IS_PRIMARYKEY")
                        .HasColumnType("bit");

                    b.Property<int>("ARD_M_REFNO")
                        .HasColumnType("int");

                    b.Property<string>("ARD_NEW_VALUE")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ARD_OLD_VALUE")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ARD_PROPERTY_NAME")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ARD_ROWID");

                    b.HasIndex("ARD_M_REFNO");

                    b.ToTable("AUDIT_RECORDS_D");
                });

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

                    b.Property<string>("AB_NAME")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AB_ROWID");

                    b.ToTable("AUTHORIZATIONS_BASE");

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("MagicT.Shared.Models.TestModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CheckData")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DescriptionDetails")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsTrue")
                        .HasColumnType("bit");

                    b.Property<string>("MediaDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("TestModel");
                });

            modelBuilder.Entity("MagicT.Shared.Models.USERS", b =>
                {
                    b.Property<int>("U_ROWID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("U_ROWID"));

                    b.Property<string>("U_EMAIL")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("U_FULLNAME")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("U_IS_ACTIVE")
                        .HasColumnType("bit");

                    b.Property<bool>("U_IS_ADMIN")
                        .HasColumnType("bit");

                    b.Property<string>("U_LASTNAME")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("U_NAME")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("U_PASSWORD")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("U_PHONE_NUMBER")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.Property<string>("U_USERNAME")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("U_ROWID");

                    b.ToTable("USERS");
                });

            modelBuilder.Entity("MagicT.Shared.Models.USER_ROLES", b =>
                {
                    b.Property<int>("UR_ROWID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UR_ROWID"));

                    b.Property<int>("UR_ROLE_REFNO")
                        .HasColumnType("int");

                    b.Property<int>("UR_USER_REFNO")
                        .HasColumnType("int");

                    b.HasKey("UR_ROWID");

                    b.HasIndex("UR_ROLE_REFNO");

                    b.HasIndex("UR_USER_REFNO");

                    b.ToTable("USER_ROLES");
                });

            modelBuilder.Entity("MagicT.Shared.Models.AUDIT_FAILED", b =>
                {
                    b.HasBaseType("MagicT.Shared.Models.AUDIT_BASE");

                    b.Property<string>("AF_ERROR")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AF_PARAMETERS")
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("AUDIT_FAILED");
                });

            modelBuilder.Entity("MagicT.Shared.Models.AUDIT_QUERY", b =>
                {
                    b.HasBaseType("MagicT.Shared.Models.AUDIT_BASE");

                    b.Property<string>("AQ_PARAMETERS")
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("AUDIT_QUERY");
                });

            modelBuilder.Entity("MagicT.Shared.Models.AUDIT_RECORDS", b =>
                {
                    b.HasBaseType("MagicT.Shared.Models.AUDIT_BASE");

                    b.Property<string>("AR_PK_VALUE")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AR_TABLE_NAME")
                        .HasColumnType("nvarchar(450)");

                    b.HasIndex("AR_TABLE_NAME");

                    b.ToTable("AUDIT_RECORDS");
                });

            modelBuilder.Entity("MagicT.Shared.Models.PERMISSIONS", b =>
                {
                    b.HasBaseType("MagicT.Shared.Models.Base.AUTHORIZATIONS_BASE");

                    b.Property<string>("PER_PERMISSION_NAME")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PER_ROLE_REFNO")
                        .HasColumnType("int");

                    b.HasIndex("PER_ROLE_REFNO");

                    b.ToTable("PERMISSIONS");
                });

            modelBuilder.Entity("MagicT.Shared.Models.ROLES", b =>
                {
                    b.HasBaseType("MagicT.Shared.Models.Base.AUTHORIZATIONS_BASE");

                    b.ToTable("ROLES");
                });

            modelBuilder.Entity("MagicT.Shared.Models.AUDIT_RECORDS_D", b =>
                {
                    b.HasOne("MagicT.Shared.Models.AUDIT_RECORDS", null)
                        .WithMany("AUDIT_RECORDS_D")
                        .HasForeignKey("ARD_M_REFNO")
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

                    b.HasOne("MagicT.Shared.Models.USERS", "USERS")
                        .WithMany("USER_ROLES")
                        .HasForeignKey("UR_USER_REFNO")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AUTHORIZATIONS_BASE");

                    b.Navigation("USERS");
                });

            modelBuilder.Entity("MagicT.Shared.Models.AUDIT_FAILED", b =>
                {
                    b.HasOne("MagicT.Shared.Models.AUDIT_BASE", null)
                        .WithOne()
                        .HasForeignKey("MagicT.Shared.Models.AUDIT_FAILED", "AB_ROWID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MagicT.Shared.Models.AUDIT_QUERY", b =>
                {
                    b.HasOne("MagicT.Shared.Models.AUDIT_BASE", null)
                        .WithOne()
                        .HasForeignKey("MagicT.Shared.Models.AUDIT_QUERY", "AB_ROWID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MagicT.Shared.Models.AUDIT_RECORDS", b =>
                {
                    b.HasOne("MagicT.Shared.Models.AUDIT_BASE", null)
                        .WithOne()
                        .HasForeignKey("MagicT.Shared.Models.AUDIT_RECORDS", "AB_ROWID")
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

                    b.HasOne("MagicT.Shared.Models.ROLES", null)
                        .WithMany("PERMISSIONS")
                        .HasForeignKey("PER_ROLE_REFNO");
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
                    b.Navigation("USER_ROLES");
                });

            modelBuilder.Entity("MagicT.Shared.Models.AUDIT_RECORDS", b =>
                {
                    b.Navigation("AUDIT_RECORDS_D");
                });

            modelBuilder.Entity("MagicT.Shared.Models.ROLES", b =>
                {
                    b.Navigation("PERMISSIONS");
                });
#pragma warning restore 612, 618
        }
    }
}
