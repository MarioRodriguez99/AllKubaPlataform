using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AllKuba.UI.Services;

public class UpdateService
{
    private const string GITHUB_USER = "TU_USUARIO";
    private const string GITHUB_REPO = "TU_REPO";
    private const string VERSION_ACTUAL = "1.0.0";

    private readonly HttpClient _http;

    public UpdateService()
    {
        _http = new HttpClient();
        _http.DefaultRequestHeaders.Add("User-Agent", "AllKuba-App");
    }

    public async Task<UpdateInfo?> ChequearActualizacionAsync()
    {
        try
        {
            var url = $"https://api.github.com/repos/{GITHUB_USER}/{GITHUB_REPO}/releases/latest";
            var json = await _http.GetStringAsync(url);
            var release = JsonSerializer.Deserialize<GitHubRelease>(json);

            if (release is null) return null;

            var versionRemota = release.TagName.TrimStart('v');

            if (EsVersionMasNueva(versionRemota, VERSION_ACTUAL))
            {
                // Buscar el .zip o .exe en los assets
                var asset = release.Assets?.FirstOrDefault(a =>
                    a.Name.EndsWith(".zip") || a.Name.EndsWith(".exe"));

                return new UpdateInfo
                {
                    VersionNueva = versionRemota,
                    VersionActual = VERSION_ACTUAL,
                    UrlDescarga = asset?.BrowserDownloadUrl ?? release.HtmlUrl,
                    Notas = release.Body ?? ""
                };
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    private static bool EsVersionMasNueva(string remota, string local)
    {
        if (Version.TryParse(remota, out var vRemota) &&
            Version.TryParse(local, out var vLocal))
        {
            return vRemota > vLocal;
        }
        return false;
    }

    public static void AbrirDescarga(string url)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }
}

public class UpdateInfo
{
    public string VersionNueva { get; set; } = "";
    public string VersionActual { get; set; } = "";
    public string UrlDescarga { get; set; } = "";
    public string Notas { get; set; } = "";
}

public class GitHubRelease
{
    [JsonPropertyName("tag_name")]
    public string TagName { get; set; } = "";

    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; } = "";

    [JsonPropertyName("body")]
    public string? Body { get; set; }

    [JsonPropertyName("assets")]
    public List<GitHubAsset>? Assets { get; set; }
}

public class GitHubAsset
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("browser_download_url")]
    public string BrowserDownloadUrl { get; set; } = "";
}
