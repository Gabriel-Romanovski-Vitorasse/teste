using System.Windows;
using System.Windows.Controls;
using EloEventos.Controls;
using EloEventos.Models;
using EloEventos.Services;

namespace EloEventos.Views;

public partial class EventoCadastroView : UserControl
{
    private readonly bool _pronto;

    public EventoCadastroView()
    {
        InitializeComponent();
        LocalCombo.ItemsSource = LocaisData.Todos;
        LocalCombo.SelectedIndex = 0;
        _pronto = true;
        AtualizarResumo();
    }

    private static string Texto(ComboBox combo) => (combo.SelectedItem as ComboBoxItem)?.Content as string ?? "";

    private void AtualizarResumo()
    {
        if (!_pronto) return;

        ResumoTipo.Text = Texto(Tipo);
        ResumoTitulo.Text = string.IsNullOrWhiteSpace(Titulo.Text) ? "Sem título" : Titulo.Text;
        ResumoCapacidade.Text = $"{Capacidade.Text} participantes · {Preco.Text}";
        ResumoLocal.Text = (LocalCombo.SelectedItem as Local)?.Nome ?? "";

        var palestrantes = Texto(Palestrantes);
        bool marina = palestrantes.Contains("Marina");
        bool rafael = palestrantes.Contains("Rafael");
        ChipMarina.Visibility = marina ? Visibility.Visible : Visibility.Collapsed;
        ChipRafael.Visibility = rafael ? Visibility.Visible : Visibility.Collapsed;
        int total = (marina ? 1 : 0) + (rafael ? 1 : 0);
        ResumoPalestrantes.Text = total == 1 ? "1 palestrante selecionado" : $"{total} palestrantes selecionados";
    }

    private void Tipo_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (!_pronto) return;
        bool workshop = Tipo.SelectedIndex == 1;
        CamposWorkshop.IsEnabled = workshop;
        CamposConferencia.IsEnabled = !workshop;
        AtualizarResumo();
    }

    private void Campo_Changed(object sender, RoutedEventArgs e) => AtualizarResumo();

    private void Rascunho_Click(object sender, RoutedEventArgs e)
    {
        RascunhoPill.Text = $"Rascunho salvo às {DateTime.Now:HH:mm}";
        RascunhoPill.Kind = PillKind.Neutral;
    }

    private void Publicar_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(Window.GetWindow(this)!, "Evento salvo e publicado.", "elo",
            MessageBoxButton.OK, MessageBoxImage.Information);
        Navigator.Go(AppPage.EventoDetalhes);
    }

    private void Voltar_Click(object sender, RoutedEventArgs e) => Navigator.Go(AppPage.Eventos);
}
