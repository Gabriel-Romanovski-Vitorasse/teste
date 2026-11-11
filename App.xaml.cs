using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace EloEventos;

public partial class App : Application
{
    public App()
    {
        // Sem isto, um erro na inicialização fecha o app sem mostrar nada.
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += (_, e) => Relatar(e.ExceptionObject as Exception);
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        Relatar(e.Exception);
        e.Handled = true;
        Shutdown(1);
    }

    /// <summary>Grava o erro em "erro.log" ao lado do executável e mostra na tela.</summary>
    private static void Relatar(Exception? ex)
    {
        string texto = ex?.ToString() ?? "Erro desconhecido.";
        string arquivo = Path.Combine(AppContext.BaseDirectory, "erro.log");
        try { File.WriteAllText(arquivo, texto); } catch (Exception) { /* pasta sem permissão de escrita */ }
        Console.Error.WriteLine(texto);
        MessageBox.Show(texto, "elo · erro ao executar", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
