using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SchoolSystem.Backend.Data;
using SchoolSystem.Backend.Interface;
using SchoolSystem.Backend.Services;
using SchoolSystem.Backend.Services.AuthService;
using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Backend.Services.EmailService;

var builder = WebApplication.CreateBuilder(args);

var jwtSetting = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSetting["Key"] ?? throw new InvalidOperationException("JWT key not set"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
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

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SchoolSystem API", Version = "v1" });

    // Add JWT Authentication to Swagger
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

builder.Services.AddControllers();
builder.Services.AddDbContext<SchoolDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Base
builder.Services.AddScoped(typeof(BaseRepository<>));
builder.Services.AddScoped(typeof(BaseService<>));

// CRUD services
builder.Services.AddScoped<TeacherService>();
builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<ParentService>();
builder.Services.AddScoped<SubjectService>();
builder.Services.AddScoped<ClassService>();
builder.Services.AddScoped<GradeService>();
builder.Services.AddScoped<EnrollmentService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TenantService>();

// Workflow services
builder.Services.AddScoped<InvitationService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Email + Notification dependencies
builder.Services.AddScoped<IEmailService, EmailService>();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.UseAuthorization();

app.Run();