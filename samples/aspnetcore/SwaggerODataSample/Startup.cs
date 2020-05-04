namespace Microsoft.Examples
{
    using Microsoft.AspNet.OData;
    using Microsoft.AspNet.OData.Builder;
    using Microsoft.AspNet.OData.Extensions;
    using Microsoft.AspNet.OData.Formatter;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.Examples.Models;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.PlatformAbstractions;
    using Microsoft.Net.Http.Headers;
    using Microsoft.OData.Edm;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using static Microsoft.AspNetCore.Mvc.CompatibilityVersion;
    using static Microsoft.OData.ODataUrlKeyDelimiter;

    /// <summary>
    /// Represents the startup process for the application.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Configures services for the application.
        /// </summary>
        /// <param name="services">The collection of services to configure the application with.</param>
        public void ConfigureServices( IServiceCollection services )
        {
            // the sample application always uses the latest version, but you may want an explicit version such as Version_2_2
            // note: Endpoint Routing is enabled by default; however, it is unsupported by OData and MUST be false
            services.AddMvc( options => options.EnableEndpointRouting = false ).SetCompatibilityVersion( Latest );
            services.AddApiVersioning( options => options.ReportApiVersions = true );
            services.AddOData().EnableApiVersioning();

            // Workaround: https://github.com/OData/WebApi/issues/1177
            services.AddMvcCore( options =>
            {
                foreach ( var outputFormatter in options.OutputFormatters.OfType<ODataOutputFormatter>().Where( _ => _.SupportedMediaTypes.Count == 0 ) )
                    {
                    outputFormatter.SupportedMediaTypes.Add( new MediaTypeHeaderValue( "application/prs.odatatestxx-odata" ) );
                    }
                foreach ( var inputFormatter in options.InputFormatters.OfType<ODataInputFormatter>().Where( _ => _.SupportedMediaTypes.Count == 0 ) )
                    {
                    inputFormatter.SupportedMediaTypes.Add( new MediaTypeHeaderValue( "application/prs.odatatestxx-odata" ) );
                    }
            } );

            services.AddODataApiExplorer(
                options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                } );
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen(
                options =>
                {
                    // add a custom operation filter which sets default values
                    options.OperationFilter<SwaggerDefaultValues>();

                    // integrate xml comments
                    options.IncludeXmlComments( XmlCommentsFilePath );

                    //options.ResolveConflictingActions( apiDescriptions => apiDescriptions.First() );
                } );
        }

        /// <summary>
        /// Configures the application using the provided builder, hosting environment, and logging factory.
        /// </summary>
        /// <param name="app">The current application builder.</param>
        /// <param name="modelBuilder">The <see cref="VersionedODataModelBuilder">model builder</see> used to create OData entity data models (EDMs).</param>
        /// <param name="provider">The API version descriptor provider used to enumerate defined API versions.</param>
        public void Configure( IApplicationBuilder app, VersionedODataModelBuilder modelBuilder, IApiVersionDescriptionProvider provider )
        {
            app.UseDeveloperExceptionPage();

            app.UseMvc(
                routeBuilder =>
                {
                    /*    
                    *   Try to Achieve:
                    *   GET ~/api/v1/People
                    *   GET ~/api/v1/Contexts({contextId})/Applications
                    */

                    var apiVersion = new ApiVersion( 1, 0 );

                    // Add ~/api/v1/People
                    routeBuilder.MapVersionedODataRoutes( "odata-default", "api/v{version:apiVersion}", GetPeopleModels( apiVersion ) );

                    // Add ~/api/v1/Contexts({contextId})/Applications
                    routeBuilder.MapVersionedODataRoutes( "odata-by-context", "api/v{version:apiVersion}/Contexts({contextId})", GetApplicationModels( apiVersion ) );
                } );
            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    // build a swagger endpoint for each discovered API version
                    foreach ( var description in provider.ApiVersionDescriptions )
                    {
                        options.SwaggerEndpoint( $"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant() );
                    }
                } );
            }

        private IEnumerable<IEdmModel> GetPeopleModels ( ApiVersion apiVersion )
            {
            var models = new List<IEdmModel>();
            var builder = new ODataConventionModelBuilder().EnableLowerCamelCase();

            var person = builder.EntitySet<Person>( "People" ).EntityType.HasKey( p => p.Id );

            var model = builder.GetEdmModel();
            model.SetAnnotationValue( model, new ApiVersionAnnotation( apiVersion ) );
            models.Add( model );
            return models;
            }

        private IEnumerable<IEdmModel> GetApplicationModels ( ApiVersion apiVersion )
            {
            var models = new List<IEdmModel>();
            var builder = new ODataConventionModelBuilder().EnableLowerCamelCase();

            var order = builder.EntitySet<Application>( "Applications" ).EntityType.HasKey( o => o.Id );

            var model = builder.GetEdmModel();
            model.SetAnnotationValue( model, new ApiVersionAnnotation( apiVersion ) );
            models.Add( model );
            return models;
            }

        static string XmlCommentsFilePath
        {
            get
            {
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var fileName = typeof( Startup ).GetTypeInfo().Assembly.GetName().Name + ".xml";
                return Path.Combine( basePath, fileName );
            }
        }
    }
}