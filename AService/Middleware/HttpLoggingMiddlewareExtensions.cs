namespace AService.Middleware;

public static class HttpLoggingMiddlewareExtensions
{
	public static IApplicationBuilder UseCustomHttpLogging(
		this IApplicationBuilder builder)
	{
		return builder.UseMiddleware<HttpLoggingMiddleware>();
	}
}
