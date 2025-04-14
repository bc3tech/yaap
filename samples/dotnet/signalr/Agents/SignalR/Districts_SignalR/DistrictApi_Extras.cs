namespace TBAAPI.V3Client.Api;

using System.ComponentModel;
using System.Text.Json;

using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

using Models.Json;

using TBAAPI.V3Client.Client;
using TBAAPI.V3Client.Model;

public partial class DistrictApi
{
    private ILogger? Log { get; }

    private static readonly JsonDocument EmptyJsonDocument = JsonDocument.Parse("[]");

    public DistrictApi(Configuration config, ILogger logger) : this(config) => this.Log = logger;
}
