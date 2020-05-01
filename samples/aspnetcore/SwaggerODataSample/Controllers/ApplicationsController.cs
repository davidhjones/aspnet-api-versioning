namespace Microsoft.Examples.Controllers
    {
    using Microsoft.AspNet.OData;
    using Microsoft.AspNet.OData.Routing;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Examples.Models;
    using System.Collections.Generic;
    using System.Linq;
    using static Microsoft.AspNet.OData.Query.AllowedQueryOptions;
    using static Microsoft.AspNetCore.Http.StatusCodes;

    /// <summary>
    /// Represents a RESTful service of applications.
    /// </summary>
    [ApiVersion( "1.0" )]
    [ODataRoutePrefix( "Applications" )]
    public class ApplicationsController : ODataController
        {
        /// <summary>
        /// Retrieves all applications.
        /// </summary>
        /// <returns>All available applications.</returns>
        /// <response code="200">Applications successfully retrieved.</response>
        /// <response code="400">The application is invalid.</response>
        [ODataRoute]
        [Produces( "application/json" )]
        [ProducesResponseType( typeof( ODataValue<IEnumerable<Application>> ), Status200OK )]
        [EnableQuery( MaxTop = 100, AllowedQueryOptions = Select | Top | Skip | Count )]
        // Note: ~/api/v1/Contexts({contextId})/People
        public IQueryable<Application> Get ([FromODataUri] string contextId)
            {
            var applications = new[]
            {
                new Application(){ Id = 1, Name = "OneNote", Version = "O365" },
                new Application(){ Id = 2, Name = "Excel", Version = "O365"},
                new Application(){ Id = 3, Name = "VisualStudio", Version = "2019" }
            };

            return applications.AsQueryable();
            }
    }
}