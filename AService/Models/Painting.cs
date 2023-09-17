namespace AService.Models
{
	public class Painting : Art
	{
		public Items.PAINTINGSTYLE Style { get; init; }

		public Painting()
		{
			this.KINDOFART = Items.KINDOFART.PAINTING;
		}

		public override string About()
		{
			return $"[{this.KINDOFART}] {this.Name} by {string.Join(", ", this.Authors ?? new())} ({this.CreatedAt}, {this.Style})";
		}
	}
}
