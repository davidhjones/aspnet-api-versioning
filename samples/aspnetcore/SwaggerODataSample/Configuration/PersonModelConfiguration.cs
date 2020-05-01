﻿namespace Microsoft.Examples.Configuration
    {
    using Microsoft.AspNet.OData.Builder;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Examples.Models;

    /// <summary>
    /// Represents the model configuration for people.
    /// </summary>
    public class PersonModelConfiguration : IModelConfiguration
        {
        /// <summary>
        /// Applies model configurations using the provided builder for the specified API version.
        /// </summary>
        /// <param name="builder">The <see cref="ODataModelBuilder">builder</see> used to apply configurations.</param>
        /// <param name="apiVersion">The <see cref="ApiVersion">API version</see> associated with the <paramref name="builder"/>.</param>
        public void Apply ( ODataModelBuilder builder, ApiVersion apiVersion )
            {
            var person = builder.EntitySet<Person>( "People" ).EntityType;
            person.HasKey( p => p.Id );
            person.Select().OrderBy( "firstName", "lastName" );
            }
        }
    }