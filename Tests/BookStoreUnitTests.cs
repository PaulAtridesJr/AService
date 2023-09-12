using AService.Controllers;
using AService.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AService.Tests
{
	public class BookStoreUnitTests
	{
		[Fact]
		public async Task TestGetAllItems()
		{

			Author author1 = new() { Name = "Mark Twain", Gender = Items.Gender.MALE, BirthDate = DateTime.UtcNow };

			Book book1 = new() { Name = "Tom Soyer", Authors = new List<Author> { author1 }, CreatedAt = DateTime.UtcNow, Pages = 100 };

			BookStoreController controller = 
				new BookStoreController(
					BookStore.Mocks.CreateBookStoreContextMock(new List<Book> { book1 }),
					BookStore.Mocks.GetServiceOptionsMock());

			var actionResult = await controller.GetItems();

			Assert.NotNull(actionResult.Value);
			Assert.NotEmpty(actionResult.Value);
		}
	}
}