using GhostNetwork.Gateway.Facade;
using GhostNetwork.Infrastructure.Repository;
using GhostNetwork.Publications.Api;
using GhostNetwork.Reactions.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace GhostNetwork.Gateway.Api
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "GhostNetwork/Gateway API",
                    Version = "1.0.0"
                });
            });

            services.AddScoped<ICurrentUserProvider, FakeCurrentUserProvider>();

            services.AddScoped<IPublicationsApi>(provider => new PublicationsApi(configuration["PUBLICATIONS_ADDRESS"]));
            services.AddScoped<ICommentsApi>(provider => new CommentsApi(configuration["PUBLICATIONS_ADDRESS"]));
            services.AddScoped<IReactionsApi>(provider => new ReactionsApi(configuration["REACTIONS_ADDRESS"]));

            services.AddScoped<INewsFeedManager, NewsFeedStorage>();

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app
                    .UseSwagger()
                    .UseSwaggerUI(config =>
                    {
                        config.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway V1");
                    });

                app.UseCors(builder => builder.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin());
            }

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}