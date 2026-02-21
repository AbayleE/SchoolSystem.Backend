using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SchoolSystem.Backend.Data;
using SchoolSystem.Backend.Interface;
using SchoolSystem.Backend.Services;
using SchoolSystem.Backend.Services.AuthService;
using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------
// JWT Authentication
// ---------------------------------------------------------
var jwtSetting = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSetting["Key"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSetting["Issuer"],
            ValidAudience = jwtSetting["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

// ---------------------------------------------------------
// Swagger + JWT Support
// ---------------------------------------------------------
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SchoolSystem API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ---------------------------------------------------------
// DbContext
// ---------------------------------------------------------
builder.Services.AddDbContext<SchoolDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// CRITICAL: BaseRepository needs DbContext, not SchoolDbContext
builder.Services.AddScoped<DbContext, SchoolDbContext>();

// ---------------------------------------------------------
// Tenant Context
// ---------------------------------------------------------
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantContext, TenantContext>();

// ---------------------------------------------------------
// Generic Repository + Service
// ---------------------------------------------------------
builder.Services.AddScoped(typeof(BaseRepository<>));
builder.Services.AddScoped(typeof(BaseService<>));

// ---------------------------------------------------------
// Tenant-scoped CRUD Services
// ---------------------------------------------------------
builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<TeacherService>();
builder.Services.AddScoped<ParentService>();
builder.Services.AddScoped<SubjectService>();
builder.Services.AddScoped<ClassService>();
builder.Services.AddScoped<GradeService>();
builder.Services.AddScoped<EnrollmentService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<InvitationService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ApplicationService>();
builder.Services.AddScoped<ApplicationDocumentService>();
builder.Services.AddScoped<FileResourceService>();
builder.Services.AddScoped<TranscriptRequestService>();
builder.Services.AddScoped<AcademicYearService>();
builder.Services.AddScoped<TermService>();

// ---------------------------------------------------------
// System-level Services (NOT tenant-scoped)
// ---------------------------------------------------------
builder.Services.AddScoped<TenantService>();
builder.Services.AddScoped<SystemSettingsService>();

// ---------------------------------------------------------
// Email + Identity
// ---------------------------------------------------------
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<PasswordHasher<User>>();

// ---------------------------------------------------------
// MVC
// ---------------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// ---------------------------------------------------------
// Middleware
// ---------------------------------------------------------
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();