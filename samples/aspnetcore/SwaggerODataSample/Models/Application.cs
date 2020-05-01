namespace Microsoft.Examples.Models
{
    using Microsoft.AspNet.OData.Query;

    /// <summary>
    /// Represents an application.
    /// </summary>
    [Select]
    public class Application
    {
        /// <summary>
        /// Gets or sets the unique identifier for the application.
        /// </summary>
        /// <value>The application's unique identifier.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        /// <value>The application's name.</value>
        public string Name { get; set; }


        /// <summary>
        /// Gets or sets the version of the application.
        /// </summary>
        /// <value>The application's version.</value>
        public string Version
            {
            get; set;
            }
        }
}