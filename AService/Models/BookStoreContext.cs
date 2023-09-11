using Microsoft.EntityFrameworkCore;

namespace AService.Models
{
	public class BookStoreContext : DbContext
	{
		public BookStoreContext(DbContextOptions<BookStoreContext> options)
			: base(options)
		{
#if DEBUG
			Author author1 = new Author() { Name = "Mark Twain", Gender = Items.Gender.MALE, BirthDate = DateTime.UtcNow };

			Book book1 = new Book() { Name = "Tom Soyer", Authors = new List<Author> { author1}, CreatedAt = DateTime.UtcNow, Pages = 100 };

			this.Add<Book>(book1);

			this.SaveChanges();
#endif
		}

		public DbSet<Book> Books { get; set; } = null!;		
	}
}
