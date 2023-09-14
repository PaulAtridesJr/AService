namespace AService.Models
{
	public static class BookStoreDBInitilizerExtension
	{
		public static IApplicationBuilder UseItToSeedBookStoreDB(this IApplicationBuilder app)
		{
			ArgumentNullException.ThrowIfNull(app, nameof(app));

			using var scope = app.ApplicationServices.CreateScope();
			var services = scope.ServiceProvider;
			
			var context = services.GetRequiredService<BookStoreContext>();
			BookStoreDBInitializer.Initialize(context);
			
			return app;
		}
	}
}
