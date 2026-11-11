namespace EloEventos.Models;

public class Palestrante
{
    public required string Id { get; set; }
    public required string Nome { get; set; }
    public required string Email { get; set; }
    public string Cpf { get; set; } = "000.000.000-00";
    public required string Especialidade { get; set; }
    public string Biografia { get; set; } = "";
    public int Eventos { get; set; }
    /// <summary>Evento concluído pelo qual pode receber certificado (ou null).</summary>
    public string? EventoConcluido { get; set; }
    public string? DataPalestra { get; set; }
    public string? EventoVinculado { get; set; }

    public string Iniciais => Participante.Iniciais_(Nome);
    public string PrimeiroNome => Nome.Split(' ')[0];
}

public static class PalestrantesData
{
    public static List<Palestrante> Criar() =>
    [
        new() { Id = "PAL-001", Nome = "Marina Alves", Email = "marina.a@example.com", Especialidade = "Inovação educacional", Eventos = 3,
                Biografia = "Pesquisadora em educação e design de aprendizagem. Há 12 anos conecta inovação, formação docente e práticas centradas nas pessoas.",
                EventoConcluido = "Educação que transforma", DataPalestra = "15/10/2026", EventoVinculado = "Conecta 2026" },
        new() { Id = "PAL-002", Nome = "Rafael Nunes", Email = "rafael.n@example.com", Especialidade = "Tecnologia e aprendizagem", Eventos = 2,
                Biografia = "Conecta tecnologia, criatividade e formação de educadores.",
                EventoConcluido = "Educação que transforma", DataPalestra = "16/10/2026", EventoVinculado = "Conecta 2026" },
        new() { Id = "PAL-003", Nome = "Juliana Torres", Email = "juliana.t@example.com", Especialidade = "Design de aprendizagem", Eventos = 2 },
        new() { Id = "PAL-004", Nome = "Pedro Carvalho", Email = "pedro.c@example.com", Especialidade = "Inteligência artificial", Eventos = 1,
                EventoVinculado = "IA na prática docente" },
        new() { Id = "PAL-005", Nome = "Sofia Ribeiro", Email = "sofia.r@example.com", Especialidade = "Educação inclusiva", Eventos = 2 },
    ];
}
