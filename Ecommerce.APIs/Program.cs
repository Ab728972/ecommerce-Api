using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using Ecommerce.Repository.Data;
using Ecommerce.Repository.Identity;
using Ecommerce.Core.Entities.Identity;
using Ecommerce.Core.Interfaces;
using Ecommerce.APIs.Helpers;
using Ecommerce.APIs.Middlewares;
using Ecommerce.APIs.Errors;
using Ecommerce.Repository.Services;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. Add Services to the container.
// ==========================================

builder.Services.AddControllers();

// --- Database 1: Store (Products) ---
builder.Services.AddDbContext<StoreContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// --- Database 2: Identity (Users) [Session 6] ---
builder.Services.AddDbContext<AppIdentityDbContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
});

// --- Redis Connection [Session 4] ---
builder.Services.AddSingleton<IConnectionMultiplexer>(c => {
    var configuration = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis"), true);
    return ConnectionMultiplexer.Connect(configuration);
});

// --- Identity Services Configuration [Session 6] ---
builder.Services.AddIdentityCore<AppUser>(opt => {
    // „„ﬂ‰  ⁄œ· ‘—Êÿ «·»«”Ê—œ Â‰« ·Ê Õ«»»
    // opt.Password.RequireDigit = true;
    // opt.Password.RequireNonAlphanumeric = true;
})
.AddEntityFrameworkStores<AppIdentityDbContext>()
.AddSignInManager<SignInManager<AppUser>>();

// --- Authentication (JWT) Configuration [Session 6] ---
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:Key"])),
            ValidIssuer = builder.Configuration["Token:Issuer"],
            ValidateIssuer = true,
            ValidateAudience = false
        };
    });

// --- Dependency Injection (Services & Repositories) ---
builder.Services.AddScoped<ITokenService, TokenService>();          // Session 6
builder.Services.AddScoped<IBasketRepository, BasketRepository>();      // Session 4
builder.Services.AddSingleton<IResponseCacheService, ResponseCacheService>(); // Session 4
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();              // Session 2
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>)); // Session 2
builder.Services.AddAutoMapper(typeof(MappingProfiles));            // Session 2
builder.Services.AddScoped<IPaymentService, PaymentService>();

// --- Error Handling (Validation Errors) [Session 5] ---
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = actionContext =>
    {
        var errors = actionContext.ModelState
            .Where(e => e.Value.Errors.Count > 0)
            .SelectMany(x => x.Value.Errors)
            .Select(x => x.ErrorMessage).ToArray();

        var errorResponse = new ApiValidationErrorResponse
        {
            Errors = errors
        };

        return new BadRequestObjectResult(errorResponse);
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ==========================================
// 2. Configure the HTTP request pipeline.
// ==========================================

// Exception Middleware (·«“„ √Ê· Ê«Õœ) [Session 5]
app.UseMiddleware<ExceptionMiddleware>();

// ’›Õ… 404 «·„Œ’’… [Session 5]
app.UseStatusCodePagesWithReExecute("/errors/{0}");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

// «· — Ì» Â‰« „Â„ Ãœ« ⁄‘«‰ «·Õ„«Ì…  ‘ €· ’Õ [Session 6]
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// --- Auto Migration & Seeding ---
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var logger = services.GetRequiredService<ILogger<Program>>();
try
{
    // 1. Migrate Store Database
    var context = services.GetRequiredService<StoreContext>();
    await context.Database.MigrateAsync();
    await StoreContextSeed.SeedAsync(context);

    // 2. Migrate Identity Database [Session 6]
    var identityContext = services.GetRequiredService<AppIdentityDbContext>();
    await identityContext.Database.MigrateAsync();
    // „„ﬂ‰ ‰⁄„· Seed ··ÌÊ“—“ Â‰« ›Ì «·„” ﬁ»·
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred during migration");
}

app.Run();