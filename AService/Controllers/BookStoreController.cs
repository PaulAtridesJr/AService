using AService.Items;
using AService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AService.Controllers
{
	[Route("api/bs")]
	[ApiController]
	public class BookStoreController : Controller
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
		public async Task<ActionResult<IEnumerable<Book>>> GetItems()
		{
			if (_context.Books == null)
			{
				return NotFound();
			}
			return await _context.Books.ToListAsync();
		}

		// GET: api/bs/5
		[HttpGet("{id}")]
		public async Task<ActionResult<Book>> GetItem(long id)
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

			return item;
		}

		// PUT: api/bs/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPut("{id}")]
		public async Task<IActionResult> PutItem(long id, Book item)
		{
			if (id != item.Id)
			{
				return BadRequest();
			}

			_context.Entry(item).State = EntityState.Modified;

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
		public async Task<ActionResult<Item>> PostItem(Book item)
		{
			if (_context.Books == null)
			{
				return Problem("Entity set 'ItemContext.Items'  is null.");
			}
			_context.Books.Add(item);
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
