using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using projectTest.Models;
using projectTest.Services;

var builder = WebApplication.CreateBuilder(args);

// 加入資料庫物件的DI注入
builder.Services.AddDbContext<dbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("projectTestDb")));

builder.Services.AddScoped<IFundingService, FundingService>();
builder.Services.AddSingleton<EmailService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Manager}/{action=ManagerIndex}/{id?}");

app.Run();
