using Microsoft.EntityFrameworkCore;
using quanLiQuanNe.Data;

var builder = WebApplication.CreateBuilder(args);

// Configure database connection
builder.Services.AddDbContext<quanLiQuanNeContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("quanLiQuanNeContext")
    ?? throw new InvalidOperationException("Connection string 'quanLiQuanNeContext' not found.")));

// Add services for controllers and views
builder.Services.AddControllersWithViews();

// Add services for session
builder.Services.AddDistributedMemoryCache(); // Provides memory cache for session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Session timeout
    options.Cookie.HttpOnly = true;  // Accessible only via HTTP
    options.Cookie.IsEssential = true;  // Essential for app functionality
});

var app = builder.Build();

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Use developer exception page in development
}
else
{
    app.UseExceptionHandler("/Home/Error"); // Use error handling in production
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Serve static files

app.UseRouting();

// Configure session
app.UseSession();

// Configure authorization
app.UseAuthorization();

// Configure default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=DangNhap}/{id?}");
app.Run();