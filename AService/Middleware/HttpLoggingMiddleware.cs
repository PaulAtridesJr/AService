using System.Globalization;

namespace AService.Middleware;

public class HttpLoggingMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<HttpLoggingMiddleware> logger;

	public HttpLoggingMiddleware(
		RequestDelegate next, 
		ILogger<HttpLoggingMiddleware> logger)
	{
		_next = next;
		this.logger = logger;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		//var cultureQuery = context.Request.Query["culture"];

		
		await _next(context);
	}
}


