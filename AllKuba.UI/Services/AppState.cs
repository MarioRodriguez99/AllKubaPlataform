namespace AllKuba.UI.Services;

public class AppState
{
    public bool ApiLista { get; private set; } = false;
    public string Mensaje { get; private set; } = "Iniciando...";

    public event Action? OnChange;

    public void SetMensaje(string mensaje)
    {
        Mensaje = mensaje;
        OnChange?.Invoke();
    }

    public void SetApiLista()
    {
        ApiLista = true;
        OnChange?.Invoke();
    }
}
