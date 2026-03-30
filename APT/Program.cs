using Microsoft.EntityFrameworkCore;
using APT.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Đăng ký DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. ĐĂNG KÝ DỊCH VỤ SESSION (Bắt buộc để Login hoạt động)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session hết hạn sau 30 phút
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 3. KÍCH HOẠT SESSION (Phải đặt TRƯỚC UseAuthorization và MapControllerRoute)
app.UseSession();

app.UseAuthorization();

// 4. CẤU HÌNH ĐƯỜNG DẪN MẶC ĐỊNH LÀ TRANG LOGIN
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Users}/{action=Login}/{id?}");

app.Run();