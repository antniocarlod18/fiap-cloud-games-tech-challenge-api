using FiapCloudGamesTechChallenge.Api.Authorize;
using FiapCloudGamesTechChallenge.Api.Endpoints;
using FiapCloudGamesTechChallenge.Application.Middlewares;
using FiapCloudGamesTechChallenge.Application.Services;
using FiapCloudGamesTechChallenge.Application.Services.Interfaces;
using FiapCloudGamesTechChallenge.Application.Validators;
using FiapCloudGamesTechChallenge.Domain.Repositories;
using FiapCloudGamesTechChallenge.Infra.Data.Context;
using FiapCloudGamesTechChallenge.Infra.Data.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true).Build();

// Configure DbContext
var connectionString = builder.Configuration.GetConnectionString("MySQL");
var serverVersion = new MySqlServerVersion(new Version(8, 0, 44));
builder.Services.AddDbContext<ContextDb>(options =>
{
    options.UseMySql(connectionString, serverVersion);
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddAuthentication(opt => {
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = configuration["Authentication:Issuer"],
        ValidAudience = configuration["Authentication:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Authentication:Key"])),
        RoleClaimType = ClaimTypes.Role
    };
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SameUserOrAdmin", policy =>
        policy.Requirements.Add(new SameUserRequirement()));
});
builder.Services.AddSingleton<IAuthorizationHandler, SameUserHandler>();
builder.Services.AddValidatorsFromAssemblyContaining<UserRequestDtoValidator>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();

app.UseHsts();
app.UseHttpsRedirection();

app.MapGameEndpoints();
app.MapAuditEndpoints();
app.MapOrderEndpoints();
app.MapPromotionEndpoints();
app.MapUserEndpoints();
app.MapAuthEndpoints();

app.Run();