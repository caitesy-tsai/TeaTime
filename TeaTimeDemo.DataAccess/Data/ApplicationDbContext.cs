using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TeaTimeDemo.Models;

namespace TeaTimeDemo.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Survey> Surveys { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<AnswerOption> AnswerOptions { get; set; }
        public DbSet<QuestionImage> QuestionImages { get; set; }
        public DbSet<QuestionOption> QuestionOptions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 預先載入的數據
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "茶飲", DisplayOrder = 1 },
                new Category { Id = 2, Name = "水果茶", DisplayOrder = 2 },
                new Category { Id = 3, Name = "咖啡", DisplayOrder = 3 }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "台灣水果茶",
                    Size = "大杯",
                    Description = "天然果飲，迷人多變",
                    Price = 60,
                    CategoryId = 1,
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 2,
                    Name = "鐵觀音",
                    Size = "中杯",
                    Description = "品鐵觀音，享人生的味道",
                    Price = 35,
                    CategoryId = 2,
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 3,
                    Name = "美式咖啡",
                    Size = "中杯",
                    Description = "用咖啡體悟悠閒時光",
                    Price = 50,
                    CategoryId = 3,
                    ImageUrl = ""
                }
            );

            modelBuilder.Entity<Store>().HasData(
                new Store
                {
                    Id = 1,
                    Name = "上海展華電子有限公司",
                    Address = "江蘇省南通高新區希望大道99號",
                    City = "南通",
                    PhoneNumber = "0513-86866000",
                    Description = "塑造展華優質企業文化，以人為本，以誠信負責為基石"
                },
                new Store
                {
                    Id = 2,
                    Name = "燿華宜蘭廠區",
                    Address = "宜蘭縣蘇澳鎮頂寮里頂平路36號",
                    City = "蘇澳鎮",
                    PhoneNumber = "(03)970 5818",
                    Description = "經營團隊秉持「積極創新、團結和諧、客戶導向、謹守誠信」的經營理念"
                },
                new Store
                {
                    Id = 3,
                    Name = "燿華泰國廠區",
                    Address = "泰國",
                    City = "未知",
                    PhoneNumber = "未知",
                    Description = "積極創新、團結和諧、客戶導向、謹守誠信"
                },
                new Store
                {
                    Id = 4,
                    Name = "燿華土城廠區",
                    Address = "新北市土城區中山路4巷3號",
                    City = "土城",
                    PhoneNumber = "(02)2268 5071",
                    Description = "積極創新、團結和諧、客戶導向、謹守誠信"
                }
            );

            // 設定 Answer 與 AnswerOption 的關聯
            modelBuilder.Entity<Answer>()
                .HasMany(a => a.SelectedOptions)
                .WithOne(ao => ao.Answer)
                .HasForeignKey(ao => ao.AnswerId)
                .OnDelete(DeleteBehavior.Cascade);  // 當刪除 Answer 時，刪除關聯的 AnswerOption

            // 設定 AnswerOption 與 QuestionOption 的關聯
            modelBuilder.Entity<AnswerOption>()
                .HasOne(ao => ao.QuestionOption)
                .WithMany(qo => qo.AnswerOptions)
                .HasForeignKey(ao => ao.QuestionOptionId)
                .OnDelete(DeleteBehavior.NoAction);  // 避免多重級聯刪除

            // 防止其他表的多重級聯刪除問題
            modelBuilder.Entity<Question>()
                .HasOne(q => q.Survey)
                .WithMany(s => s.Questions)
                .HasForeignKey(q => q.SurveyId)
                .OnDelete(DeleteBehavior.Cascade);  // 問卷刪除時，問題一起刪除

            modelBuilder.Entity<QuestionImage>()
                .HasOne(qi => qi.Survey)  // Survey 可以有圖片，但不設置級聯刪除
                .WithMany(s => s.QuestionImages)
                .HasForeignKey(qi => qi.SurveyId)
                .OnDelete(DeleteBehavior.NoAction);  // 取消對 SurveyId 的級聯刪除，改為手動刪除

            modelBuilder.Entity<QuestionImage>()
                .HasOne(qi => qi.Question)
                .WithMany(q => q.QuestionImages)
                .HasForeignKey(qi => qi.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);  // 問題刪除時，圖片一起刪除

            modelBuilder.Entity<QuestionOption>()
                .HasOne(qo => qo.Question)
                .WithMany(q => q.QuestionOptions)
                .HasForeignKey(qo => qo.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);  // 問題刪除時，選項一起刪除
        }

    }
}
