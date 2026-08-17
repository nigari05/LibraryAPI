using Business.DependencyResolver;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebAPI.BackgroundServices;
using WebAPI.Middleware;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddBusinessService(builder.Configuration);

// Add services to the container.
    
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSwaggerGen(options =>
{

    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "LibraryAPI",
        Version = "v1",
        Description =
           "Kitabxana idarəetmə sistemi üçün REST API (Onion Architecture). " +
           "JWT əsaslı autentifikasiya, kitab/müəllif/kateqoriya/üzv idarəetməsi, " +
           "kitab icarəsi (borrow/return), üz qabığı şəklinin yüklənməsi/endirilməsi, " +
           "gündəlik təmizləmə (planlaşdırılmış tapşırıq) və icarə zamanı asinxron " +
           "email bildirişi simulyasiyasını əhatə edir."
    });


    // JWT Bearer autentifikasiyası - Swagger UI-da "Authorize" düyməsi vasitəsilə
    // qorunan (Authorize atributlu) endpoint-ləri birbaşa test etmək mümkün olsun deyə.
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Description = "JWT tokeni daxil edin (yalnız tokenin özü, 'Bearer' prefiksi olmadan)."
    });

    options.AddSecurityRequirement(document => new Microsoft.OpenApi.OpenApiSecurityRequirement
    {
        [new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddAuthorization();

// Planlaşdırılmış tapşırıq (@Scheduled) - gündəlik təmizləmə xidməti. Bax:
// WebAPI/BackgroundServices/DailyCleanupService.cs
builder.Services.AddHostedService<DailyCleanupService>();

// Asinxron emal (@Async) - növbədəki email tapşırıqlarını icra edən worker. Bax:
// WebAPI/BackgroundServices/QueuedBackgroundEmailService.cs
builder.Services.AddHostedService<QueuedBackgroundEmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
