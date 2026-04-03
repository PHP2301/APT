using Microsoft.EntityFrameworkCore;
using APT.Data;
using System;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình MySQL (Giữ nguyên vì anh làm đúng rồi)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 2. Cấu hình SESSION (Sửa lại cho chuẩn Docker/Linux)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    // Quan trọng: Không ép SecurePolicy trên Railway vì Railway lo phần HTTPS rồi
    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
});

builder.Services.AddControllersWithViews();
// builder.Services.AddSession(); // Dòng này bị thừa, đã có ở trên rồi

var app = builder.Build();

// 3. Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Hiện lỗi chi tiết khi Dev
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// ⚠️ CHỖ NÀY LÀ ĐIỂM CHẾT: Tắt HttpsRedirection trên Railway
// app.UseHttpsRedirection(); 

app.UseStaticFiles();
app.UseRouting();

// ⚠️ THỨ TỰ PHẢI ĐÚNG: Session -> Routing -> Auth
app.UseSession();
app.UseAuthorization();

// 4. Cấu hình Route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Users}/{action=Login}/{id?}");

app.Run();