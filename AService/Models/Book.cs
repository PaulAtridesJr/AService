namespace AService.Models
{
	public class Book : Art
	{
		public int Pages { get; init; }

		public Book() 
		{
			this.KINDOFART = Items.KINDOFART.WRITING;
		}

		public override string About()
		{
			return $"[{this.KINDOFART}] {this.Name} by {string.Join(", ", this.Authors ?? new())} ({this.CreatedAt}, {this.Pages} p.)";
		}
	}
}
