using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using Ecommerce.Repository.Data;
using Ecommerce.Core.Interfaces;
using Ecommerce.APIs.Helpers;
using Ecommerce.APIs.Middlewares; // ⁄‘«‰ «·‹ Middleware
using Ecommerce.APIs.Errors;      // ⁄‘«‰ ﬂ·«”«  «·‹ Errors
using Ecommerce.Repository.Services;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. Add Services to the container.
// ==========================================

builder.Services.AddControllers();

//  Ê’Ì· «·œ« «»Ì“ (SQL Server)
builder.Services.AddDbContext<StoreContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//  Ê’Ì· Redis (”Ì‘‰ 4)
builder.Services.AddSingleton<IConnectionMultiplexer>(c => {
    var configuration = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis"), true);
    return ConnectionMultiplexer.Connect(configuration);
});

//  ”ÃÌ· «·Œœ„«  (Dependency Injection)
// ------------------------------------
// ”Ì‘‰ 4: Basket & Cache
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddSingleton<IResponseCacheService, ResponseCacheService>();

// ”Ì‘‰ 2: UnitOfWork & GenericRepository
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// ”Ì‘‰ 2: AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfiles));

// ------------------------------------
// ”Ì‘‰ 5: («·Ã“¡ «··Ì «‰  » ”√· ⁄·ÌÂ - Validation Errors)
// ------------------------------------
// «·ﬂÊœ œÂ »Ì€Ì— ‘ﬂ· «·—œ ·„« ÌÕ’· Œÿ√ ›Ì «·»Ì«‰«  („À·« »«⁄  ‰’ „ﬂ«‰ —ﬁ„)
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = actionContext =>
    {
        // 1. »‰„”ﬂ ﬂ· «·√Œÿ«¡ «··Ì Õ’· 
        var errors = actionContext.ModelState
            .Where(e => e.Value.Errors.Count > 0)
            .SelectMany(x => x.Value.Errors)
            .Select(x => x.ErrorMessage).ToArray();

        // 2. »‰ÕÿÂ„ ›Ì «·‘ﬂ· «·„ÊÕœ » «⁄‰«
        var errorResponse = new ApiValidationErrorResponse
        {
            Errors = errors
        };

        // 3. »‰—Ã⁄Â„ ﬂ‹ BadRequest (400)
        return new BadRequestObjectResult(errorResponse);
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ==========================================
// 2. Configure the HTTP request pipeline.
// ==========================================

// ”Ì‘‰ 5: (Middleware) ·«“„ ÌﬂÊ‰ √Ê· Ê«Õœ ⁄‘«‰ Ì·ﬁÿ √Ì Error
app.UseMiddleware<ExceptionMiddleware>();

// ’›Õ… «· ÊÃÌÂ ·Ê Õœ ﬂ » URL €·ÿ (404)
app.UseStatusCodePagesWithReExecute("/errors/{0}");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(); // ⁄‘«‰ «·’Ê—  ŸÂ—

app.UseAuthorization();

app.MapControllers();

//  ‘€Ì· «·‹ Migrations  ·ﬁ«∆Ì« («Œ Ì«—Ì »” „›Ìœ)
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var logger = services.GetRequiredService<ILogger<Program>>();
try
{
    var context = services.GetRequiredService<StoreContext>();
    await context.Database.MigrateAsync();
    await StoreContextSeed.SeedAsync(context); // ·Ê ⁄‰œﬂ Seed
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred during migration");
}

app.Run();