using System.Windows;
using System.Windows.Controls;
using EloEventos.Controls;
using EloEventos.Models;

namespace EloEventos.Views;

public partial class OrganizadoresView : UserControl
{
    private static readonly List<Organizador> Dados = OrganizadoresData.Criar();
    private static int _total = 12, _totalPJ = 7, _totalPF = 5;

    // Campos específicos de cada tipo; mudam de bloco conforme a opção escolhida.
    private readonly TextBox _cnpj = NovoInput("00.000.000/0000-00");
    private readonly TextBox _razao = NovoInput("");
    private readonly TextBox _cpf = NovoInput("000.000.000-00");
    private readonly Field _campoCnpj, _campoRazao, _campoCpf;

    private Organizador? _editando;
    private readonly bool _pronto;

    public OrganizadoresView()
    {
        InitializeComponent();
        _campoCnpj = new Field { Label = "CNPJ *", Content = _cnpj };
        _campoRazao = new Field { Label = "Razão social *", Content = _razao };
        _campoCpf = new Field { Label = "CPF *", Content = _cpf };
        _pronto = true;
        Atualizar();
        Lista.SelectedIndex = 0;
    }

    private static TextBox NovoInput(string dica) => new()
    {
        Style = (Style)Application.Current.FindResource("Input"),
        Tag = dica,
    };

    private void Atualizar(Organizador? manterSelecao = null)
    {
        if (!_pronto) return;
        string termo = Busca.Text.Trim();
        int tipo = FiltroTipo.SelectedIndex; // 0 todos, 1 PJ, 2 PF

        var lista = Dados.Where(o =>
            (tipo == 0 || (tipo == 1) == o.Juridica) &&
            (termo == "" ||
             o.Nome.Contains(termo, StringComparison.CurrentCultureIgnoreCase) ||
             o.Cnpj.Contains(termo) || o.Cpf.Contains(termo))).ToList();

        Lista.ItemsSource = lista;
        if (manterSelecao != null) Lista.SelectedItem = manterSelecao;
        Paginacao.Summary = lista.Count == 0
            ? "Nenhum organizador encontrado"
            : $"Mostrando 1–{lista.Count} de {_total} organizadores";
        ResumoTotal.Text = $"{_total} organizadores";
        ResumoPJ.Text = $"{_totalPJ} pessoas jurídicas";
        ResumoPF.Text = $"{_totalPF} pessoas físicas";
    }

    private void AplicarTipo(bool juridica)
    {
        CamposPrincipais.Children.Clear();
        CamposAlternativos.Children.Clear();
        if (juridica)
        {
            TituloPrincipal.Text = "PESSOA JURÍDICA";
            TituloAlternativo.Text = "PESSOA FÍSICA · opção alternativa";
            DicaAlternativa.Text = "Este campo é solicitado ao selecionar Pessoa Física, no lugar de CNPJ e razão social.";
            CamposPrincipais.Children.Add(_campoCnpj);
            CamposPrincipais.Children.Add(_campoRazao);
            CamposAlternativos.Children.Add(_campoCpf);
        }
        else
        {
            TituloPrincipal.Text = "PESSOA FÍSICA";
            TituloAlternativo.Text = "PESSOA JURÍDICA · opção alternativa";
            DicaAlternativa.Text = "Estes campos são solicitados ao selecionar Pessoa Jurídica, no lugar do CPF.";
            CamposPrincipais.Children.Add(_campoCpf);
            CamposAlternativos.Children.Add(_campoCnpj);
            CamposAlternativos.Children.Add(_campoRazao);
        }
    }

    private void Preencher(Organizador? o)
    {
        _editando = o;
        Erro.Visibility = Visibility.Collapsed;
        FormTitulo.Subtitle = o == null ? "Novo cadastro" : $"Editando {o.Nome}";
        CampoId.Text = $"{o?.Id ?? $"ORG-{Dados.Count + 1:000}"} · gerado automaticamente";
        CampoNome.Text = o?.Nome ?? "";
        CampoEmail.Text = o?.Email ?? "";
        CampoTelefone.Text = o?.Telefone ?? "";
        _cnpj.Text = o?.Cnpj ?? "";
        _razao.Text = o?.RazaoSocial ?? "";
        _cpf.Text = o?.Cpf ?? "";

        bool juridica = o?.Juridica ?? true;
        OpcaoPJ.IsChecked = juridica;
        OpcaoPF.IsChecked = !juridica;
        AplicarTipo(juridica);

        if (o == null)
        {
            EventosTitulo.Title = "Eventos do novo organizador";
            EventosTitulo.Subtitle = "Salve o cadastro para vincular eventos";
            EventosLista.ItemsSource = null;
            CampoNome.Focus();
        }
        else
        {
            EventosTitulo.Title = $"Eventos de {o.Nome}";
            EventosTitulo.Subtitle = $"Organizador selecionado · {o.Id}";
            EventosLista.ItemsSource = o.Eventos;
        }
    }

    private void Tipo_Checked(object sender, RoutedEventArgs e)
    {
        if (_pronto) AplicarTipo(OpcaoPJ.IsChecked == true);
    }

    private void Lista_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Lista.SelectedItem is Organizador o) Preencher(o);
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

    private void Salvar_Click(object sender, RoutedEventArgs e)
    {
        bool juridica = OpcaoPJ.IsChecked == true;
        var obrigatorios = new List<TextBox> { CampoNome, CampoEmail, CampoTelefone };
        if (juridica) obrigatorios.AddRange(new[] { _cnpj, _razao });
        else obrigatorios.Add(_cpf);
        if (obrigatorios.Any(t => string.IsNullOrWhiteSpace(t.Text)))
        {
            Erro.Text = "Preencha todos os campos obrigatórios (*).";
            Erro.Visibility = Visibility.Visible;
            return;
        }

        var alvo = _editando;
        if (alvo == null)
        {
            alvo = new Organizador { Id = $"ORG-{Dados.Count + 1:000}", Nome = "", Email = "" };
            Dados.Add(alvo);
            _total++;
            if (juridica) _totalPJ++; else _totalPF++;
        }
        else if (alvo.Juridica != juridica)
        {
            _totalPJ += juridica ? 1 : -1;
            _totalPF += juridica ? -1 : 1;
        }

        alvo.Nome = CampoNome.Text.Trim();
        alvo.Email = CampoEmail.Text.Trim();
        alvo.Telefone = CampoTelefone.Text.Trim();
        alvo.Juridica = juridica;
        alvo.Cnpj = _cnpj.Text.Trim();
        alvo.RazaoSocial = _razao.Text.Trim();
        alvo.Cpf = _cpf.Text.Trim();
        Atualizar(alvo);
        Preencher(alvo);
    }

    private void Busca_Changed(object sender, TextChangedEventArgs e) => Atualizar();
    private void Filtro_Changed(object sender, SelectionChangedEventArgs e) => Atualizar();
}
