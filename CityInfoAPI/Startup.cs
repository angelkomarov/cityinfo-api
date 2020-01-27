using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using CityInfo.API.Entities;
using CityInfo.API.Services;
using CityInfo.API;

namespace CityInfoAPI
{
    //Startup configure Services and Middleware)
    public class Startup
    {
        //CORS Middleware handles cross-origin requests
        readonly string CorsSpecificOrigins = "_corsSpecificOrigins";

        //adding loggin in startup process
        private readonly ILogger _logger;
        //Represents a type used to configure the logging system and create instances of ILogger from the registered ILoggerProviders.
        public ILoggerFactory LoggerFactory { get; }
        //Represents a set of key/value application configuration properties.
        public static IConfiguration Configuration { get; private set;  }
        //Provides information about the web hosting environment an application is running in.
        public IHostingEnvironment Environment { get; }


        //Inject IConfiguration: to get access to asp.net config files (appsettings.json)
        //Inject IHostingEnvironment: to get application's root
        //Inject ILoggerFactory: configuring logger.
        public Startup(IConfiguration configuration, IHostingEnvironment env, ILoggerFactory loggerFactory, ILogger<Startup> logger)
        {
            Configuration = configuration;
            LoggerFactory = loggerFactory;
            _logger = logger;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                //CORS Middleware handles cross-origin requests
                options.AddPolicy(CorsSpecificOrigins,
                builder =>
                {
                    builder.WithOrigins("http://localhost:4200",
                                        "https://localhost:44345")
                                        .AllowAnyHeader()
                                        .AllowAnyMethod();
                });
            });

            //adds the services Razor Pages and MVC require
            services.AddMvc()
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_2)
                .AddMvcOptions(o => o.OutputFormatters.Add( //add XML serialisation to OutputFormatter
                    new XmlDataContractSerializerOutputFormatter()));

            //custom services registration
#if DEBUG
            services.AddTransient<IMailService, LocalMailService>(m => new LocalMailService("hotmail", m.GetRequiredService<ILogger<LocalMailService>>()));
#else
            services.AddTransient<IMailService, CloudMailService>();
#endif
            //!!AK3.2 - providing connection string - set options param
            var connectionString = Startup.Configuration["connectionStrings:cityInfoDBConnectionString"];
            //!!AK3.3 register for DI context (pass connection string in DBContext options param)
            services.AddDbContext<CityInfoContext>(o => o.UseSqlServer(connectionString));
            //!!AK5.5 register Repository service as scoped (per request)
            services.AddScoped<ICityInfoRepository, CityInfoRepository>(r => new CityInfoRepository(r.GetRequiredService<CityInfoContext>()));
            services.AddScoped<ICitiesService, CitiesService>(s => new CitiesService(s.GetRequiredService<ICityInfoRepository>(),
                s.GetRequiredService< ILogger<CitiesService>>()));
            services.AddScoped<IPointsOfInterestService, PointsOfInterestService>(s => new PointsOfInterestService(s.GetRequiredService<ICityInfoRepository>(),
                            s.GetRequiredService<IMailService>(), s.GetRequiredService<ILogger<PointsOfInterestService>>()));

            //services.AddScoped<ICityInfoService, CityInfoService>(s => new CityInfoService(s.GetRequiredService<ICityInfoRepository>(),
            //    LoggerFactory.CreateLogger<CityInfoService>()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //adding Middleware components to the HTTP request pipeline
        //IHostingEnvironment provides the core abstraction for working with environments. It’s provided by ASP.NET hosting layer and can be injected 
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, CityInfoContext cityInfoContext) 
        {
            //configure where to log:
            //adding debug logger that is enabled for Microsoft.Extensions.Logging.LogLevel.Information or higher
            LoggerFactory.AddDebug();
            //adding NLog provider service.
            LoggerFactory.AddProvider(new NLog.Extensions.Logging.NLogLoggerProvider());
            //!!AK handling any un-caught exception - Exception handler middleware
            if (env.IsDevelopment()) //ASPNETCORE_ENVIRONMENT = Development
            {
                _logger.LogInformation("In Development environment");
                //Exception handler middleware - DEV environment 
                //IApplicationBuilder extension method for DeveloperExceptionPageMiddleware.
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //Exception handler middleware - PROD environment
                //IApplicationBuilder extension method for ExceptionHandlerExtensions
                //!!AK any un-caught exception - redirect to Error controller
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }
            //When calling from browser and it returns an error – we can use StatusCodePages middleware to return web page showing the error
            app.UseStatusCodePages();
            //!!AK4.2 call dbcontext extension class to seed the data
            //??AK cityInfoContext.EnsureSeedDataForContext();
            //!!AK7.1 Init aotomapper - mapping source class to target class
            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<CityInfo.API.Entities.City, CityInfo.API.Models.CityWithoutPointsOfInterestDto>();
                cfg.CreateMap<CityInfo.API.Entities.City, CityInfo.API.Models.CityDto>();
                cfg.CreateMap<CityInfo.API.Entities.PointOfInterest, CityInfo.API.Models.PointOfInterestDto>();
                //insert
                cfg.CreateMap<CityInfo.API.Models.PointOfInterestForCreationDto, CityInfo.API.Entities.PointOfInterest>();
                //update
                cfg.CreateMap<CityInfo.API.Models.PointOfInterestForUpdateDto, CityInfo.API.Entities.PointOfInterest>();
                //partial update
                cfg.CreateMap<CityInfo.API.Entities.PointOfInterest, CityInfo.API.Models.PointOfInterestForUpdateDto>();
                cfg.CreateMap<CityInfo.API.Models.PointOfInterestDto, CityInfo.API.Models.PointOfInterestForUpdateDto>();
            });

            //CORS Middleware handles cross-origin requests
            app.UseCors(CorsSpecificOrigins);

            //Add MVC middleware to the request pipeline
            app.UseMvc();
        }
    }
}
