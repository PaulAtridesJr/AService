namespace AService.Models
{
	public class BookStoreDBInitializer
	{
		internal static void Initialize(BookStoreContext dbContext)
		{
			ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));

			dbContext.Database.EnsureCreated();

			if (dbContext.Books.Any()) return;

			Author author1 = new() { Name = "Mark Twain", Gender = Items.Gender.MALE, BirthDate = DateTime.UtcNow };

			Book book1 = new() { Name = "Tom Soyer", Authors = new List<Author> { author1 }, CreatedAt = DateTime.UtcNow, Pages = 100 };

			dbContext.Authors.Add(author1);

			dbContext.Books.Add(book1);

			dbContext.SaveChanges();
		}
	}
}
