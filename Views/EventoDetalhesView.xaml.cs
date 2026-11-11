using System.Windows;
using System.Windows.Controls;
using EloEventos.Services;

namespace EloEventos.Views;

public partial class EventoDetalhesView : UserControl
{
    public EventoDetalhesView() => InitializeComponent();

    private void Voltar_Click(object sender, RoutedEventArgs e) => Navigator.Go(AppPage.Eventos);
    private void Editar_Click(object sender, RoutedEventArgs e) => Navigator.Go(AppPage.EventoCadastro);
    private void Inscrever_Click(object sender, RoutedEventArgs e) => Navigator.Go(AppPage.Inscricoes, "nova");

    private void Contato_Click(object sender, RoutedEventArgs e) =>
        MessageBox.Show(Window.GetWindow(this)!, "Contato: contato@conecta.example · (11) 0000-0000",
            "Conecta Educação", MessageBoxButton.OK, MessageBoxImage.Information);

    private void Localizacao_Click(object sender, RoutedEventArgs e) =>
        MessageBox.Show(Window.GetWindow(this)!, "Centro de Convenções Aurora\nAv. dos Encontros, 120 · Vila Nova · São Paulo, SP",
            "Localização", MessageBoxButton.OK, MessageBoxImage.Information);
}
