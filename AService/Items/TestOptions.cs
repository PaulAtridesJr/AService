using System.ComponentModel.DataAnnotations;

namespace AService.Items
{
	public sealed class TestOptions
	{
		public const string ConfigurationSectionName = "TestOptions";

		[RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$")]
		public string? TestSetting { get; set; }
		[Url]
		public string? Url { get; set; }

		[Required, EmailAddress]
		public required string Email { get; set; }

		[Required, DataType(DataType.PhoneNumber)]
		public required string PhoneNumber { get; set; }
	}
}
