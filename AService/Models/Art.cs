namespace AService.Models
{
	public abstract class Art
	{
		public long Id { get; set; }
		public Items.KINDOFART KINDOFART { get; init; }
		public List<Author>? Authors { get; init; }
		public DateTime? CreatedAt { get; init; }
		public string? Name { get; init; }
		public decimal Price { get; init; }

		public abstract string About();
	}
}
