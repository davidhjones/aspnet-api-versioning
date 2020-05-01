namespace Microsoft.Examples.Controllers
    {
    using Microsoft.AspNet.OData;
    using Microsoft.AspNet.OData.Query;
    using Microsoft.AspNet.OData.Routing;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Examples.Models;
    using Microsoft.OData;
    using System.Collections.Generic;
    using System.Linq;
    using static Microsoft.AspNet.OData.Query.AllowedQueryOptions;
    using static Microsoft.AspNetCore.Http.StatusCodes;

    /// <summary>
    /// Represents a RESTful people service.
    /// </summary>
    [ApiVersion( "1.0" )]
    [ODataRoutePrefix( "People" )]
    public class PeopleController : ODataController
        {
        /// <summary>
        /// Gets all people.
        /// </summary>
        /// <param name="options">The current OData query options.</param>
        /// <returns>All available people.</returns>
        /// <response code="200">The successfully retrieved people.</response>
        [ODataRoute]
        [Produces( "application/json" )]
        [ProducesResponseType( typeof( ODataValue<IEnumerable<Person>> ), Status200OK )]
        // Note: ~/api/v1/People
        public IActionResult Get ( ODataQueryOptions<Person> options )
            {
            var validationSettings = new ODataValidationSettings()
                {
                AllowedQueryOptions = Select | OrderBy | Top | Skip | Count,
                AllowedOrderByProperties = { "firstName", "lastName" },
                AllowedArithmeticOperators = AllowedArithmeticOperators.None,
                AllowedFunctions = AllowedFunctions.None,
                AllowedLogicalOperators = AllowedLogicalOperators.None,
                MaxOrderByNodeCount = 2,
                MaxTop = 100,
                };

            try
                {
                options.Validate( validationSettings );
                }
            catch ( ODataException )
                {
                return BadRequest();
                }

            var people = new[]
            {
                new Person()
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@somewhere.com",
                    Phone = "555-987-1234",
                },
                new Person()
                {
                    Id = 2,
                    FirstName = "Bob",
                    LastName = "Smith",
                    Email = "bob.smith@somewhere.com",
                    Phone = "555-654-4321",
                },
                new Person()
                {
                    Id = 3,
                    FirstName = "Jane",
                    LastName = "Doe",
                    Email = "jane.doe@somewhere.com",
                    Phone = "555-789-3456",
                }
            };

            return Ok( options.ApplyTo( people.AsQueryable() ) );
            }
        }
    }