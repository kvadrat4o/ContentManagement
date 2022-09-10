using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.Core
{
    /// <summary>
    /// Represents interface for configuration/options for the <see cref="IContentManager"/>
    /// </summary>
    public interface IConfigurationOptions
    {
        /// <summary>
        /// Gets or sets the file server path in UNC format
        /// </summary>
        public string FileServerPath { get; set; }
    }
}
