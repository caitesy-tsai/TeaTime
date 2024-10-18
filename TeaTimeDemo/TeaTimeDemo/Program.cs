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
using TeaTimeDemo.Mapping; // �ޥ� AutoMapperProfile ���R�W�Ŷ�



var builder = WebApplication.CreateBuilder(args);

// �K�[�A�Ȩ�e��
builder.Services.AddControllersWithViews();


// �t�m��Ʈw�s�u
builder.Services.AddDbContext<ApplicationDbContext>(options=>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
      .EnableSensitiveDataLogging() // �ҥαӷP��Ƥ�x�A�`�N�G�Ȧb�}�o���Ҥ��ҥΡA�]�����i�ପ�S�ӷP��T�C
      .UseLazyLoadingProxies() // �ҥ��i�[���N�z
    );

// �t�m Identity �t��
builder.Services.AddIdentity<IdentityUser,IdentityRole>(options =>
    options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

// �t�m���ε{���� Cookie �欰
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";

});

// ���U�̿�`�J���A��
builder.Services.AddScoped<IDbInitializer, DbInitializer>();

// ���U IUnitOfWork �P���@
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IEmailSender, EmailSender>();

// ���U IImageService �P���@
builder.Services.AddScoped<IImageService, ImageService>();

// �s�W SignalR �䴩
builder.Services.AddSignalR();

// �[�J�� Razor Pages ���䴩
builder.Services.AddRazorPages();//�s�W�n�n��

// ���U AutoMapper �ñ��y Mapping ��Ƨ������t�m�ɮ�
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

var app = builder.Build();

// �t�m������
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

// ��l�Ƹ�Ʈw
SeedDatabase();

// �ҥΨ������ҩM���v
app.UseAuthentication();//�s�WIdentity
app.UseAuthorization();


// �t�m����
app.MapControllerRoute(
    name: "default",
    pattern:
    "{area=Customer}/{controller=Home}/{action=Index}/{id?}");


/*
// �]�w�ϰ����

// 1. Admin �ϰ쪺����
app.MapAreaControllerRoute(
    name: "AdminArea",
    areaName: "Admin",
    pattern: "Admin/{controller=Survey}/{action=Index}/{id?}");

// 2. Customer �ϰ쪺����
app.MapAreaControllerRoute(
    name: "CustomerArea",
    areaName: "Customer",
    pattern: "Customer/{controller=Home}/{action=Index}/{id?}");

// 3. �w�]���ѡ]�L�ϰ�ɨϥΡ^
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
