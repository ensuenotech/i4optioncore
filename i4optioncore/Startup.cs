using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using i4optioncore.DBModels;
using System.Text;
using i4optioncore.Repositories;
using Microsoft.AspNetCore.Http;
using System;
using StackExchange.Redis;
using i4optioncore.DBModelsUser;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authorization;
using i4optioncore.DBModelsMaster;
using i4optioncore.Repositories.UpdateRedis;
using i4optioncore.Repositories.OptionWindow;
using i4optioncore.Repositories.EOD;
using i4optioncore.Repositories.Dhan;
using i4optioncore.Repositories.GlobalMarket;
using i4optioncore.Repositories.Snapshot;
using Microsoft.Extensions.Options;
using i4optioncore.Services;

namespace i4optioncore
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder().AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build());
            });
            
            services.AddSwaggerGen();
            services.AddOutputCache(outputCache =>
            {
                outputCache.AddBasePolicy(builder => builder.Cache());
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });

            services.AddOptions();
            services.AddMemoryCache();

            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
            services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            services.AddDbContext<i4option_dbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("CS")));
            services.AddDbContext<I4optionUserDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("CSUSER")));
            services.AddDbContext<MasterdataDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("CSMASTERLIVE")));

            services.AddScoped<IUserBL, UserBL>();
            services.AddScoped<IOrderBL, OrderBL>();
            services.AddScoped<ICommonBL, CommonBL>();
            services.AddScoped<IRedisBL, RedisBL>();
            services.AddScoped<IAzureBL, AzureBL>();
            services.AddScoped<IKiteBL, KiteBL>();
            services.AddScoped<ICacheBL, CacheBL>();
            services.AddScoped<IStocksBL, StocksBL>();
            services.AddScoped<IUpdateBL, UpdateBL>();
            services.AddScoped<IOptionWindowBL, OptionWindowBL>();
            services.AddScoped<IEODBL, EODBL>();
            services.AddScoped<IDhanBL, DhanBL>();
            services.AddScoped<IGlobalMarketBL, GlobalMarketBL>();
            services.AddScoped<ISnapshotBL, SnapshotBL>();
            services.AddScoped<IImportBL, ImportBL>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<INotificationService, NotificationService>();

            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
                
            services.AddCors(options => options.AddPolicy("AllowCors", builder =>
            {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            }));

            services.AddMvcCore(option => option.EnableEndpointRouting = false).AddXmlDataContractSerializerFormatters();
            services.AddResponseCaching();

            var multiplexer = ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                AbortOnConnectFail = false,
                EndPoints = { "localhost" }
            });
            services.AddSingleton<IConnectionMultiplexer>(multiplexer);

            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v2";
                    document.Info.Title = "i4option API";
                    document.Info.Description = "All Requests";
                    document.Info.Contact = new NSwag.OpenApiContact
                    {
                        Name = "Ensueno Technologies",
                        Email = string.Empty,
                        Url = "https://www.ensuenotech.com"
                    };
                    document.Info.License = new NSwag.OpenApiLicense
                    {
                        Name = "Use under LICX",
                        Url = "https://www.ensuenotech.com/page/terms-and-conditions"
                    };
                };
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowCors");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseIpRateLimiting();
            app.UseResponseCaching();
            app.UseOutputCache();

            app.UseMvcWithDefaultRoute();

            app.Use(async (context, next) =>
            {
                context.Response.GetTypedHeaders().CacheControl =
                    new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                    {
                        Public = true,
                        MaxAge = TimeSpan.FromSeconds(10)
                    };
                context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] = new[] { "Accept-Encoding" };

                await next();
            });

            app.UseOpenApi();
        }
    }
}
