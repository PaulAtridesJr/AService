using AService.Controllers;
using AService.Models;

namespace AService.Tests.BookStore
{
	public class BookStoreUnitTests
    {
        [Fact]
        public async Task TestGetAllItems()
        {

            Author author1 = new() { Id = 0, Name = "Mark Twain", Gender = Items.Gender.MALE, BirthDate = DateTime.UtcNow };

            Book book1 = new() { Id = 0, Name = "Tom Soyer", Authors = new List<Author> { author1 }, CreatedAt = DateTime.UtcNow, Pages = 100 };

            BookStoreController controller =
                new BookStoreController(
                    Mocks.CreateBookStoreContextMock(new List<Book> { book1 }),
                    Mocks.GetServiceOptionsMock(),
                    Mocks.GetLoggerMock());

            var actionResult = await controller.GetItems();

            Assert.NotNull(actionResult);
            Assert.NotEmpty(actionResult);
        }
    }
}