namespace BlazorMicrofrontends.Core
{
    /// <summary>
    /// Options for publishing to a NuGet feed
    /// </summary>
    public class FeedOptions
    {
        /// <summary>
        /// Gets or sets the feed URL
        /// </summary>
        public string FeedUrl { get; set; }

        /// <summary>
        /// Gets or sets the API key for the feed
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets whether to skip duplicates
        /// </summary>
        public bool SkipDuplicate { get; set; } = false;
    }
} 