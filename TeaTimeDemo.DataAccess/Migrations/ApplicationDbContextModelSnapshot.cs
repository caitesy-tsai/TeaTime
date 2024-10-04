﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TeaTimeDemo.DataAccess.Data;

#nullable disable

namespace TeaTimeDemo.DataAccess.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(21)
                        .HasColumnType("nvarchar(21)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);

                    b.HasDiscriminator().HasValue("IdentityUser");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("TeaTimeDemo.Models.Answer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AnswerText")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("AnswerType")
                        .HasColumnType("int");

                    b.Property<string>("ApplicationUserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("CompleteTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("CreateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2");

                    b.Property<string>("MtNum")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("QuestionId")
                        .HasColumnType("int");

                    b.Property<string>("Remark")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.HasIndex("QuestionId");

                    b.ToTable("Answers");
                });

            modelBuilder.Entity("TeaTimeDemo.Models.AnswerOption", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AnswerId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<bool>("IsCorrect")
                        .HasColumnType("bit");

                    b.Property<bool>("IsOther")
                        .HasColumnType("bit");

                    b.Property<int>("QuestionOptionId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AnswerId");

                    b.HasIndex("QuestionOptionId");

                    b.ToTable("AnswerOptions");
                });

            modelBuilder.Entity("TeaTimeDemo.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("Id");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            DisplayOrder = 1,
                            Name = "茶飲"
                        },
                        new
                        {
                            Id = 2,
                            DisplayOrder = 2,
                            Name = "水果茶"
                        },
                        new
                        {
                            Id = 3,
                            DisplayOrder = 3,
                            Name = "咖啡"
                        });
                });

            modelBuilder.Entity("TeaTimeDemo.Models.OrderDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<string>("Ice")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OrderHeaderId")
                        .HasColumnType("int");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<string>("sweetness")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("OrderHeaderId");

                    b.HasIndex("ProductId");

                    b.ToTable("OrderDetails");
                });

            modelBuilder.Entity("TeaTimeDemo.Models.OrderHeader", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ApplicationUserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("OrderStatus")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("OrderTotal")
                        .HasColumnType("float");

                    b.Property<DateTime>("PaymentDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("PaymentDueDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("PaymentStatus")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SessionId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.ToTable("OrderHeaders");
                });

            modelBuilder.Entity("TeaTimeDemo.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<string>("Size")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CategoryId = 1,
                            Description = "天然果飲，迷人多變",
                            ImageUrl = "",
                            Name = "台灣水果茶",
                            Price = 60.0,
                            Size = "大杯"
                        },
                        new
                        {
                            Id = 2,
                            CategoryId = 2,
                            Description = "品鐵觀音，享人生的味道",
                            ImageUrl = "",
                            Name = "鐵觀音",
                            Price = 35.0,
                            Size = "中杯"
                        },
                        new
                        {
                            Id = 3,
                            CategoryId = 3,
                            Description = "用咖啡體悟悠閒時光",
                            ImageUrl = "",
                            Name = "美式咖啡",
                            Price = 50.0,
                            Size = "中杯"
                        });
                });

            modelBuilder.Entity("TeaTimeDemo.Models.Question", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AnswerType")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CompleteTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("CreateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2");

                    b.Property<string>("MceHtml")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("QuestionText")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Remark")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SurveyId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SurveyId");

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("TeaTimeDemo.Models.QuestionImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AltText")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int?>("QuestionId")
                        .HasColumnType("int");

                    b.Property<int?>("QuestionOptionId")
                        .HasColumnType("int");

                    b.Property<int?>("SortOrder")
                        .HasColumnType("int");

                    b.Property<int?>("SurveyId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UploadTime")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("QuestionId");

                    b.HasIndex("QuestionOptionId");

                    b.HasIndex("SurveyId");

                    b.ToTable("QuestionImages");
                });

            modelBuilder.Entity("TeaTimeDemo.Models.QuestionOption", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<bool>("IsCorrect")
                        .HasColumnType("bit");

                    b.Property<bool>("IsOther")
                        .HasColumnType("bit");

                    b.Property<string>("OptionText")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("QuestionId")
                        .HasColumnType("int");

                    b.Property<int?>("SortOrder")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("QuestionId");

                    b.ToTable("QuestionOptions");
                });

            modelBuilder.Entity("TeaTimeDemo.Models.ShoppingCart", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ApplicationUserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<string>("Ice")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<string>("Sweetness")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.HasIndex("ProductId");

                    b.ToTable("ShoppingCarts");
                });

            modelBuilder.Entity("TeaTimeDemo.Models.Station", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Remark")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Stations");
                });

            modelBuilder.Entity("TeaTimeDemo.Models.Store", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Stores");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Address = "江蘇省南通高新區希望大道99號",
                            City = "南通",
                            Description = "塑造展華優質企業文化，以人為本，以誠信負責為基石",
                            Name = "上海展華電子有限公司",
                            PhoneNumber = "0513-86866000"
                        },
                        new
                        {
                            Id = 2,
                            Address = "宜蘭縣蘇澳鎮頂寮里頂平路36號",
                            City = "蘇澳鎮",
                            Description = "經營團隊秉持「積極創新、團結和諧、客戶導向、謹守誠信」的經營理念",
                            Name = "燿華宜蘭廠區",
                            PhoneNumber = "(03)970 5818"
                        },
                        new
                        {
                            Id = 3,
                            Address = "泰國",
                            City = "未知",
                            Description = "積極創新、團結和諧、客戶導向、謹守誠信",
                            Name = "燿華泰國廠區",
                            PhoneNumber = "未知"
                        },
                        new
                        {
                            Id = 4,
                            Address = "新北市土城區中山路4巷3號",
                            City = "土城",
                            Description = "積極創新、團結和諧、客戶導向、謹守誠信",
                            Name = "燿華土城廠區",
                            PhoneNumber = "(02)2268 5071"
                        });
                });

            modelBuilder.Entity("TeaTimeDemo.Models.Survey", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ApplicationUserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CompleteTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("CreateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsPublished")
                        .HasColumnType("bit");

                    b.Property<string>("JobName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("JobNum")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("MceHtml")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("QuestionNum")
                        .HasColumnType("int");

                    b.Property<string>("Remark")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StationName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.ToTable("Surveys");
                });

            modelBuilder.Entity("TeaTimeDemo.Models.ApplicationUser", b =>
                {
                    b.HasBaseType("Microsoft.AspNetCore.Identity.IdentityUser");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("StoreId")
                        .HasColumnType("int");

                    b.HasIndex("StoreId");

                    b.HasDiscriminator().HasValue("ApplicationUser");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TeaTimeDemo.Models.Answer", b =>
                {
                    b.HasOne("TeaTimeDemo.Models.ApplicationUser", "ApplicationUser")
                        .WithMany()
                        .HasForeignKey("ApplicationUserId");

                    b.HasOne("TeaTimeDemo.Models.Question", "Question")
                        .WithMany()
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ApplicationUser");

                    b.Navigation("Question");
                });

            modelBuilder.Entity("TeaTimeDemo.Models.AnswerOption", b =>
                {
                    b.HasOne("TeaTimeDemo.Models.Answer", "Answer")
                        .WithMany("SelectedOptions")
                        .HasForeignKey("AnswerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TeaTimeDemo.Models.QuestionOption", "QuestionOption")
                        .WithMany("AnswerOptions")
                        .HasForeignKey("QuestionOptionId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Answer");

                    b.Navigation("QuestionOption");
                });

            modelBuilder.Entity("TeaTimeDemo.Models.OrderDetail", b =>
                {
                    b.HasOne("TeaTimeDemo.Models.OrderHeader", "OrderHeader")
                        .WithMany()
                        .HasForeignKey("OrderHeaderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TeaTimeDemo.Models.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("OrderHeader");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("TeaTimeDemo.Models.OrderHeader", b =>
                {
                    b.HasOne("TeaTimeDemo.Models.ApplicationUser", "ApplicationUser")
                        .WithMany()
                        .HasForeignKey("ApplicationUserId");

                    b.Navigation("ApplicationUser");
                });

            modelBuilder.Entity("TeaTimeDemo.Models.Product", b =>
                {
                    b.HasOne("TeaTimeDemo.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("TeaTimeDemo.Models.Question", b =>
                {
                    b.HasOne("TeaTimeDemo.Models.Survey", "Survey")
                        .WithMany("Questions")
                        .HasForeignKey("SurveyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Survey");
                });

            modelBuilder.Entity("TeaTimeDemo.Models.QuestionImage", b =>
                {
                    b.HasOne("TeaTimeDemo.Models.Question", "Question")
                        .WithMany("QuestionImages")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("TeaTimeDemo.Models.QuestionOption", "QuestionOption")
                        .WithMany("QuestionImages")
                        .HasForeignKey("QuestionOptionId");

                    b.HasOne("TeaTimeDemo.Models.Survey", "Survey")
                        .WithMany("QuestionImages")
                        .HasForeignKey("SurveyId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("Question");

                    b.Navigation("QuestionOption");

                    b.Navigation("Survey");
                });

            modelBuilder.Entity("TeaTimeDemo.Models.QuestionOption", b =>
                {
                    b.HasOne("TeaTimeDemo.Models.Question", "Question")
                        .WithMany("QuestionOptions")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Question");
                });

            modelBuilder.Entity("TeaTimeDemo.Models.ShoppingCart", b =>
                {
                    b.HasOne("TeaTimeDemo.Models.ApplicationUser", "ApplicationUser")
                        .WithMany()
                        .HasForeignKey("ApplicationUserId");

                    b.HasOne("TeaTimeDemo.Models.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ApplicationUser");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("TeaTimeDemo.Models.Survey", b =>
                {
                    b.HasOne("TeaTimeDemo.Models.ApplicationUser", "ApplicationUser")
                        .WithMany()
                        .HasForeignKey("ApplicationUserId");

                    b.Navigation("ApplicationUser");
                });

            modelBuilder.Entity("TeaTimeDemo.Models.ApplicationUser", b =>
                {
                    b.HasOne("TeaTimeDemo.Models.Store", "Store")
                        .WithMany()
                        .HasForeignKey("StoreId");

                    b.Navigation("Store");
                });

            modelBuilder.Entity("TeaTimeDemo.Models.Answer", b =>
                {
                    b.Navigation("SelectedOptions");
                });

            modelBuilder.Entity("TeaTimeDemo.Models.Question", b =>
                {
                    b.Navigation("QuestionImages");

                    b.Navigation("QuestionOptions");
                });

            modelBuilder.Entity("TeaTimeDemo.Models.QuestionOption", b =>
                {
                    b.Navigation("AnswerOptions");

                    b.Navigation("QuestionImages");
                });

            modelBuilder.Entity("TeaTimeDemo.Models.Survey", b =>
                {
                    b.Navigation("QuestionImages");

                    b.Navigation("Questions");
                });
#pragma warning restore 612, 618
        }
    }
}
