using AService.Models;

namespace AService.DTOs
{
	public abstract class ArtDTO
	{
		public long Id { get; set; }
		public List<Author>? Authors { get; init; }
		public DateTime? CreatedAt { get; init; }
		public string? Name { get; init; }
	}
}
