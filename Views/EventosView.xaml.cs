using System.Windows;
using System.Windows.Controls;
using EloEventos.Models;
using EloEventos.Services;

namespace EloEventos.Views;

public partial class EventosView : UserControl
{
    private bool? _ordemCrescente; // null = ordem original do cadastro
    private bool _pronto;

    public EventosView()
    {
        InitializeComponent();
        var todos = EventosData.Todos;
        TabTodos.Content = $"Todos os eventos  {todos.Count}";
        TabConferencias.Content = $"Conferências  {todos.Count(e => e.Tipo == "Conferência")}";
        TabWorkshops.Content = $"Workshops  {todos.Count(e => e.Tipo == "Workshop")}";
        _pronto = true;
        Atualizar();
    }

    private void Atualizar()
    {
        if (!_pronto) return;

        string tipo = (TabConferencias.IsChecked == true ? TabConferencias.Tag
                     : TabWorkshops.IsChecked == true ? TabWorkshops.Tag : "") as string ?? "";
        string termo = Busca.Text.Trim();
        string status = FiltroStatus.SelectedIndex > 0 ? ((ComboBoxItem)FiltroStatus.SelectedItem).Content as string ?? "" : "";

        IEnumerable<Evento> lista = EventosData.Todos;
        if (tipo != "") lista = lista.Where(e => e.Tipo == tipo);
        if (status != "") lista = lista.Where(e => e.Status == status);
        if (termo != "")
            lista = lista.Where(e =>
                e.Titulo.Contains(termo, StringComparison.CurrentCultureIgnoreCase) ||
                e.Cidade.Contains(termo, StringComparison.CurrentCultureIgnoreCase) ||
                e.Id.Contains(termo, StringComparison.CurrentCultureIgnoreCase));
        if (_ordemCrescente is bool asc)
            lista = asc ? lista.OrderBy(e => e.Inicio) : lista.OrderByDescending(e => e.Inicio);

        var resultado = lista.ToList();
        Tabela.ItemsSource = resultado;
        Vazio.Visibility = resultado.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        Contagem.Text = resultado.Count == 0
            ? "Nenhum evento para mostrar"
            : $"Mostrando 1–{resultado.Count} de {resultado.Count} eventos";
    }

    private void Tab_Checked(object sender, RoutedEventArgs e) => Atualizar();
    private void Busca_Changed(object sender, TextChangedEventArgs e) => Atualizar();
    private void Filtro_Changed(object sender, SelectionChangedEventArgs e) => Atualizar();

    private void Ordenar_Click(object sender, RoutedEventArgs e)
    {
        _ordemCrescente = _ordemCrescente != true;
        OrdenarTexto.Text = _ordemCrescente == true ? "Ordenar por: data do evento ↑" : "Ordenar por: data do evento ↓";
        Atualizar();
    }

    private void CriarEvento_Click(object sender, RoutedEventArgs e) => Navigator.Go(AppPage.EventoCadastro);
    private void VerEvento_Click(object sender, RoutedEventArgs e) => Navigator.Go(AppPage.EventoDetalhes);
    private void Detalhes_Click(object sender, RoutedEventArgs e) => Navigator.Go(AppPage.EventoDetalhes);
}
