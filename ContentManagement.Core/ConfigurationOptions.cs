
namespace ContentManagement.Core
{
    /// <summary>
    /// Represents implementation for configuration/options for the <see cref="IContentManager"/>
    /// </summary>
    public class ConfigurationOptions : IConfigurationOptions
    {
        /// <summary>
        /// Gets or sets the file server path in UNC format
        /// </summary>
        public string FileServerPath { get; set; }

        public ConfigurationOptions()
        {
            FileServerPath = string.Empty;
        }
    }
}
