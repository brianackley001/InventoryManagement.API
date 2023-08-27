using InventoryManagement.API.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog.Web;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics;
using InventoryManagement.API.Hubs;
using InventoryManagement.API.Health;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace InventoryManagement.API
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private readonly string _authAudience;
        private readonly string _authDomain;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly string AllowAllOrigins = "_allowSAllOrigins";

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
            _authDomain = _configuration["Auth0:Domain"];
            _authAudience = _configuration["Auth0:ApiIdentifier"];
        }

        //public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

            services.AddLogging();
            services.AddControllers();
            services.AddApplicationInsightsTelemetry();
            services.AddApplicationInsightsTelemetryProcessor<Api404Filter>();
            services.AddHealthChecks().AddCheck<AppConfigurationHealthCheck>("App Configuration",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "App Configuration" });

            services.AddCors(options =>
            {
                options.AddPolicy(AllowAllOrigins,
                builder =>
                {
                    builder.WithOrigins("https://localhost:8080", 
                        "https://inventorymanagementweb-feature.azurewebsites.net", 
                        "https://inventorymanagementweb-uat.azurewebsites.net",
                        "https://inventorymanagementweb.azurewebsites.net")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
            });
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            }
            );

            //  Auth0 Authorization
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.Authority = _authDomain;
                options.Audience = _authAudience;
                options.RequireHttpsMetadata = !_env.IsDevelopment();

                // Sending the access token in the query string is required due to
                // a limitation in Browser APIs. We restrict it to only calls to the
                // SignalR hub in this code.
                // See https://docs.microsoft.com/aspnet/core/signalr/security#access-token-logging
                // for more information about security considerations when using
                // the query string to transmit the access token.
                //options.Events = new JwtBearerEvents
                //{
                //    OnMessageReceived = context =>
                //    {
                //        var accessToken = context.Request.Query["access_token"];

                //        // If the request is for our hub...
                //        var path = context.HttpContext.Request.Path;
                //        if (!string.IsNullOrEmpty(accessToken) &&
                //            (path.StartsWithSegments("/sync-hub")))
                //        {
                //            // Read the token out of the query string
                //            context.Token = accessToken;
                //        }
                //        return System.Threading.Tasks.Task.CompletedTask;
                //    }
                //};
            });


            //  Scopes
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("read:messages", policy => policy.Requirements.Add(new HasScopeRequirement("read:messages", domain)));
            //});

            //// Register the scope authorization handler
            //services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Dependency Injection Mappings
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            services.AddScoped<Managers.IGroupManager, Managers.GroupManager>();
            services.AddScoped<DataProvider.SQL.IGroupRepository, DataProvider.SQL.GroupRepository>();
            services.AddScoped<Managers.IItemManager, Managers.ItemManager>();
            services.AddScoped<DataProvider.SQL.IItemRepository, DataProvider.SQL.ItemRepository>();
            services.AddScoped<Managers.ITagManager, Managers.TagManager>();
            services.AddScoped<DataProvider.SQL.ITagRepository, DataProvider.SQL.TagRepository>();
            services.AddScoped<Managers.IItemGroupManager, Managers.ItemGroupManager>();
            services.AddScoped<DataProvider.SQL.IItemGroupRepository, DataProvider.SQL.ItemGroupRepository>();
            services.AddScoped<Managers.IItemTagManager, Managers.ItemTagManager>();
            services.AddScoped<DataProvider.SQL.IItemTagRepository, DataProvider.SQL.ItemTagRepository>();
            services.AddScoped<Managers.ISearchManager, Managers.SearchManager>();
            services.AddScoped<DataProvider.SQL.ISearchRepository, DataProvider.SQL.SearchRepository>();
            services.AddScoped<Managers.IProfileSubscriptionManager, Managers.ProfileSubscriptionManager>();
            services.AddScoped<DataProvider.SQL.IProfileSubscriptionRepository, DataProvider.SQL.ProfileSubscriptionRepository>();
            services.AddScoped<Managers.IShoppingListManager, Managers.ShoppingListManager>();
            services.AddScoped<DataProvider.SQL.IShoppingListRepository, DataProvider.SQL.ShoppingListRepository>();
            services.AddScoped<Managers.IProfileSyncManager, Managers.ProfileSyncManager>();


            // SignalR
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "InventoryManagement API", Version = "v1" }); 
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "text/html";

                        await context.Response.WriteAsync("<html lang=\"en\"><body>\r\n");
                        await context.Response.WriteAsync("ERROR!<br><br>\r\n");

                        var exceptionHandlerPathFeature =
                            context.Features.Get<IExceptionHandlerPathFeature>();

                        var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
                        logger.Error(exceptionHandlerPathFeature.Error, "Startup Exception");

                        await context.Response.WriteAsync("<a href=\"/\">Home</a><br>\r\n");
                        await context.Response.WriteAsync("</body></html>\r\n");
                        await context.Response.WriteAsync(new string(' ', 512)); // IE padding
                    });
                });
                app.UseHsts();
            }

            // app.UseCors("AllowSpecificOrigin");
            app.UseCors(AllowAllOrigins);

            app.UseRouting();


            app.UseAuthentication();
            app.UseAuthorization();

            //app.UseHttpsRedirection();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<SyncHub>("/sync-hub"); 
                endpoints.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions()
                {
                    AllowCachingResponses = false
                });
            });
            ////Signal R Hub(s)
            //app.UseSignalR(route =>
            //{
            //    route.MapHub<SyncHub>("/sync-hub");
            //});

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(); 
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "InventoryManagement API");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
