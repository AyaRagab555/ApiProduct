using Core.Entities;
using Core.Entities.OrderAggregate;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Data
{
    public class StoreDbContextSeed
    {
        public static async Task SeedAsync(StoreDbContext context , ILoggerFactory loggerFactory)
        {
            try
            {
                if(context.productBrands != null && !context.productBrands.Any())
                {
                    var brandData = File.ReadAllText("../Infrastructure/Data/SeedData/brands.json");
                    var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandData);
                    foreach (var brand in brands)
                        context.productBrands.Add(brand);

                    await context.SaveChangesAsync();
                }

                if (context.productTypes != null && !context.productTypes.Any())
                {
                    var TypeData = File.ReadAllText("../Infrastructure/Data/SeedData/types.json");
                    var Types = JsonSerializer.Deserialize<List<ProductType>>(TypeData);
                    foreach (var Type in Types)
                        context.productTypes.Add(Type);

                    await context.SaveChangesAsync();
                }

                if (context.products != null && !context.products.Any())
                {
                    var productData = File.ReadAllText("../Infrastructure/Data/SeedData/products.json");
                    var products = JsonSerializer.Deserialize<List<Product>>(productData);
                    foreach (var product in products)
                        context.products.Add(product);

                    await context.SaveChangesAsync();
                }
                if (context.DeliveryMethods != null && !context.DeliveryMethods.Any())
                {
                    var deliveryMethodsData = File.ReadAllText("../Infrastructure/Data/SeedData/delivery.json");
                    var deliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryMethodsData);
                    foreach (var deliveryMethod in deliveryMethods)
                        context.DeliveryMethods.Add(deliveryMethod);

                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<StoreDbContextSeed>();
                logger.LogError(ex.Message);
            }
        }
    }
}
