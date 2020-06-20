using System.Collections.Generic;

namespace MailChemist.Core.Entities
{
    /// <summary>
    ///
    /// </summary>
    public class EmailContentData
    {
        /// <summary>
        ///
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        ///
        /// </summary>
        public IEnumerable<string> Errors { get; set; }
    }
}