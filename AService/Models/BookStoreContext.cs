using Microsoft.EntityFrameworkCore;

namespace AService.Models
{
	public class BookStoreContext : DbContext
	{
		public BookStoreContext(DbContextOptions<BookStoreContext> options)
			: base(options)
		{
		}

		public virtual DbSet<Book> Books { get; set; } = null!;
		public virtual DbSet<Author> Authors { get; set; } = null!;
	}
}
