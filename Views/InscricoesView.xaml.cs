using System.Windows;
using System.Windows.Controls;
using EloEventos.Controls;
using EloEventos.Models;
using EloEventos.Services;

namespace EloEventos.Views;

public partial class InscricoesView : UserControl
{
    private static readonly List<Inscricao> Dados = InscricoesData.Criar();
    // Totais do sistema (a tabela mostra a primeira página).
    private static int _confirmadas = 476, _pendentes = 18, _canceladas = 7, _total = 501;
    private static decimal _emAberto = 3762m;

    private Inscricao? _selecionada;
    private bool _preenchendo;
    private readonly bool _pronto;

    public InscricoesView()
    {
        InitializeComponent();
        CampoParticipante.ItemsSource = InscricoesData.Participantes;
        CampoEvento.ItemsSource = InscricoesData.Eventos;
        _pronto = true;
        AtualizarIndicadores();
        Atualizar();

        if (Equals(Navigator.Parameter, "nova")) Nova_Click(this, new RoutedEventArgs());
        else Lista.SelectedIndex = 0;
    }

    private void AtualizarIndicadores()
    {
        IndConfirmadas.Value = _confirmadas.ToString("00");
        IndPendentes.Value = _pendentes.ToString("00");
        IndPendentes.Caption = $"{_emAberto.ToString("C2", Formato.PtBr)} em aberto";
        IndCanceladas.Value = _canceladas.ToString("00");
    }

    private void Atualizar(Inscricao? manterSelecao = null)
    {
        if (!_pronto) return;
        string termo = Busca.Text.Trim();
        string status = FiltroStatus.SelectedIndex > 0 ? ((ComboBoxItem)FiltroStatus.SelectedItem).Content as string ?? "" : "";

        var lista = Dados.Where(i =>
            (status == "" || i.Status == status) &&
            (termo == "" ||
             i.Participante.Contains(termo, StringComparison.CurrentCultureIgnoreCase) ||
             i.Evento.Contains(termo, StringComparison.CurrentCultureIgnoreCase) ||
             i.Id.Contains(termo, StringComparison.CurrentCultureIgnoreCase))).ToList();

        Lista.ItemsSource = lista;
        if (manterSelecao != null) Lista.SelectedItem = manterSelecao;
        Paginacao.Summary = lista.Count == 0
            ? "Nenhuma inscrição encontrada"
            : $"Mostrando 1–{lista.Count} de {_total} inscrições";
    }

    private void Preencher(Inscricao i)
    {
        _selecionada = i;
        _preenchendo = true;

        DadosTitulo.Subtitle = $"{i.Id} · criada em {i.Data:dd/MM/yyyy}, às {i.Data:HH:mm}";
        DadosStatus.Text = i.Status == "Pendente" ? "Pendente de pagamento" : i.Status;
        DadosStatus.Kind = i.StatusKind;
        CampoParticipante.SelectedItem = InscricoesData.Participantes.FirstOrDefault(p => p.Id == i.ParticipanteId);
        CampoEvento.SelectedItem = InscricoesData.Eventos.FirstOrDefault(e => e.Id == i.EventoId);
        CampoData.Text = i.DataTexto;
        CampoStatus.Text = i.Status;
        var evento = InscricoesData.Eventos.FirstOrDefault(e => e.Id == i.EventoId);
        DadosEvento.Text = evento == null ? "" : $"{evento.Periodo} · {evento.Local}";

        // Participante e evento só podem ser trocados enquanto a inscrição está pendente.
        bool pendente = i.Status == "Pendente";
        CampoParticipante.IsEnabled = CampoEvento.IsEnabled = CampoForma.IsEnabled = pendente;

        ValorTotal.Text = i.ValorTexto;
        BotaoPagar.Content = i.Status switch
        {
            "Pendente" => $"Pagar {i.ValorTexto}",
            "Confirmada" => "Pagamento confirmado",
            _ => "Inscrição cancelada",
        };
        BotaoPagar.IsEnabled = pendente;
        BotaoCancelar.IsEnabled = i.Status != "Cancelada";
        AvisoPagamento.Text = i.Status switch
        {
            "Pendente" => "Ao confirmar o pagamento, a inscrição passa para Confirmada e a vaga é garantida.",
            "Confirmada" => "Pagamento aprovado. A vaga está garantida para este participante.",
            _ => "Esta inscrição foi cancelada e a vaga voltou a ficar disponível.",
        };

        _preenchendo = false;
    }

    private void Lista_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Lista.SelectedItem is Inscricao i) Preencher(i);
    }

    private void Participante_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (_preenchendo || _selecionada == null || CampoParticipante.SelectedItem is not ParticipanteOpcao p) return;
        _selecionada.ParticipanteId = p.Id;
        _selecionada.Participante = p.Nome;
        Atualizar(_selecionada);
    }

    private void Evento_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (_preenchendo || _selecionada == null || CampoEvento.SelectedItem is not EventoOpcao ev) return;
        if (_selecionada.Status == "Pendente") _emAberto += ev.Preco - _selecionada.Valor;
        _selecionada.EventoId = ev.Id;
        _selecionada.Evento = ev.NomeCurto;
        _selecionada.Valor = ev.Preco;
        AtualizarIndicadores();
        Atualizar(_selecionada);
        Preencher(_selecionada);
    }

    private void Nova_Click(object sender, RoutedEventArgs e)
    {
        var ev = InscricoesData.Eventos[0];
        var p = InscricoesData.Participantes[0];
        var nova = new Inscricao
        {
            Id = $"INS-{_total + 1:000}",
            ParticipanteId = p.Id, Participante = p.Nome,
            EventoId = ev.Id, Evento = ev.NomeCurto,
            Data = DateTime.Now, Valor = ev.Preco, Status = "Pendente",
        };
        Dados.Insert(0, nova);
        _total++;
        _pendentes++;
        _emAberto += nova.Valor;
        AtualizarIndicadores();
        Busca.Text = "";
        FiltroStatus.SelectedIndex = 0;
        Atualizar(nova);
        CampoParticipante.Focus();
    }

    private void Pagar_Click(object sender, RoutedEventArgs e)
    {
        if (_selecionada is not { Status: "Pendente" } i) return;
        i.Status = "Confirmada";
        i.FormaPagamento = (CampoForma.SelectedItem as ComboBoxItem)?.Content as string ?? i.FormaPagamento;
        _pendentes--;
        _confirmadas++;
        _emAberto -= i.Valor;
        AtualizarIndicadores();
        Atualizar(i);
        Preencher(i);
    }

    private void Cancelar_Click(object sender, RoutedEventArgs e)
    {
        if (_selecionada is not { } i || i.Status == "Cancelada") return;
        var resposta = MessageBox.Show(Window.GetWindow(this)!,
            $"Cancelar a inscrição {i.Id} de {i.Participante}? A vaga será liberada.",
            "Cancelar inscrição", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (resposta != MessageBoxResult.Yes) return;

        if (i.Status == "Pendente") { _pendentes--; _emAberto -= i.Valor; }
        else if (i.Status == "Confirmada") _confirmadas--;
        i.Status = "Cancelada";
        _canceladas++;
        AtualizarIndicadores();
        Atualizar(i);
        Preencher(i);
    }

    private void Busca_Changed(object sender, TextChangedEventArgs e) => Atualizar();
    private void Filtro_Changed(object sender, SelectionChangedEventArgs e) => Atualizar();
}
