using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.Swagger;

public class FileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var formFileParameters = context.ApiDescription.ParameterDescriptions
            .Where(p => p.Type == typeof(IFormFile) ||
                       p.Type == typeof(IFormFile[]) ||
                       p.Type == typeof(IEnumerable<IFormFile>))
            .ToList();

        if (!formFileParameters.Any())
            return;

        // Cambiar el content type a multipart/form-data
        operation.RequestBody = new OpenApiRequestBody
        {
            Content =
            {
                ["multipart/form-data"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Properties = context.ApiDescription.ParameterDescriptions
                            .ToDictionary(
                                p => p.Name,
                                p => p.Type == typeof(IFormFile)
                                    ? new OpenApiSchema
                                    {
                                        Type = "string",
                                        Format = "binary"
                                    }
                                    : new OpenApiSchema
                                    {
                                        Type = GetSchemaType(p.Type)
                                    }
                            )
                    }
                }
            }
        };
    }

    private static string GetSchemaType(Type type)
    {
        if (type == typeof(string)) return "string";
        if (type == typeof(int) || type == typeof(long)) return "integer";
        if (type == typeof(decimal) || type == typeof(double) || type == typeof(float)) return "number";
        if (type == typeof(bool)) return "boolean";
        if (type == typeof(DateTime) || type == typeof(DateTimeOffset)) return "string";
        if (type == typeof(TimeSpan)) return "string";
        return "string";
    }
}
