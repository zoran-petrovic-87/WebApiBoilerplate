﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using WebApi.Data;

namespace WebApi.Data.Migrations.App
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("WebApi.Models.Log", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("Application")
                        .HasColumnType("text");

                    b.Property<string>("CallSite")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Exception")
                        .HasColumnType("text");

                    b.Property<string>("Level")
                        .HasColumnType("text");

                    b.Property<string>("Logger")
                        .HasColumnType("text");

                    b.Property<string>("Message")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Logs");
                });

            modelBuilder.Entity("WebApi.Models.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("CreatedById")
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid?>("UpdatedById")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("UpdatedById");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("WebApi.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid?>("CreatedById")
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .HasMaxLength(320)
                        .HasColumnType("character varying(320)");

                    b.Property<string>("FirstName")
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("LastLoginAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("LastName")
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)");

                    b.Property<DateTime?>("LoginFailedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("LoginFailedCount")
                        .HasColumnType("integer");

                    b.Property<byte[]>("PasswordHash")
                        .HasColumnType("bytea");

                    b.Property<byte[]>("PasswordSalt")
                        .HasColumnType("bytea");

                    b.Property<string>("ResetPasswordCode")
                        .HasColumnType("text");

                    b.Property<int>("ResetPasswordCount")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("ResetPasswordCreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid?>("RoleId")
                        .HasColumnType("uuid");

                    b.Property<string>("UnconfirmedEmail")
                        .HasColumnType("text");

                    b.Property<string>("UnconfirmedEmailCode")
                        .HasColumnType("text");

                    b.Property<int>("UnconfirmedEmailCount")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("UnconfirmedEmailCreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid?>("UpdatedById")
                        .HasColumnType("uuid");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("IsActive");

                    b.HasIndex("RoleId");

                    b.HasIndex("UpdatedById");

                    b.HasIndex("Username");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("WebApi.Models.Role", b =>
                {
                    b.HasOne("WebApi.Models.User", "CreatedBy")
                        .WithMany("CreatedRoles")
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebApi.Models.User", "UpdatedBy")
                        .WithMany("UpdatedRoles")
                        .HasForeignKey("UpdatedById");

                    b.Navigation("CreatedBy");

                    b.Navigation("UpdatedBy");
                });

            modelBuilder.Entity("WebApi.Models.User", b =>
                {
                    b.HasOne("WebApi.Models.User", "CreatedBy")
                        .WithMany("CreatedUsers")
                        .HasForeignKey("CreatedById");

                    b.HasOne("WebApi.Models.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId");

                    b.HasOne("WebApi.Models.User", "UpdatedBy")
                        .WithMany("UpdatedUsers")
                        .HasForeignKey("UpdatedById");

                    b.Navigation("CreatedBy");

                    b.Navigation("Role");

                    b.Navigation("UpdatedBy");
                });

            modelBuilder.Entity("WebApi.Models.Role", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("WebApi.Models.User", b =>
                {
                    b.Navigation("CreatedRoles");

                    b.Navigation("CreatedUsers");

                    b.Navigation("UpdatedRoles");

                    b.Navigation("UpdatedUsers");
                });
#pragma warning restore 612, 618
        }
    }
}
