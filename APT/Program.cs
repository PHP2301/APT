//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllersWithViews();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.Run();
using Microsoft.EntityFrameworkCore;
using APT.Data;
using System;

var builder = WebApplication.CreateBuilder(args);


// =======================
// 1. DbContext (SSMS)
// =======================
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(
//        builder.Configuration.GetConnectionString("DefaultConnection")));
 // MySQL 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(
            builder.Configuration.GetConnectionString("DefaultConnection")
        )));


// =======================
// 2. SESSION (LOGIN)
// =======================
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // hết hạn sau 30p
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// =======================
// 3. MVC
// =======================
builder.Services.AddControllersWithViews();

var app = builder.Build();


// =======================
// 4. Middleware pipeline
// =======================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


// ⚠️ SESSION PHẢI NẰM Ở ĐÂY
app.UseSession();

app.UseAuthorization();


// =======================
// 5. Default Route → Login
// =======================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Users}/{action=Login}/{id?}");

app.Run();