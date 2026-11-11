using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using EloEventos.Controls;
using EloEventos.Models;
using Microsoft.Win32;

namespace EloEventos.Views;

public partial class CertificadosView : UserControl
{
    private static readonly List<Certificado> Dados = CertificadosData.Criar();
    private static int _emitidosNoSistema = 12; // total de certificados emitidos em todos os eventos

    private Certificado? _selecionado;
    private bool _sincronizando;
    private readonly bool _pronto;

    public CertificadosView()
    {
        InitializeComponent();
        CarregarSelo();
        _pronto = true;
        AtualizarDestinatarios();
        Atualizar();
        Lista.SelectedIndex = 0;
    }

    /// <summary>O ícone "award" do Figma só é exibido se tiver sido baixado para Assets/Icons.</summary>
    private void CarregarSelo()
    {
        var uri = new Uri("/Assets/Icons/award.svg", UriKind.Relative);
        try
        {
            if (Application.GetResourceStream(uri) == null) return;
            SeloSvg.Source = uri;
            Selo.Visibility = Visibility.Visible;
        }
        catch (IOException) { /* recurso ausente: mantém o selo oculto */ }
    }

    private void Atualizar(Certificado? manterSelecao = null)
    {
        if (!_pronto) return;
        string termo = Busca.Text.Trim();
        string tipo = FiltroTipo.SelectedIndex switch { 1 => "Participante", 2 => "Palestrante", _ => "" };

        var lista = Dados.Where(c =>
            (tipo == "" || c.Tipo == tipo) &&
            (termo == "" ||
             c.Destinatario.Contains(termo, StringComparison.CurrentCultureIgnoreCase) ||
             c.Codigo.Contains(termo, StringComparison.CurrentCultureIgnoreCase))).ToList();

        Lista.ItemsSource = lista;
        if (manterSelecao != null) Lista.SelectedItem = manterSelecao;
        Paginacao.Summary = lista.Count == 0 ? "Nenhum certificado encontrado" : $"Mostrando 1–{lista.Count} de {Dados.Count}";

        int pendentes = Dados.Count(c => !c.Emitido);
        ResumoEmitidos.Text = $"{_emitidosNoSistema} certificados emitidos";
        ResumoPendentes.Text = pendentes == 1 ? "1 pendente" : $"{pendentes} pendentes";
    }

    private void AtualizarDestinatarios()
    {
        string tipo = CampoTipo.SelectedIndex == 1 ? "Palestrante" : "Participante";
        CampoDestinatario.ItemsSource = Dados.Where(c => c.Tipo == tipo).ToList();
        CampoDestinatario.SelectedIndex = 0;
    }

    private void Mostrar(Certificado c)
    {
        _selecionado = c;
        PreviaTitulo.Text = c.Tipo == "Palestrante" ? "Certificado de palestrante" : "Certificado de participação";
        PreviaNome.Text = c.Destinatario;
        PreviaTexto.Text = c.Reconhecimento;
        PreviaCodigo.Text = $"Código de validação: {c.Codigo} · {c.LinkValidacao}";
        PreviaStatus.Text = c.Emitido ? "Emitido" : "Rascunho";
        PreviaStatus.Kind = c.Emitido ? PillKind.Success : PillKind.Neutral;

        DetalheEvento.Text = c.Evento;
        DetalhePeriodo.Text = $"{c.EventoPeriodo} · {c.CargaHoraria}";
        DetalheCodigo.Text = c.Codigo;
        DetalheEmissao.Text = c.Emitido ? $"Emitido em {c.EmitidoTexto}" : "Ainda não emitido";

        BotaoBaixar.IsEnabled = BotaoCopiar.IsEnabled = c.Emitido;
        Aviso.Text = c.Emitido
            ? "O link de validação permite que qualquer pessoa confira a autenticidade do certificado."
            : "Emita o certificado para liberar o download e o link de validação.";

        BotaoCopiar.Content = "Copiar link de validação";

        // Mantém o formulário de emissão sincronizado com a seleção.
        _sincronizando = true;
        CampoTipo.SelectedIndex = c.Tipo == "Palestrante" ? 1 : 0;
        AtualizarDestinatarios();
        CampoDestinatario.SelectedItem = c;
        _sincronizando = false;
        BotaoEmitir.IsEnabled = !c.Emitido;
        BotaoEmitir.Content = c.Emitido ? "Certificado já emitido" : "Emitir certificado";
    }

    private void Lista_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Lista.SelectedItem is Certificado c) Mostrar(c);
    }

    private void Tipo_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (!_pronto || _sincronizando) return;
        var atual = _selecionado;
        AtualizarDestinatarios();
        if (atual != null && CampoDestinatario.Items.Contains(atual)) CampoDestinatario.SelectedItem = atual;
    }

    private void Destinatario_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (!_pronto || _sincronizando || CampoDestinatario.SelectedItem is not Certificado c || c == _selecionado) return;
        Lista.SelectedItem = c;
        if (Lista.SelectedItem != c) Mostrar(c); // fora do filtro atual
    }

    private void Emitir_Click(object sender, RoutedEventArgs e)
    {
        if (CampoDestinatario.SelectedItem is not Certificado { Emitido: false } c) return;
        c.EmitidoEm = DateTime.Today;
        _emitidosNoSistema++;
        Atualizar(c);
        Mostrar(c);
    }

    private void Baixar_Click(object sender, RoutedEventArgs e)
    {
        if (_selecionado is not { Emitido: true } c)
        {
            MessageBox.Show(Window.GetWindow(this)!, "Selecione um certificado já emitido para baixar.", "elo",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var dialogo = new SaveFileDialog
        {
            FileName = $"{c.Codigo} - {c.Destinatario}.png",
            Filter = "Imagem PNG (*.png)|*.png",
        };
        if (dialogo.ShowDialog(Window.GetWindow(this)) != true) return;

        // Renderiza a prévia em alta resolução (2x).
        const double escala = 2;
        var tamanho = new Size(CertificadoVisual.ActualWidth, CertificadoVisual.ActualHeight);
        var bitmap = new RenderTargetBitmap((int)(tamanho.Width * escala), (int)(tamanho.Height * escala),
            96 * escala, 96 * escala, PixelFormats.Pbgra32);
        var visual = new DrawingVisual();
        using (var ctx = visual.RenderOpen())
        {
            ctx.DrawRectangle(new VisualBrush(CertificadoVisual), null, new Rect(tamanho));
        }
        bitmap.Render(visual);

        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        using var arquivo = File.Create(dialogo.FileName);
        encoder.Save(arquivo);
    }

    private void Copiar_Click(object sender, RoutedEventArgs e)
    {
        if (_selecionado is not { Emitido: true } c) return;
        Clipboard.SetText(c.LinkValidacao);
        BotaoCopiar.Content = "Link copiado ✓";
    }

    private void Busca_Changed(object sender, TextChangedEventArgs e) => Atualizar();
    private void Filtro_Changed(object sender, SelectionChangedEventArgs e) => Atualizar();
}
