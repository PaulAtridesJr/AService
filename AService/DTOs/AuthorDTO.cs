namespace AService.DTOs
{
	public class AuthorDTO
	{
		public string? Name { get; init; }
		public DateTime? BirthDate { get; init; }
		public Items.Gender Gender { get; init; }
	}
}
