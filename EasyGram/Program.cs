using EasyGram.Data;
using EasyGram.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EasyGram.Services;
using Rotativa.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddIdentity<Users, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;                    // Не требует наличия хотя бы одного неалфавитно-цифрового символа в пароле (например, @, #, $ и т.д.)
    options.Password.RequiredLength = 8;                                // Минимальная длина пароля — 8 символов
    options.Password.RequireUppercase = false;                          // Не требует наличия хотя бы одной заглавной буквы в пароле
    options.Password.RequireLowercase = false;                          // Не требует наличия хотя бы одной строчной буквы в пароле
    options.User.RequireUniqueEmail = true;                             // Требует, чтобы email каждого пользователя был уникальным
    options.SignIn.RequireConfirmedPhoneNumber = false;                 // Не требует подтверждения номера телефона для входа в систему
    options.SignIn.RequireConfirmedEmail = false;                       // Не требует подтверждения email-адреса для входа в систему
    options.SignIn.RequireConfirmedAccount = false;                     // Не требует подтверждения аккаунта (например, через email или телефон) для входа в систему
})
    .AddEntityFrameworkStores<AppDbContext>()                           // для хранения данных пользователей будет использоваться Entity Framework Core с контекстом базы данных AppDbContext
    .AddDefaultTokenProviders();                                        // добавляет стандартные провайдеры токенов

builder.Services.AddScoped<IMarkdownService, MarkdownService>(); // добавляем markdown сервис
builder.Services.AddScoped<IExamService, ExamService>(); // Exam сервис
builder.Services.AddScoped<IEmailService, EmailService>();


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
app.UseRotativa();
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


