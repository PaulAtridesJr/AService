using AService.DTOs;
using AService.Items;
using AService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace AService.Controllers
{
	[Route("api/bs")]
	[ApiController]
	public class BookStoreController : ControllerBase
	{
		private readonly BookStoreContext _context;
		private readonly IOptions<ServiceOptions> _options;
		private readonly ILogger<BookStoreController> _logger;
		private readonly IDistributedCache _cache;

		public BookStoreController(
			BookStoreContext context,
			IOptions<ServiceOptions> options, 
			ILogger<BookStoreController> logger, 
			IDistributedCache cache)
		{
			this._context = context;
			this._options = options;
			this._logger = logger;
			this._cache = cache;
		}

		// GET: api/bs		
		[HttpGet]
		public async Task<IEnumerable<BookDTO>> GetItems()
		{
			this._logger?.BeginScope("All items requested");

			var books = await this._context.Books.Include(a => a.Authors).ToListAsync();
			
			var result = books.Select(s => 
				new BookDTO() { 
					Id = s.Id, 
					Name = s.Name, 
					Authors = 
						s.Authors?.Select(a => new AuthorDTO() { Name = a.Name, BirthDate = a.BirthDate, Gender = a.Gender }).ToList(), 
					CreatedAt = s.CreatedAt, 
					Pages = s.Pages 
				});

			this._logger?.LogDebug("{Count} books found", result.Count());

			return result;
		}

		// GET: api/bs/5
		[HttpGet("{id}")]
		public async Task<ActionResult<BookDTO>> GetItem(long id)
		{
			this._logger?.BeginScope("Book with id {ID} requested", id);

			if (_context.Books == null)
			{
				this._logger?.LogError("Book store is undefined");
				return NotFound();
			}
			var item = await _context.Books.FindAsync(id);

			if (item == null)
			{
				this._logger?.LogWarning("Book with id {ID} was not found", id);
				return NotFound();
			}

			this._logger?.LogDebug("Book with id {ID} was found", id);

			return Ok(new BookDTO() { Name = item.Name, Authors = item.Authors?.Select(a => new AuthorDTO() { Name = a.Name, BirthDate = a.BirthDate, Gender = a.Gender }).ToList(), CreatedAt = item.CreatedAt, Pages = item.Pages });
		}

		// GET: api/bs/bookname/5
		[HttpGet("bookname/{id}")]
		public async Task<ActionResult<string>> GetBookName(long id) 
		{
			this._logger?.BeginScope("Name of book with id {ID} requested", id);

			if (_context.Books == null)
			{
				this._logger?.LogError("Book store is undefined");
				return NotFound();
			}

			string bookName;

			var cashedValue = await this._cache.GetStringAsync(id.ToString());
			if(cashedValue == null) 
			{
				var item = await _context.Books.FindAsync(id);

				if (item == null)
				{
					this._logger?.LogWarning("Book with id {ID} was not found", id);
					return NotFound();
				}
				else 
				{
					bookName = item.Name ?? "_unnamed_";
					this._logger?.LogDebug("Name of book with id {ID} is '{NAME}'", id, bookName);
					
					var cacheOptions = new DistributedCacheEntryOptions
					{
						AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5.0),
						SlidingExpiration = TimeSpan.FromSeconds(3600)
					};
					await this._cache.SetStringAsync(id.ToString(), bookName, cacheOptions);
				}
			}
			else 
			{
				bookName = cashedValue;
				this._logger?.LogDebug("Name of book with id {ID} is '{NAME}' (from cashe)", id, bookName);				
			}

			return Ok(bookName);
		}


		// PUT: api/bs/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPut("{id}")]
		public async Task<IActionResult> PutItem(long id, BookDTO item)
		{
			this._logger?.BeginScope("Book data update with id {ID} requested", id);

			var book = await _context.Books.FindAsync(id);
			if (book == null)
			{
				this._logger?.LogWarning("Book with id {ID} was not found", id);
				return NotFound();
			}

			Book updatedBook = 
				new() { 
					Id = book.Id,
					CreatedAt = item.CreatedAt,
					Name = item.Name,
					Authors = item.Authors?.Select(s => new Author() { Name = s.Name, Gender = s.Gender, BirthDate = s.BirthDate }).ToList(),
					Pages = item.Pages,
					Price = book.Price };

			_context.Books.Remove(book);
			_context.Books.Add(updatedBook);

			try
			{
				await _context.SaveChangesAsync();				
			}
			catch (DbUpdateConcurrencyException)
			{
				this._logger?.LogError("Failed to update book with id {ID} info", id);
				if (!ItemExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			this._logger?.LogWarning("Book with id {ID} info was updated", id);
			return NoContent();
		}

		// POST: api/bs
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		public async Task<ActionResult<Item>> PostItem(BookDTO item)
		{
			this._logger?.BeginScope("Request to insert new book data");

			if (_context.Books == null)
			{
				this._logger?.LogError("Book store is undefined");
				return Problem("Entity set is null.");
			}

			var book = await _context.Books.FindAsync(item.Id);
			if (book != null)
			{
				this._logger?.LogWarning("Book with id {ID} is already exists", item.Id);
				return Problem($"Entity with id {item.Id} already exists.");
			}

			_context.Books.Add(
				new Book() { 
					Id = item.Id, 
					Authors = item.Authors?.Select(s => new Author() { Name = s.Name, Gender = s.Gender, BirthDate = s.BirthDate}).ToList(),
					CreatedAt = item.CreatedAt,
					Name = item.Name, 
					Pages = item.Pages, 
					Price = Random.Shared.Next(1, 1000)
				});
			await _context.SaveChangesAsync();

			this._logger?.LogDebug("Book with id {ID} was added to store", item.Id);

			return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item);
		}

		// DELETE: api/bs/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteItem(long id)
		{
			this._logger?.BeginScope("Request to remove book with id {ID} from store", id);

			if (_context.Books == null)
			{
				this._logger?.LogError("Book store is undefined");
				return NotFound();
			}
			var item = await _context.Books.FindAsync(id);
			if (item == null)
			{
				this._logger?.LogWarning("Book with id {ID} was not found", id);
				return NotFound();
			}

			_context.Books.Remove(item);
			await _context.SaveChangesAsync();

			this._logger?.LogDebug("Book with id {ID} was removed from store", id);

			return NoContent();
		}

		private bool ItemExists(long id)
		{
			return (_context.Books?.Any(e => e.Id == id)).GetValueOrDefault();
		}
	}
}
