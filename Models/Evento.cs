using EloEventos.Controls;

namespace EloEventos.Models;

public record Evento(
    string Id,
    string Titulo,
    string Cidade,
    string Tipo,
    DateTime Inicio,
    string Data,
    string Vagas,
    string Inscricao,
    string Status)
{
    public string Subtitulo => $"{Id} · {Cidade}";

    public PillKind StatusKind => StatusKinds.Of(Status);
}

public static class EventosData
{
    public static readonly IReadOnlyList<Evento> Todos =
    [
        new("EVT-001", "Conecta 2026: futuros da educação", "São Paulo, SP", "Conferência", new(2026, 11, 24), "24–25 nov. 2026", "82 de 300", "R$ 249,00", "Publicado"),
        new("EVT-002", "Design de experiências de aprendizagem", "São Paulo, SP", "Workshop", new(2026, 11, 26), "26 nov. 2026", "12 de 40", "R$ 180,00", "Publicado"),
        new("EVT-003", "IA na prática docente", "São Paulo, SP", "Workshop", new(2026, 12, 3), "03 dez. 2026", "0 de 30", "R$ 120,00", "Esgotado"),
        new("EVT-004", "Encontro de inovação e impacto", "São Paulo, SP", "Conferência", new(2026, 12, 10), "10–11 dez. 2026", "150 de 150", "R$ 320,00", "Rascunho"),
        new("EVT-005", "Educação que transforma", "São Paulo, SP", "Conferência", new(2026, 10, 15), "15–16 out. 2026", "0 de 200", "Gratuito", "Encerrado"),
        new("EVT-006", "Facilitação de grupos criativos", "São Paulo, SP", "Workshop", new(2026, 12, 12), "12 dez. 2026", "24 de 24", "R$ 150,00", "Publicado"),
    ];
}
