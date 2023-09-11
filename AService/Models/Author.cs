namespace AService.Models
{
	public class Author
	{
		public long Id { get; set; }
		public string? Name { get; init; }
		public DateTime? BirthDate { get; init; }
		public required Items.Gender Gender { get; init; }
	}
}
