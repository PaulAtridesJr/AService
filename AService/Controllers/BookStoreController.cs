using AService.DTOs;
using AService.Items;
using AService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AService.Controllers
{
	[Route("api/bs")]
	[ApiController]
	public class BookStoreController : ControllerBase
	{
		private readonly BookStoreContext _context;
		private readonly IOptions<ServiceOptions> options;

		public BookStoreController(BookStoreContext context, IOptions<ServiceOptions> options)
		{
			this._context = context;
			this.options = options;
		}

		// GET: api/bs		
		[HttpGet]
		public async Task<IEnumerable<BookDTO>> GetItems()
		{			
			return 
				await _context.Books.
					Select(s => new BookDTO() { Name = s.Name, Authors = s.Authors, CreatedAt = s.CreatedAt, Pages = s.Pages}).
					ToListAsync();
		}

		// GET: api/bs/5
		[HttpGet("{id}")]
		public async Task<ActionResult<BookDTO>> GetItem(long id)
		{
			if (_context.Books == null)
			{
				return NotFound();
			}
			var item = await _context.Books.FindAsync(id);

			if (item == null)
			{
				return NotFound();
			}

			return Ok(new BookDTO() { Name = item.Name, Authors = item.Authors, CreatedAt = item.CreatedAt, Pages = item.Pages });
		}

		// PUT: api/bs/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPut("{id}")]
		public async Task<IActionResult> PutItem(long id, BookDTO item)
		{
			if (id != item.Id)
			{
				return BadRequest();
			}

			var book = await _context.Books.FindAsync(id);
			if (book == null)
			{
				return NotFound();
			}

			Book updatedBook = 
				new() { 
					Id = book.Id,
					CreatedAt = item.CreatedAt,
					Name = item.Name,
					Authors = item.Authors,
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
				if (!ItemExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		// POST: api/bs
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		public async Task<ActionResult<Item>> PostItem(BookDTO item)
		{
			if (_context.Books == null)
			{
				return Problem("Entity set is null.");
			}

			var book = await _context.Books.FindAsync(item.Id);
			if (book != null)
			{
				return Problem($"Entity with id {item.Id} already exists.");
			}

			_context.Books.Add(
				new Book() { 
					Id = item.Id, 
					Authors = item.Authors,
					CreatedAt = item.CreatedAt,
					Name = item.Name, 
					Pages = item.Pages, 
					Price = Random.Shared.Next(1, 1000)
				});
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item);
		}

		// DELETE: api/bs/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteItem(long id)
		{
			if (_context.Books == null)
			{
				return NotFound();
			}
			var item = await _context.Books.FindAsync(id);
			if (item == null)
			{
				return NotFound();
			}

			_context.Books.Remove(item);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		private bool ItemExists(long id)
		{
			return (_context.Books?.Any(e => e.Id == id)).GetValueOrDefault();
		}
	}
}
