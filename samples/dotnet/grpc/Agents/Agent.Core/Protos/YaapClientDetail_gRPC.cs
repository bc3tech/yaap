namespace Grpc.Models;
public partial class YaapClientDetail
{
    public static implicit operator Yaap.Core.Models.YaapClientDetail(YaapClientDetail grpcYaapClientDetail) =>
        new(
            grpcYaapClientDetail.Name,
            grpcYaapClientDetail.Description,
            string.IsNullOrWhiteSpace(grpcYaapClientDetail.CallbackUrl) ? null : new(grpcYaapClientDetail.CallbackUrl)
        );

    public static implicit operator YaapClientDetail(Yaap.Core.Models.YaapClientDetail yaapClientDetail) =>
        new()
        {
            Name = yaapClientDetail.Name,
            Description = yaapClientDetail.Description,
            CallbackUrl = yaapClientDetail.CallbackUrl?.ToString()
        };
}
