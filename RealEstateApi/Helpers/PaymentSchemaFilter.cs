using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using RealEstateApi.DTOs;

public class PaymentSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(PaymentDto))
        {
            schema.Example = new OpenApiObject
            {
                ["userId"] = new OpenApiInteger(10),
                ["listingId"] = new OpenApiInteger(8),
                ["paymentMethod"] = new OpenApiString("CreditCard"),
                ["cardNumber"] = new OpenApiString("4556 7375 8689 9855"),
                ["cardHolderName"] = new OpenApiString("Yiğit Umre Tastan"),
                ["expiryDate"] = new OpenApiString("06/25"),
                ["cvv"] = new OpenApiString("123"),
                ["billingAddress"] = new OpenApiString("Luxury Apartment"),
                ["billingCity"] = new OpenApiString("İzmir"),
                ["postalCode"] = new OpenApiString("35000"),
                ["description"] = new OpenApiString("Tek çekim ödeme")
            };
        }
    }
}
