using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AService.Models;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

namespace AService.Tests.BookStore
{
	public class BookStoreIntegrationTests
	{
		//[Fact]
		public async Task MiddlewareTest_ReturnsNotFoundForRequest() 
		{
			using var host = await new HostBuilder()
				.ConfigureWebHost(webBuilder =>
				{
					webBuilder
						.UseTestServer();
						//.ConfigureServices(services =>
						//{
						//	services.AddMyServices();
						//});
						//.Configure(app =>
						//{
						//	app.UseMiddleware<MyMiddleware>();
						//});
				})
				.StartAsync();
		}
	}
}
