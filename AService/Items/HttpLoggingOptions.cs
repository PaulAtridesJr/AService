namespace AService.Items
{
	public class HttpLoggingOptions
	{
		public const string ConfigurationSectionName = "HttpLogging";

		public bool Default { get; set; }
		public bool Custom { get; set; }
	}
}
