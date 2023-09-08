using System.Configuration;
using System.Security.Claims;
using AService.Items;
using AService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


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

			var app = builder.Build();

			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseAuthorization();

			app.MapGet("/", () => "Hello World!");
			app.MapGet("/secret", (ClaimsPrincipal user) => $"Hello {user.Identity?.Name}. My secret")
				.RequireAuthorization();

			app.MapControllers();

			app.Run();
		}
	}
}