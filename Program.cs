using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
// CORS
// ---------------------------------------------------------
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ??
                     ["http://127.0.0.1:5500"];
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


// ---------------------------------------------------------
// Health checks
// ---------------------------------------------------------
builder.Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("DefaultConnection") ??
        throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required"),
        name: "database");

// ---------------------------------------------------------
// JWT Authentication
// ---------------------------------------------------------
var jwtSetting = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSetting["Key"];
if (string.IsNullOrEmpty(jwtKey))
    throw new InvalidOperationException(
        "Jwt:Key is required. Set it in appsettings, User Secrets, or environment (Jwt__Key).");
var key = Encoding.UTF8.GetBytes(jwtKey);

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
builder.Services.AddScoped<AssignmentService>();
builder.Services.AddScoped<ApplicationService>();
builder.Services.AddScoped<ApplicationDocumentService>();
builder.Services.AddScoped<FileResourceService>();
builder.Services.AddScoped<TranscriptRequestService>();
builder.Services.AddScoped<AcademicYearService>();
builder.Services.AddScoped<TermService>();

// ---------------------------------------------------------
// Workflow services (depend on tenant-scoped services above)
// ---------------------------------------------------------
builder.Services.AddScoped<SchoolSystem.Backend.Services.Workflows.AssignmentWorkflowService>();

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
// Global exception handling
// ---------------------------------------------------------
app.UseExceptionHandler(err =>
{
    err.Run(async ctx =>
    {
        ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        ctx.Response.ContentType = "application/json";
        var ex = ctx.Features.Get<IExceptionHandlerFeature>()?.Error;
        if (ex != null)
        {
            var logger = ctx.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Unhandled exception");
            await ctx.Response.WriteAsJsonAsync(new { error = "An error occurred.", message = ex.Message });
        }
    });
});

// ---------------------------------------------------------
// Middleware
// ---------------------------------------------------------
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

//app.MapHealthChecks("/health", new HealthCheckOptions { ResponseWriter = AspNetCore.HealthChecks.UI.Client.UIResponseWriter.WriteHealthCheckUIResponse });

app.Run();