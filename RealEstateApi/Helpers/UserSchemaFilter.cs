using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using RealEstateApi.DTOs;

public class UserSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(RegisterDto))
        {
            schema.Example = new OpenApiObject
            {
                ["fullName"] = new OpenApiString("Yiğit Tastan"),
                ["email"] = new OpenApiString("tastanyigitumre@gmail.com"),
                ["password"] = new OpenApiString("Deneme_123")
            };
        }
        else if (context.Type == typeof(LoginDto))
        {
            schema.Example = new OpenApiObject
            {
                ["email"] = new OpenApiString("tastanyigitumre@gmail.com"),
                ["password"] = new OpenApiString("Deneme_123")
            };
        }
        else if (context.Type == typeof(UpdateUserDto))
        {
            schema.Example = new OpenApiObject
            {
                ["fullName"] = new OpenApiString("Yiğit U. Tastan"),
                ["email"] = new OpenApiString("yigit.updated@gmail.com"),
                ["password"] = new OpenApiString("YeniSifre_456")
            };
        }
    }
}
