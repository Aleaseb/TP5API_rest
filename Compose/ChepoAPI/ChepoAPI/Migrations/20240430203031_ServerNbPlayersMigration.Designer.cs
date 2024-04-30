﻿// <auto-generated />
using System;
using System.Collections.Generic;
using ChepoAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ChepoAPI.Migrations
{
    [DbContext(typeof(PostgreDbContext))]
    [Migration("20240430203031_ServerNbPlayersMigration")]
    partial class ServerNbPlayersMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ChepoAPI.PlayerStateData", b =>
                {
                    b.Property<Guid>("user_uuid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<List<Guid>>("friends")
                        .HasColumnType("uuid[]");

                    b.Property<bool>("is_in_game")
                        .HasColumnType("boolean");

                    b.Property<string>("map_name")
                        .HasColumnType("text");

                    b.Property<Guid?>("server_uuid")
                        .HasColumnType("uuid");

                    b.HasKey("user_uuid");

                    b.ToTable("player_state");
                });

            modelBuilder.Entity("ChepoAPI.PlayerStatsData", b =>
                {
                    b.Property<Guid>("user_uuid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("death")
                        .HasColumnType("integer");

                    b.Property<int>("kill")
                        .HasColumnType("integer");

                    b.Property<Guid>("rank_uuid")
                        .HasColumnType("uuid");

                    b.HasKey("user_uuid");

                    b.ToTable("player_stats");
                });

            modelBuilder.Entity("ChepoAPI.RankData", b =>
                {
                    b.Property<Guid>("uuid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("uuid");

                    b.ToTable("rank");
                });

            modelBuilder.Entity("ChepoAPI.ServerData", b =>
                {
                    b.Property<Guid>("uuid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("ip")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("nb_players")
                        .HasColumnType("integer");

                    b.HasKey("uuid");

                    b.ToTable("servers");
                });

            modelBuilder.Entity("ChepoAPI.SuccessData", b =>
                {
                    b.Property<Guid>("uuid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("image")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("uuid");

                    b.ToTable("success");
                });

            modelBuilder.Entity("ChepoAPI.UsersData", b =>
                {
                    b.Property<Guid>("uuid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("salt")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("uuid");

                    b.ToTable("users");
                });
#pragma warning restore 612, 618
        }
    }
}
