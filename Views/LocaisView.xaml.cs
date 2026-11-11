using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EloEventos.Models;

namespace EloEventos.Views;

public partial class LocaisView : UserControl
{
    private static readonly List<Local> Dados = LocaisData.Todos;
    private static int _totalCadastrado = 8;

    private Local? _editando;
    private readonly bool _pronto;

    public LocaisView()
    {
        InitializeComponent();
        _pronto = true;
        Atualizar();
        Lista.SelectedIndex = 0;
    }

    private void Atualizar(Local? manterSelecao = null)
    {
        if (!_pronto) return;
        string termo = Busca.Text.Trim();
        Func<Local, bool> faixa = FiltroCapacidade.SelectedIndex switch
        {
            1 => l => l.Capacidade <= 50,
            2 => l => l.Capacidade is > 50 and <= 200,
            3 => l => l.Capacidade > 200,
            _ => _ => true,
        };

        var lista = Dados.Where(l => faixa(l) &&
            (termo == "" ||
             l.Nome.Contains(termo, StringComparison.CurrentCultureIgnoreCase) ||
             l.EnderecoCompleto.Contains(termo, StringComparison.CurrentCultureIgnoreCase))).ToList();

        Lista.ItemsSource = lista;
        if (manterSelecao != null) Lista.SelectedItem = manterSelecao;
        Paginacao.Summary = lista.Count == 0
            ? "Nenhum local encontrado"
            : $"Mostrando 1–{lista.Count} de {_totalCadastrado} locais";
        ResumoTotal.Text = $"{_totalCadastrado} locais cadastrados";
    }

    private void Preencher(Local? l)
    {
        _editando = l;
        Erro.Visibility = Visibility.Collapsed;
        FormTitulo.Subtitle = l == null ? "Novo cadastro" : $"Editando {l.Nome}";
        CampoId.Text = $"{l?.Id ?? $"LOC-{Dados.Count + 1:000}"} · gerado automaticamente";
        CampoNome.Text = l?.Nome ?? "";
        CampoEndereco.Text = l?.EnderecoCompleto ?? "";
        CampoCapacidade.Text = l?.Capacidade.ToString() ?? "";

        // Destaque: a foto do Figma é do Centro de Convenções Aurora.
        Foto.Visibility = l?.Id == "LOC-001" ? Visibility.Visible : Visibility.Collapsed;
        DestaqueNome.Text = l?.Nome ?? "Novo local";
        DestaqueProximo.Text = l?.ProximoEvento is string prox ? $"Próximo evento: {prox}." : "Nenhum evento agendado.";
        DestaqueOcupacao.Visibility = l?.LugaresDoEvento != null ? Visibility.Visible : Visibility.Collapsed;
        if (l?.LugaresDoEvento is int lugares)
            DestaqueOcupacao.Text = $"{lugares} de {l.Capacidade} lugares destinados ao evento";

        if (l == null) CampoNome.Focus();
    }

    private void Lista_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Lista.SelectedItem is Local l) Preencher(l);
    }

    private void Novo_Click(object sender, RoutedEventArgs e)
    {
        Lista.SelectedItem = null;
        Preencher(null);
    }

    private void Cancelar_Click(object sender, RoutedEventArgs e)
    {
        if (_editando != null) Preencher(_editando);
        else Lista.SelectedIndex = 0;
    }

    private void SoNumeros(object sender, TextCompositionEventArgs e) => e.Handled = !e.Text.All(char.IsDigit);

    private void Salvar_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(CampoNome.Text) || string.IsNullOrWhiteSpace(CampoEndereco.Text) ||
            !int.TryParse(CampoCapacidade.Text, out int capacidade) || capacidade <= 0)
        {
            Erro.Text = "Preencha nome, endereço e uma capacidade maior que zero.";
            Erro.Visibility = Visibility.Visible;
            return;
        }
        if (_editando?.LugaresDoEvento is int reservados && capacidade < reservados)
        {
            Erro.Text = $"A capacidade não pode ficar abaixo dos {reservados} lugares já destinados a um evento.";
            Erro.Visibility = Visibility.Visible;
            return;
        }

        var alvo = _editando;
        if (alvo == null)
        {
            alvo = new Local { Id = $"LOC-{Dados.Count + 1:000}", Nome = "", Endereco = "" };
            Dados.Add(alvo);
            _totalCadastrado++;
        }

        // Endereço: os dois primeiros trechos viram a linha curta da tabela; o resto, o complemento.
        var partes = CampoEndereco.Text.Split('·', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        alvo.Endereco = string.Join(" · ", partes.Take(2));
        alvo.Complemento = string.Join(" · ", partes.Skip(2));
        alvo.Nome = CampoNome.Text.Trim();
        alvo.Capacidade = capacidade;
        Atualizar(alvo);
        Preencher(alvo);
    }

    private void Busca_Changed(object sender, TextChangedEventArgs e) => Atualizar();
    private void Filtro_Changed(object sender, SelectionChangedEventArgs e) => Atualizar();
}
