using BudgetTrack.Models;
using Microsoft.EntityFrameworkCore;
using BudgetTrack.Services;

var builder = WebApplication.CreateBuilder(args);

// إضافة MVC
builder.Services.AddControllersWithViews();

// إضافة Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

// إضافة قاعدة البيانات
builder.Services.AddDbContext<BudgetTrackDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BudgetTrackConnection")));

// إضافة خدمة التنبؤ
builder.Services.AddScoped<PredictionService>();

// إضافة Authentication (مهم جدًا)
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Denied";
    });

var app = builder.Build();

// معالجة الأخطاء
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// تشغيل الجلسة
app.UseSession();

// تشغيل تسجيل الدخول
app.UseAuthentication();

// تشغيل الصلاحيات
app.UseAuthorization();

// الراوت الافتراضي
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();