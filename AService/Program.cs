using System.Security.Claims;
using AService.Items;
using AService.Middleware;
using AService.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StackExchange.Redis;

namespace AService
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddControllers();
			builder.Services.AddDbContext<ItemContext>(opt =>
				opt.UseInMemoryDatabase("ItemList"));
			builder.Services.AddDbContextPool<BookStoreContext>(opt =>
				opt.UseInMemoryDatabase("BookStore"));

			builder.Services.AddScoped<BookStoreDBInitializer>();

			builder.Services.AddHttpLogging(options =>
			{
				options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
				//options.RequestHeaders.Add("Cache-Control");
				//options.ResponseHeaders.Add("Server");
			});

			// default cache
			//builder.Services.AddDistributedMemoryCache();

			// use redis (should be run in docker before start)
			var configurationOptions = new ConfigurationOptions
			{
				EndPoints = { builder.Configuration["Redis:URL"] },
				Ssl = false
			};
			builder.Services.AddStackExchangeRedisCache(opt => 
			{
				opt.ConfigurationOptions = configurationOptions;
			});

			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			IHostEnvironment env = builder.Environment;

			// don't clean -> IOP ex
			//builder.Configuration.Sources.Clear();

			// already loaded at this point
			//builder.Configuration
			//	.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
			//	.AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);

			// get a config section
			TestOptions testOptions = new() { Email = "help@support.example.com", PhoneNumber = "+7(812)-3452489" };
			builder.Configuration.GetSection(nameof(TestOptions))
				.Bind(testOptions);

			// register a config section object for DI
			builder.Services.Configure<ServiceOptions>(
				builder.Configuration.GetSection(
					key: nameof(ServiceOptions)));

			builder.Services.
				AddOptions<TestOptions>().
				BindConfiguration(TestOptions.ConfigurationSectionName).
				ValidateDataAnnotations();

			builder.Services.AddAuthorization();
			builder.Services.AddAuthentication("Bearer").AddJwtBearer();

			builder.Host.UseSerilog((context, services, configuration) => configuration
				.ReadFrom.Configuration(context.Configuration)
				.ReadFrom.Services(services)
				);

			var app = builder.Build();

			if (app.Environment.IsDevelopment())
			{
				app.UseHttpLogging();
				app.UseItToSeedBookStoreDB();
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseAuthorization();

			app.UseCustomHttpLogging();

			app.MapGet("/", () => "Hello World!");
			app.MapGet("/secret", (ClaimsPrincipal user) => $"Hello {user.Identity?.Name}. My secret")
				.RequireAuthorization();
			app.MapGet("/secret2", () => "This is a different secret!")
				.RequireAuthorization(p => p.RequireClaim("scope", "myapi:secrets"));

			app.MapControllers();

			app.Run();
		}
	}
}