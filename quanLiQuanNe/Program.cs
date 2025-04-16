using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using quanLiQuanNe.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình kết nối cơ sở dữ liệu
builder.Services.AddDbContext<quanLiQuanNeContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("quanLiQuanNeContext")
    ?? throw new InvalidOperationException("Connection string 'quanLiQuanNeContext' not found.")));

// Thêm dịch vụ cho controller và view
builder.Services.AddControllersWithViews();

// Thêm dịch vụ cho session
builder.Services.AddDistributedMemoryCache(); // Cung cấp bộ nhớ đệm cho session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Thời gian hết hạn session
    options.Cookie.HttpOnly = true;  // Chỉ có thể truy cập cookie qua HTTP
    options.Cookie.IsEssential = true;  // Cookie này cần thiết cho hoạt động ứng dụng
});

var app = builder.Build();

// Cấu hình HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Đảm bảo rằng tệp tĩnh (CSS, JS, hình ảnh) được phục vụ đúng cách

app.UseRouting();

// Cấu hình session
app.UseSession();

// Cấu hình quyền truy cập
app.UseAuthorization();

// Cấu hình route mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=DangNhap}/{id?}");

app.Run();
