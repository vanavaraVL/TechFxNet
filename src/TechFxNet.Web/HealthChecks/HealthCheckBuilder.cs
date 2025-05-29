using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;
using System.Text;

namespace TechFxNet.Web.HealthChecks;

/// <summary>
/// Health check builder to build formatted JSON of health response
/// </summary>
public static class HealthCheckBuilder
{
    /// <summary>
    /// Create check
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    public static HealthCheckOptions CreateHealthCheck(Func<HealthCheckRegistration, bool>? filter = null)
    {
        filter ??= _ => true;

        return new HealthCheckOptions
        {
            AllowCachingResponses = false,
            ResultStatusCodes =
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
            },
            ResponseWriter = HealthCheckReportFormatter.WriteResponse,
            Predicate = filter
        };
    }
}

/// <summary>
/// JSON formatter
/// </summary>
public static class HealthCheckReportFormatter
{
    /// <summary>
    /// Write response
    /// </summary>
    /// <param name="context"></param>
    /// <param name="healthReport"></param>
    /// <returns></returns>
    public static Task WriteResponse(HttpContext context, HealthReport healthReport)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var options = new JsonWriterOptions { Indented = false };

        using var memoryStream = new MemoryStream();

        using (var jsonWriter = new Utf8JsonWriter(memoryStream, options))
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WriteString("status", healthReport.Status.ToString());
            jsonWriter.WriteString("duration", healthReport.TotalDuration.ToString());

            if (healthReport.Entries.Any())
            {
                jsonWriter.WriteStartArray("results");

                foreach (var healthReportEntry in healthReport.Entries)
                {
                    jsonWriter.WriteStartObject();

                    jsonWriter.WriteString("system", healthReportEntry.Key);

                    jsonWriter.WriteString("status", healthReportEntry.Value.Status.ToString());

                    jsonWriter.WriteString("description", healthReportEntry.Value.Description);

                    jsonWriter.WriteString("exception", healthReportEntry.Value.Exception?.Message);

                    if (healthReportEntry.Value.Data is { Count: > 0 })
                    {

                        jsonWriter.WriteStartObject("data");

                        foreach (var item in healthReportEntry.Value.Data)
                        {
                            jsonWriter.WritePropertyName(item.Key);

                            JsonSerializer.Serialize(jsonWriter, item.Value, item.Value?.GetType() ?? typeof(object));
                        }

                        jsonWriter.WriteEndObject();
                    }

                    jsonWriter.WriteEndObject();
                }

                jsonWriter.WriteEndArray();
            }

            jsonWriter.WriteEndObject();
        }

        return context.Response.WriteAsync(Encoding.UTF8.GetString(memoryStream.ToArray()));
    }
}