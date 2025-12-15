using Microsoft.EntityFrameworkCore;
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

//  Ê’Ì· «·‹ UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//  Ê’Ì· «·‹ GenericRepository («Õ Ì«ÿÌ ·Ê «Õ Ã‰«Â ·ÊÕœÂ)
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

//  Ê’Ì· «·‹ AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfiles));

var app = builder.Build();

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