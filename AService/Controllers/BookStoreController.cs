using System.Web.Http;
using System.Web.Http.Description;
using AService.Items;
using AService.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AService.Controllers
{
	[RoutePrefix("api/bs")]	
	public class BookStoreController : ApiController
	{
		private readonly BookStoreContext _context;
		private readonly IOptions<ServiceOptions> options;

		public BookStoreController(BookStoreContext context, IOptions<ServiceOptions> options)
		{
			this._context = context;
			this.options = options;
		}

		// GET: api/bs		
		[Route("")]
		public async Task<IEnumerable<Book>> GetItems()
		{			
			return await _context.Books.ToListAsync();
		}

		// GET: api/bs/5
		[Route("{id:int}")]
		[ResponseType(typeof(Book))]
		public async Task<IHttpActionResult> GetItem(long id)
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

			return Ok(item);
		}

		// https://learn.microsoft.com/en-us/archive/blogs/youssefm/writing-tests-for-an-asp-net-web-api-service

		//// PUT: api/bs/5
		//// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		//[HttpPut("{id}")]
		//public async Task<IActionResult> PutItem(long id, Book item)
		//{
		//	if (id != item.Id)
		//	{
		//		return BadRequest();
		//	}

		//	_context.Entry(item).State = EntityState.Modified;

		//	try
		//	{
		//		await _context.SaveChangesAsync();
		//	}
		//	catch (DbUpdateConcurrencyException)
		//	{
		//		if (!ItemExists(id))
		//		{
		//			return NotFound();
		//		}
		//		else
		//		{
		//			throw;
		//		}
		//	}

		//	return NoContent();
		//}

		//// POST: api/bs
		//// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		//[HttpPost]
		//public async Task<ActionResult<Item>> PostItem(Book item)
		//{
		//	if (_context.Books == null)
		//	{
		//		return Problem("Entity set 'ItemContext.Items'  is null.");
		//	}
		//	_context.Books.Add(item);
		//	await _context.SaveChangesAsync();

		//	return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item);
		//}

		//// DELETE: api/bs/5
		//[HttpDelete("{id}")]
		//public async Task<IActionResult> DeleteItem(long id)
		//{
		//	if (_context.Books == null)
		//	{
		//		return NotFound();
		//	}
		//	var item = await _context.Books.FindAsync(id);
		//	if (item == null)
		//	{
		//		return NotFound();
		//	}

		//	_context.Books.Remove(item);
		//	await _context.SaveChangesAsync();

		//	return NoContent();
		//}

		//private bool ItemExists(long id)
		//{
		//	return (_context.Books?.Any(e => e.Id == id)).GetValueOrDefault();
		//}
	}
}
