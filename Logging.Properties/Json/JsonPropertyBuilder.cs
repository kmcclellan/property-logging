namespace Microsoft.Extensions.Logging.Properties.Json;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using System.Text.Json;

public class JsonPropertyBuilder<TProvider> : LogPropertyBuilder<TProvider, Utf8JsonWriter>
{
    public JsonPropertyBuilder(IServiceCollection services)
        : base(services)
    {
        this.Options = services.AddOptions<JsonPropertyOptions<TProvider>>();
    }

    public OptionsBuilder<JsonPropertyOptions<TProvider>> Options { get; }
}
