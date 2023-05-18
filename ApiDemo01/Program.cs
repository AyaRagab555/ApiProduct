using ApiDemo01.Extensions;
using ApiDemo01.MiddleWare;
using Core.Entities.Identity;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace ApiDemo01
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<StoreDbContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddDbContext<AppIdentityDbContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });
            // Add services to the container.
            builder.Services.AddSingleton<IConnectionMultiplexer>(config =>
            {
                var configration = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis"), true);
                return ConnectionMultiplexer.Connect(configration);
            });
            builder.Services.AddApplicationServices();
            builder.Services.AddIdentityServices(builder.Configuration);
            builder.Services.AddSwaggerDecumentaion();
            builder.Services.AddControllers();

            builder.Services.AddCors(option =>
                option.AddPolicy("CrosPolicy", policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed(orgin => true);
                })
            ) ;
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
           

            var app = builder.Build();

            using (var Scope = app.Services.CreateScope())
            {
                var services = Scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                try
                {
                    var context = services.GetRequiredService<StoreDbContext>();
                    await context.Database.MigrateAsync(); 
                    await StoreDbContextSeed.SeedAsync(context, loggerFactory);

                    var userManger = services.GetRequiredService<UserManager<AppUser>>();
                    var identityContext = services.GetRequiredService<AppIdentityDbContext>();
                    await identityContext.Database.MigrateAsync();
                    await AppIdentityDbContextSeed.SeedUserAsync(userManger);

                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(ex, "An Error Accured while seeding Data ");
                }
            }


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json" , "Api Demo v1"));
            }

            app.UseMiddleware<ExceptionMiddleWare>();

            app.UseStaticFiles();

            app.UseCors("CrosPolicy");

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}