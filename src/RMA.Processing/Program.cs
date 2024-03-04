using Microsoft.OpenApi.Models;
using System.Reflection;
using static RMA_Processing.Domain.Utils.LifetimeAttributes;
using RMA_Processing.Domain.Services;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Rewrite;
using System.Text.Json.Serialization;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

#region Configuring Services
void ConfigureSwagger(IServiceCollection services)
{
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(gen =>
    {
        gen.SwaggerDoc("v1", new OpenApiInfo { Title = $"{Assembly.GetExecutingAssembly().GetName().Name}", Version = "v1" });
        gen.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });
        gen.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,
                },
                new List<string>()
            }
        });

        gen.MapType<decimal>(() => new OpenApiSchema { Type = "number", Format = "decimal" });
        gen.MapType<decimal?>(() => new OpenApiSchema { Type = "number", Format = "decimal?" });
    });
}

void ConfigureDependencyInjection(IServiceCollection services, ConfigurationManager configurationManager)
{
    services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    services.Scan(i =>
            i.FromAssembliesOf(new Type[] { 
                // DI needs at least one example from an assembly being used
                typeof(ProcessingService),  // domain
            })

            .AddClasses(c => c.WithAttribute<TransientAttribute>())
            .AsImplementedInterfaces()
            .WithTransientLifetime()

            .AddClasses(c => c.WithAttribute<ScopedAttribute>())
            .AsImplementedInterfaces()
            .WithScopedLifetime()

            .AddClasses(c => c.WithAttribute<SingletonAttribute>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime()
            );
}

void ConfigureHttpClients(IServiceCollection services, ConfigurationManager configurationManager)
{

}
#endregion
#region services
builder.Services.AddRouting(options => options.LowercaseUrls = true);
ConfigureSwagger(builder.Services);
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

ConfigureDependencyInjection(builder.Services, builder.Configuration);
ConfigureHttpClients(builder.Services, builder.Configuration);
#endregion

var app = builder.Build();

if (builder.Environment.IsEnvironment("Local") || builder.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UseRewriter(new RewriteOptions().AddRedirect("^$", "/swagger"));
// Enable middleware to serve generated Swagger as a JSON endpoint.
app.UseSwagger();
// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
// specifying the Swagger JSON endpoint.
app.UseSwaggerUI(c =>
{
    c.EnableDeepLinking();
    c.DisplayRequestDuration();
    c.DefaultModelExpandDepth(2);
    c.DefaultModelsExpandDepth(-1);
    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    c.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
    c.SwaggerEndpoint("v1/swagger.json", "V1");
});

app.UseHttpsRedirection();
app.MapControllers();

app.Run();