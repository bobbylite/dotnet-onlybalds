﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using OnlyBalds.Api.Data;

#nullable disable

namespace OnlyBalds.Api.Migrations
{
    [DbContext(typeof(OnlyBaldsDataContext))]
    [Migration("20241025222935_QuestionnaireQuestionsUp")]
    partial class QuestionnaireQuestionsUp
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("OnlyBalds.Api.Models.BaldingOption", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasAnnotation("Relational:JsonPropertyName", "id");

                    b.Property<string>("BaldingOptionTitle")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasAnnotation("Relational:JsonPropertyName", "baldingOptionTitle");

                    b.Property<Guid?>("BaldingOptionsId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("BaldingOptionsId");

                    b.ToTable("BaldingOption");

                    b.HasAnnotation("Relational:JsonPropertyName", "option");
                });

            modelBuilder.Entity("OnlyBalds.Api.Models.BaldingOptions", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasAnnotation("Relational:JsonPropertyName", "id");

                    b.Property<Guid?>("QuestionnaireDataId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("QuestionnaireDataId");

                    b.ToTable("BaldingOptions");

                    b.HasAnnotation("Relational:JsonPropertyName", "baldingOptions");
                });

            modelBuilder.Entity("OnlyBalds.Api.Models.CommentItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("PostedOn")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("CommentItems");
                });

            modelBuilder.Entity("OnlyBalds.Api.Models.PostItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("PostedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("ThreadId")
                        .HasColumnType("uuid");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("PostItems");
                });

            modelBuilder.Entity("OnlyBalds.Api.Models.Question", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Answer")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasAnnotation("Relational:JsonPropertyName", "answer");

                    b.Property<Guid?>("BaldingOptionId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("QuestionnaireDataId")
                        .HasColumnType("uuid");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasAnnotation("Relational:JsonPropertyName", "title");

                    b.HasKey("Id");

                    b.HasIndex("BaldingOptionId");

                    b.HasIndex("QuestionnaireDataId");

                    b.ToTable("Question");

                    b.HasAnnotation("Relational:JsonPropertyName", "questions");
                });

            modelBuilder.Entity("OnlyBalds.Api.Models.QuestionnaireData", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasAnnotation("Relational:JsonPropertyName", "id");

                    b.HasKey("Id");

                    b.ToTable("QuestionnaireData");

                    b.HasAnnotation("Relational:JsonPropertyName", "data");
                });

            modelBuilder.Entity("OnlyBalds.Api.Models.QuestionnaireItems", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasAnnotation("Relational:JsonPropertyName", "id");

                    b.Property<Guid?>("DataId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("boolean")
                        .HasAnnotation("Relational:JsonPropertyName", "isCompleted");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp with time zone")
                        .HasAnnotation("Relational:JsonPropertyName", "startDate");

                    b.HasKey("Id");

                    b.HasIndex("DataId");

                    b.ToTable("QuestionnaireItems");
                });

            modelBuilder.Entity("OnlyBalds.Api.Models.ThreadItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Creator")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("PostsCount")
                        .HasColumnType("integer");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Summary")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("ThreadItems");
                });

            modelBuilder.Entity("OnlyBalds.Api.Models.BaldingOption", b =>
                {
                    b.HasOne("OnlyBalds.Api.Models.BaldingOptions", null)
                        .WithMany("Option")
                        .HasForeignKey("BaldingOptionsId");
                });

            modelBuilder.Entity("OnlyBalds.Api.Models.BaldingOptions", b =>
                {
                    b.HasOne("OnlyBalds.Api.Models.QuestionnaireData", null)
                        .WithMany("BaldingOptions")
                        .HasForeignKey("QuestionnaireDataId");
                });

            modelBuilder.Entity("OnlyBalds.Api.Models.Question", b =>
                {
                    b.HasOne("OnlyBalds.Api.Models.BaldingOption", null)
                        .WithMany("Questions")
                        .HasForeignKey("BaldingOptionId");

                    b.HasOne("OnlyBalds.Api.Models.QuestionnaireData", null)
                        .WithMany("Questions")
                        .HasForeignKey("QuestionnaireDataId");
                });

            modelBuilder.Entity("OnlyBalds.Api.Models.QuestionnaireItems", b =>
                {
                    b.HasOne("OnlyBalds.Api.Models.QuestionnaireData", "Data")
                        .WithMany()
                        .HasForeignKey("DataId");

                    b.Navigation("Data");
                });

            modelBuilder.Entity("OnlyBalds.Api.Models.BaldingOption", b =>
                {
                    b.Navigation("Questions");
                });

            modelBuilder.Entity("OnlyBalds.Api.Models.BaldingOptions", b =>
                {
                    b.Navigation("Option");
                });

            modelBuilder.Entity("OnlyBalds.Api.Models.QuestionnaireData", b =>
                {
                    b.Navigation("BaldingOptions");

                    b.Navigation("Questions");
                });
#pragma warning restore 612, 618
        }
    }
}