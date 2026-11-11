using System.Windows;
using System.Windows.Controls;
using EloEventos.Models;

namespace EloEventos.Views;

public partial class ParticipantesView : UserControl
{
    private const string TodasInstituicoes = "Todas as instituições";
    private static readonly List<Participante> Dados = ParticipantesData.Criar();
    private static int _totalCadastrado = 128; // total do diretório (a lista mostra a primeira página)

    private Participante? _editando;
    private readonly bool _pronto;

    public ParticipantesView()
    {
        InitializeComponent();
        FiltroInstituicao.ItemsSource = new[] { TodasInstituicoes }
            .Concat(Dados.Select(p => p.Instituicao).Distinct().Order()).ToList();
        FiltroInstituicao.SelectedIndex = 0;
        _pronto = true;
        Atualizar();
        Lista.SelectedIndex = 0;
    }

    private void Atualizar(Participante? manterSelecao = null)
    {
        if (!_pronto) return;
        string termo = Busca.Text.Trim();
        string inst = FiltroInstituicao.SelectedItem as string ?? TodasInstituicoes;

        var lista = Dados.Where(p =>
            (inst == TodasInstituicoes || p.Instituicao == inst) &&
            (termo == "" ||
             p.Nome.Contains(termo, StringComparison.CurrentCultureIgnoreCase) ||
             p.Email.Contains(termo, StringComparison.CurrentCultureIgnoreCase) ||
             p.Cpf.Contains(termo))).ToList();

        Lista.ItemsSource = lista;
        if (manterSelecao != null) Lista.SelectedItem = manterSelecao;
        Paginacao.Summary = lista.Count == 0
            ? "Nenhum participante encontrado"
            : $"Mostrando 1–{lista.Count} de {_totalCadastrado} participantes";
        ResumoTotal.Text = $"{_totalCadastrado} participantes";
    }

    private void Preencher(Participante? p)
    {
        _editando = p;
        Erro.Visibility = Visibility.Collapsed;
        if (p == null)
        {
            FormTitulo.Subtitle = "Novo cadastro";
            CampoId.Text = $"PAR-{Dados.Count + 1:000} · gerado automaticamente";
            CampoNome.Text = CampoCpf.Text = CampoEmail.Text = CampoInstituicao.Text = "";
            InscricoesAna.Visibility = Visibility.Collapsed;
            InscricoesResumo.Visibility = Visibility.Visible;
            InscricoesResumo.Text = "Nenhuma inscrição ainda.";
            CampoNome.Focus();
            return;
        }

        FormTitulo.Subtitle = $"Editando {p.Nome}";
        CampoId.Text = $"{p.Id} · gerado automaticamente";
        CampoNome.Text = p.Nome;
        CampoCpf.Text = p.Cpf;
        CampoEmail.Text = p.Email;
        CampoInstituicao.Text = p.Instituicao;

        bool detalhado = p.Id == "PAR-001";
        InscricoesAna.Visibility = detalhado ? Visibility.Visible : Visibility.Collapsed;
        InscricoesResumo.Visibility = detalhado ? Visibility.Collapsed : Visibility.Visible;
        InscricoesResumo.Text = $"{p.Nome} está inscrito(a) em {p.InscricoesTexto}.";
    }

    private void Lista_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Lista.SelectedItem is Participante p) Preencher(p);
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
        if (new[] { CampoNome, CampoCpf, CampoEmail, CampoInstituicao }.Any(t => string.IsNullOrWhiteSpace(t.Text)))
        {
            Erro.Text = "Preencha todos os campos obrigatórios (*).";
            Erro.Visibility = Visibility.Visible;
            return;
        }

        var alvo = _editando;
        if (alvo == null)
        {
            alvo = new Participante
            {
                Id = $"PAR-{Dados.Count + 1:000}", Nome = "", Email = "", Cpf = "", Instituicao = "", Inscricoes = 0,
            };
            Dados.Add(alvo);
            _totalCadastrado++;
        }

        alvo.Nome = CampoNome.Text.Trim();
        alvo.Cpf = CampoCpf.Text.Trim();
        alvo.Email = CampoEmail.Text.Trim();
        alvo.Instituicao = CampoInstituicao.Text.Trim();
        Atualizar(alvo);
        Preencher(alvo);
    }

    private void Busca_Changed(object sender, TextChangedEventArgs e) => Atualizar();
    private void Filtro_Changed(object sender, SelectionChangedEventArgs e) => Atualizar();
}
