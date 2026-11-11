using EloEventos.Controls;

namespace EloEventos.Models;

public class Inscricao
{
    public required string Id { get; set; }
    public required string ParticipanteId { get; set; }
    public required string Participante { get; set; }
    public required string EventoId { get; set; }
    public required string Evento { get; set; }
    public required DateTime Data { get; set; }
    public decimal Valor { get; set; }
    public string Status { get; set; } = "Pendente";
    public string FormaPagamento { get; set; } = "Pix · confirmação instantânea";

    public string DataTexto => Data.ToString("dd/MM/yyyy");
    public string ValorTexto => Valor == 0 ? "Gratuito" : Valor.ToString("C2", Formato.PtBr);
    public PillKind StatusKind => StatusKinds.Of(Status);
}

public static class Formato
{
    public static readonly System.Globalization.CultureInfo PtBr = new("pt-BR");
}

/// <summary>Opções de evento para inscrição (nome curto usado na tabela).</summary>
public record EventoOpcao(string Id, string NomeCurto, decimal Preco, string Periodo, string Local)
{
    public string Rotulo => $"{NomeCurto} · {Id}";
}

public record ParticipanteOpcao(string Id, string Nome)
{
    public string Rotulo => $"{Nome} · {Id}";
}

public static class InscricoesData
{
    public static readonly List<EventoOpcao> Eventos =
    [
        new("EVT-001", "Conecta 2026", 249m, "24–25/11/2026", "Centro de Convenções Aurora"),
        new("EVT-002", "Design de experiências", 180m, "26/11/2026", "Laboratório Horizonte"),
        new("EVT-003", "IA na prática docente", 120m, "03/12/2026", "Sala Criativa"),
        new("EVT-006", "Facilitação de grupos criativos", 150m, "12/12/2026", "Laboratório Horizonte"),
    ];

    public static readonly List<ParticipanteOpcao> Participantes =
        ParticipantesData.Criar().Select(p => new ParticipanteOpcao(p.Id, p.Nome)).ToList();

    public static List<Inscricao> Criar() =>
    [
        new() { Id = "INS-001", ParticipanteId = "PAR-001", Participante = "Ana Beatriz Lima", EventoId = "EVT-001", Evento = "Conecta 2026",
                Data = new(2026, 11, 5, 10, 32, 0), Valor = 249m, Status = "Pendente" },
        new() { Id = "INS-002", ParticipanteId = "PAR-002", Participante = "Bruno Martins", EventoId = "EVT-001", Evento = "Conecta 2026",
                Data = new(2026, 11, 4, 15, 10, 0), Valor = 249m, Status = "Confirmada" },
        new() { Id = "INS-003", ParticipanteId = "PAR-003", Participante = "Camila Rocha", EventoId = "EVT-002", Evento = "Design de experiências",
                Data = new(2026, 11, 4, 9, 45, 0), Valor = 180m, Status = "Confirmada" },
        new() { Id = "INS-004", ParticipanteId = "PAR-004", Participante = "Diego Santos", EventoId = "EVT-001", Evento = "Conecta 2026",
                Data = new(2026, 11, 3, 18, 2, 0), Valor = 249m, Status = "Cancelada" },
        new() { Id = "INS-005", ParticipanteId = "PAR-005", Participante = "Elisa Moreira", EventoId = "EVT-003", Evento = "IA na prática docente",
                Data = new(2026, 11, 3, 11, 20, 0), Valor = 120m, Status = "Confirmada" },
    ];
}
