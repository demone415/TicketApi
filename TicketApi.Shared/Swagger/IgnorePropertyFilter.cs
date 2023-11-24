using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TicketApi.Shared.Swagger;

public class IgnorePropertyFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.ApiDescription == null || operation.Parameters == null)
            return;

        if (!context.ApiDescription.ParameterDescriptions.Any())
            return;


        var excludedProperties = context.ApiDescription.ParameterDescriptions.Where(p =>
            p.Source.Equals(BindingSource.Form));

        if (excludedProperties.Any())
            foreach (var excludedPropertie in excludedProperties)
            foreach (var customAttribute in excludedPropertie.CustomAttributes())
                if (customAttribute.GetType() == typeof(SwaggerIgnoreAttribute))
                    for (var i = 0; i < operation.RequestBody.Content.Values.Count; i++)
                    for (var j = 0; j < operation.RequestBody.Content.Values.ElementAt(i).Encoding.Count; j++)
                        if (operation.RequestBody.Content.Values.ElementAt(i).Encoding.ElementAt(j).Key ==
                            excludedPropertie.Name)
                        {
                            operation.RequestBody.Content.Values.ElementAt(i).Encoding
                                .Remove(operation.RequestBody.Content.Values.ElementAt(i).Encoding
                                    .ElementAt(j));
                            operation.RequestBody.Content.Values.ElementAt(i).Schema.Properties
                                .Remove(excludedPropertie.Name);
                        }
    }
}