using Microsoft.EntityFrameworkCore;
using Ecommerce.Repository.Data;
using StackExchange.Redis; 
using Ecommerce.Core.Interfaces;
using Ecommerce.Repository.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database Config
builder.Services.AddDbContext<StoreContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddSingleton<IConnectionMultiplexer>(c => {
var configuration = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis"), true);
return ConnectionMultiplexer.Connect(configuration);



builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

//  Ê’Ì· «·‹ AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfiles));

var app = builder.Build();


builder.Services.AddScoped<IBasketRepository, BasketRepository>();

builder.Services.AddSingleton<IResponseCacheService, ResponseCacheService>();

// Auto-Migrate on Start
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    try
    {
        var context = services.GetRequiredService<StoreContext>();
        await context.Database.MigrateAsync();
        await StoreContextSeed.SeedAsync(context);
    }
    catch (Exception ex)
    {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "Error during migration");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();