using Microsoft.OpenApi.Models;

namespace ApiDemo01.Extensions
{
    public static class SwaggerServices
    {
        public static IServiceCollection AddSwaggerDecumentaion(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",new OpenApiInfo { Title = "Api Demo" , Version = "v1" });
                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "jwt Auth Bearer Schema",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                };
                c.AddSecurityDefinition("Bearer", securitySchema);
                var SecurityRequirement = new OpenApiSecurityRequirement
                {
                    { securitySchema, new string[]{ } }
                };
                c.AddSecurityRequirement(SecurityRequirement);
            });
            return services;
        }

    }
}
