using System.Windows;
using System.Windows.Controls;
using EloEventos.Models;

namespace EloEventos.Views;

public partial class PalestrantesView : UserControl
{
    private const string TodasAreas = "Todas as áreas";
    private static readonly List<Palestrante> Dados = PalestrantesData.Criar();
    private static int _totalCadastrado = 18;
    private static int _certificadosEmitidos = 5;
    private static readonly HashSet<string> Emitidos = [];

    private Palestrante? _editando;
    private readonly bool _pronto;

    public PalestrantesView()
    {
        InitializeComponent();
        FiltroArea.ItemsSource = new[] { TodasAreas }.Concat(Dados.Select(p => p.Especialidade).Distinct().Order()).ToList();
        FiltroArea.SelectedIndex = 0;
        _pronto = true;
        Atualizar();
        Lista.SelectedIndex = 0;
    }

    private void Atualizar(Palestrante? manterSelecao = null)
    {
        if (!_pronto) return;
        string termo = Busca.Text.Trim();
        string area = FiltroArea.SelectedItem as string ?? TodasAreas;

        var lista = Dados.Where(p =>
            (area == TodasAreas || p.Especialidade == area) &&
            (termo == "" ||
             p.Nome.Contains(termo, StringComparison.CurrentCultureIgnoreCase) ||
             p.Especialidade.Contains(termo, StringComparison.CurrentCultureIgnoreCase))).ToList();

        Lista.ItemsSource = lista;
        if (manterSelecao != null) Lista.SelectedItem = manterSelecao;
        Paginacao.Summary = lista.Count == 0
            ? "Nenhum palestrante encontrado"
            : $"Mostrando 1–{lista.Count} de {_totalCadastrado} palestrantes";
        ResumoTotal.Text = $"{_totalCadastrado} palestrantes";
        ResumoCertificados.Text = $"{_certificadosEmitidos} certificados emitidos";
    }

    private void Preencher(Palestrante? p)
    {
        _editando = p;
        Erro.Visibility = Visibility.Collapsed;

        FormTitulo.Subtitle = p == null ? "Novo cadastro" : $"Editando {p.Nome}";
        CampoId.Text = $"{p?.Id ?? $"PAL-{Dados.Count + 1:000}"} · gerado automaticamente";
        CampoNome.Text = p?.Nome ?? "";
        CampoCpf.Text = p?.Cpf ?? "";
        CampoEmail.Text = p?.Email ?? "";
        CampoEspecialidade.Text = p?.Especialidade ?? "";
        CampoBio.Text = p?.Biografia ?? "";

        AvisoVinculo.Visibility = p?.EventoVinculado != null ? Visibility.Visible : Visibility.Collapsed;
        if (p?.EventoVinculado != null)
            AvisoVinculo.Text = $"{p.PrimeiroNome} está vinculado(a) ao {p.EventoVinculado}. A minibiografia será exibida nos detalhes do evento.";
        if (p?.Id == "PAL-001")
            AvisoVinculo.Text = "Marina está vinculada ao Conecta 2026. A minibiografia será exibida nos detalhes do evento.";

        AtualizarCertificado();
        if (p == null) CampoNome.Focus();
    }

    private void AtualizarCertificado()
    {
        var p = _editando;
        if (p?.EventoConcluido is string evento)
        {
            CertEvento.Text = evento;
            CertDetalhe.Text = $"{p.Nome} · palestra realizada em {p.DataPalestra}";
            bool emitido = Emitidos.Contains(p.Id);
            EmitirBotao.IsEnabled = !emitido;
            EmitirBotao.Content = emitido ? "Certificado emitido" : "Emitir certificado";
            EmitirBotao.Visibility = Visibility.Visible;
        }
        else
        {
            CertEvento.Text = "Nenhum evento concluído";
            CertDetalhe.Text = p == null
                ? "Salve o cadastro para vincular o palestrante a um evento."
                : $"{p.Nome} ainda não tem palestras concluídas.";
            EmitirBotao.Visibility = Visibility.Collapsed;
        }
    }

    private void Lista_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Lista.SelectedItem is Palestrante p) Preencher(p);
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
        if (new[] { CampoNome, CampoCpf, CampoEmail, CampoEspecialidade, CampoBio }.Any(t => string.IsNullOrWhiteSpace(t.Text)))
        {
            Erro.Text = "Preencha todos os campos obrigatórios (*).";
            Erro.Visibility = Visibility.Visible;
            return;
        }

        var alvo = _editando;
        if (alvo == null)
        {
            alvo = new Palestrante { Id = $"PAL-{Dados.Count + 1:000}", Nome = "", Email = "", Especialidade = "" };
            Dados.Add(alvo);
            _totalCadastrado++;
        }
        alvo.Nome = CampoNome.Text.Trim();
        alvo.Cpf = CampoCpf.Text.Trim();
        alvo.Email = CampoEmail.Text.Trim();
        alvo.Especialidade = CampoEspecialidade.Text.Trim();
        alvo.Biografia = CampoBio.Text.Trim();
        Atualizar(alvo);
        Preencher(alvo);
    }

    private void Emitir_Click(object sender, RoutedEventArgs e)
    {
        if (_editando == null || !Emitidos.Add(_editando.Id)) return;
        _certificadosEmitidos++;
        ResumoCertificados.Text = $"{_certificadosEmitidos} certificados emitidos";
        AtualizarCertificado();
    }

    private void Busca_Changed(object sender, TextChangedEventArgs e) => Atualizar();
    private void Filtro_Changed(object sender, SelectionChangedEventArgs e) => Atualizar();
}
