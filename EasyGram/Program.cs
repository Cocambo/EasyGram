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
    options.Password.RequireNonAlphanumeric = false;                    // �� ������� ������� ���� �� ������ �����������-��������� ������� � ������ (��������, @, #, $ � �.�.)
    options.Password.RequiredLength = 8;                                // ����������� ����� ������ � 8 ��������
    options.Password.RequireUppercase = false;                          // �� ������� ������� ���� �� ����� ��������� ����� � ������
    options.Password.RequireLowercase = false;                          // �� ������� ������� ���� �� ����� �������� ����� � ������
    options.User.RequireUniqueEmail = true;                             // �������, ����� email ������� ������������ ��� ����������
    options.SignIn.RequireConfirmedPhoneNumber = false;                 // �� ������� ������������� ������ �������� ��� ����� � �������
    options.SignIn.RequireConfirmedEmail = false;                       // �� ������� ������������� email-������ ��� ����� � �������
    options.SignIn.RequireConfirmedAccount = false;                     // �� ������� ������������� �������� (��������, ����� email ��� �������) ��� ����� � �������
})
    .AddEntityFrameworkStores<AppDbContext>()                           // ��� �������� ������ ������������� ����� �������������� Entity Framework Core � ���������� ���� ������ AppDbContext
    .AddDefaultTokenProviders();                                        // ��������� ����������� ���������� �������

builder.Services.AddScoped<IMarkdownService, MarkdownService>(); // ��������� markdown ������
builder.Services.AddScoped<IExamService, ExamService>(); // Exam ������
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


