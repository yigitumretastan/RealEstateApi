using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using RealEstateApi.DTOs;

public class ListingSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(CreateListingDto))
        {
            schema.Example = new OpenApiObject
            {
                ["title"] = new OpenApiString("Manzaralı daire"),
                ["description"] = new OpenApiString("Denize bakan manzaralı"),
                ["city"] = new OpenApiString("İzmir"),
                ["district"] = new OpenApiString("Karabağlar"),
                ["street"] = new OpenApiString("Polat"),
                ["apartmentNumber"] = new OpenApiString("18"),
                ["roomType"] = new OpenApiString("4+1"),
                ["price"] = new OpenApiDouble(1000000)
            };
        }
        else if (context.Type == typeof(UpdateListingDto))
        {
            schema.Example = new OpenApiObject
            {
                ["title"] = new OpenApiString("Manzaralı daire"),
                ["description"] = new OpenApiString("Denize bakan manzaralı"),
                ["city"] = new OpenApiString("İzmir"),
                ["district"] = new OpenApiString("Karabağlar"),
                ["street"] = new OpenApiString("Polat"),
                ["apartmentNumber"] = new OpenApiString("18"),
                ["roomType"] = new OpenApiString("4+1"),
                ["price"] = new OpenApiDouble(1000000)
            };
        }
    }
}
