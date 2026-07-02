using AllKuba.UI.Services;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AllKuba.UI;

public static class MauiProgram
{
    private static Process? _apiProcess;

#if DEBUG
    private const string ApiUrl = "http://localhost:5236/api/";
    private const string ApiCheck = "http://localhost:5236/api/clientes";
#else
    private const string ApiUrl = "http://localhost:5236/api/";
    private const string ApiCheck = "http://localhost:5236/api/clientes";
#endif

    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddSingleton<AppState>();
        builder.Services.AddSingleton<UpdateService>();

        builder.Services.AddHttpClient<ApiService>(client =>
            client.BaseAddress = new Uri(ApiUrl));

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();

        _ = Task.Run(() => IniciarApiAsync(app.Services));

        return app;
    }

    private static async Task IniciarApiAsync(IServiceProvider services)
    {
        var state = services.GetRequiredService<AppState>();

        try
        {
            state.SetMensaje("Iniciando servicios...");

            var appDir = AppContext.BaseDirectory;
            var apiExe = Path.Combine(appDir, "AllKuba.API.exe");

            if (File.Exists(apiExe))
            {
                state.SetMensaje("Levantando API...");

                _apiProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = apiExe,
                        WorkingDirectory = Path.GetDirectoryName(apiExe),
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    }
                };
                _apiProcess.Start();
            }
#if DEBUG
            else
            {
                // En debug la API se corre manualmente desde VS
                state.SetMensaje("Esperando API...");
            }
#endif

            state.SetMensaje("Conectando...");

            using var http = new HttpClient();
            var intentos = 0;

            while (intentos < 20)
            {
                try
                {
                    var res = await http.GetAsync(ApiCheck);
                    if (res.IsSuccessStatusCode)
                    {
                        state.SetMensaje("¡Listo!");
                        await Task.Delay(600);
                        state.SetApiLista();
                        return;
                    }
                }
                catch { }

                intentos++;
                await Task.Delay(500);
            }

            state.SetMensaje("¡Listo!");
            await Task.Delay(300);
            state.SetApiLista();
        }
        catch
        {
            state.SetApiLista();
        }
    }

    public static void DetenerApi()
    {
        try
        {
            _apiProcess?.Kill(entireProcessTree: true);
            _apiProcess?.Dispose();
        }
        catch { }
    }
}