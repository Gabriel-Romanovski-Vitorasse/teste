namespace EloEventos.Models;

public class Certificado
{
    public required string Codigo { get; set; }
    public required string Destinatario { get; set; }
    public required string DestinatarioId { get; set; }
    public required string Tipo { get; set; } // Participante | Palestrante
    public required string Evento { get; set; }
    public required string EventoPeriodo { get; set; }
    public string CargaHoraria { get; set; } = "16 horas";
    public DateTime? EmitidoEm { get; set; }

    public bool Emitido => EmitidoEm != null;
    public string SituacaoTexto => Emitido ? "Emitido" : "Pronto para emitir";
    public string EmitidoTexto => EmitidoEm?.ToString("dd/MM/yyyy") ?? "";
    public string LinkValidacao => $"https://elo.example/validar/{Codigo}";

    public string Reconhecimento => Tipo == "Palestrante"
        ? $"compartilhou conhecimento como palestrante em {Evento}, realizado em {EventoPeriodo}, com carga horária de {CargaHoraria}."
        : $"participou de {Evento}, realizado em {EventoPeriodo}, com carga horária de {CargaHoraria}.";
}

public static class CertificadosData
{
    public static List<Certificado> Criar() =>
    [
        new() { Codigo = "ELO-2026-0001", Destinatario = "Ana Beatriz Lima", DestinatarioId = "PAR-001", Tipo = "Participante",
                Evento = "Educação que transforma", EventoPeriodo = "15 e 16 de outubro de 2026", EmitidoEm = new(2026, 10, 17) },
        new() { Codigo = "ELO-2026-0002", Destinatario = "Marina Alves", DestinatarioId = "PAL-001", Tipo = "Palestrante",
                Evento = "Educação que transforma", EventoPeriodo = "15 e 16 de outubro de 2026" },
        new() { Codigo = "ELO-2026-0003", Destinatario = "Rafael Nunes", DestinatarioId = "PAL-002", Tipo = "Palestrante",
                Evento = "Educação que transforma", EventoPeriodo = "15 e 16 de outubro de 2026" },
        new() { Codigo = "ELO-2026-0004", Destinatario = "Camila Rocha", DestinatarioId = "PAR-003", Tipo = "Participante",
                Evento = "Educação que transforma", EventoPeriodo = "15 e 16 de outubro de 2026" },
    ];
}
