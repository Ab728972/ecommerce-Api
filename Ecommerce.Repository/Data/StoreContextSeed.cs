using System.Text.Json;
using Ecommerce.Core.Entities;

namespace Ecommerce.Repository.Data
{
    public static class StoreContextSeed
    {
        public static async Task SeedAsync(StoreContext context)
        {
            // 1. Seeding Brands
            if (!context.ProductBrands.Any())
            {
                var brandsData = File.ReadAllText("../Ecommerce.Repository/Data/DataSeed/brands.json");
                var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);
                if (brands?.Count > 0)
                {
                    foreach (var brand in brands)
                        context.ProductBrands.Add(brand);
                    await context.SaveChangesAsync();
                }
            }

            // 2. Seeding Types
            if (!context.ProductTypes.Any())
            {
                var typesData = File.ReadAllText("../Ecommerce.Repository/Data/DataSeed/types.json");
                var types = JsonSerializer.Deserialize<List<ProductType>>(typesData);
                if (types?.Count > 0)
                {
                    foreach (var type in types)
                        context.ProductTypes.Add(type);
                    await context.SaveChangesAsync();
                }
            }

            // 3. Seeding Products
            if (!context.Products.Any())
            {
                var productsData = File.ReadAllText("../Ecommerce.Repository/Data/DataSeed/products.json");
                var products = JsonSerializer.Deserialize<List<Product>>(productsData);
                if (products?.Count > 0)
                {
                    foreach (var product in products)
                        context.Products.Add(product);
                    await context.SaveChangesAsync();
                }
                if (!context.DeliveryMethods.Any())
                {
                    var dmData = File.ReadAllText("../Ecommerce.Repository/Data/SeedData/delivery.json");
                    var methods = JsonSerializer.Deserialize<List<DeliveryMethod>>(dmData);
                    foreach (var item in methods)
                    {
                        context.DeliveryMethods.Add(item);
                    }
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}