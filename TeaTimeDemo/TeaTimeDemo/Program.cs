using Microsoft.EntityFrameworkCore;
using TeaTimeDemo.DataAccess.Data;
using TeaTimeDemo.DataAccess.Repository;
using TeaTimeDemo.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using TeaTimeDemo.DataAccess.DbInitializer;
using TeaTimeDemo.Utility;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore.Proxies;
using AutoMapper;
using TeaTimeDemo.Mapping; // 引用 AutoMapperProfile 的命名空間



var builder = WebApplication.CreateBuilder(args);

// 添加服務到容器
builder.Services.AddControllersWithViews();


// 配置資料庫連線
builder.Services.AddDbContext<ApplicationDbContext>(options=>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
      .EnableSensitiveDataLogging() // 啟用敏感資料日誌，注意：僅在開發環境中啟用，因為它可能洩露敏感資訊。
      .UseLazyLoadingProxies() // 啟用懶加載代理
    );

// 配置 Identity 系統
builder.Services.AddIdentity<IdentityUser,IdentityRole>(options =>
    options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

// 配置應用程式的 Cookie 行為
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";

});

// 註冊依賴注入的服務
builder.Services.AddScoped<IDbInitializer, DbInitializer>();

// 註冊 IUnitOfWork 與其實作
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IEmailSender, EmailSender>();

// 註冊 IImageService 與其實作
builder.Services.AddScoped<IImageService, ImageService>();

// 新增 SignalR 支援
builder.Services.AddSignalR();

// 加入對 Razor Pages 的支援
builder.Services.AddRazorPages();//新增登登錄

// 註冊 AutoMapper 並掃描 Mapping 資料夾中的配置檔案
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

var app = builder.Build();

// 配置中間件
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// 初始化資料庫
SeedDatabase();

// 啟用身份驗證和授權
app.UseAuthentication();//新增Identity
app.UseAuthorization();


// 配置路由
app.MapControllerRoute(
    name: "default",
    pattern:
    "{area=Customer}/{controller=Home}/{action=Index}/{id?}");


/*
// 設定區域路由

// 1. Admin 區域的路由
app.MapAreaControllerRoute(
    name: "AdminArea",
    areaName: "Admin",
    pattern: "Admin/{controller=Survey}/{action=Index}/{id?}");

// 2. Customer 區域的路由
app.MapAreaControllerRoute(
    name: "CustomerArea",
    areaName: "Customer",
    pattern: "Customer/{controller=Home}/{action=Index}/{id?}");

// 3. 預設路由（無區域時使用）
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
*/

app.MapRazorPages();


app.Run();

void SeedDatabase()
{
  
    using(var scope =app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbInitializer.Initialize ();
    }
}
