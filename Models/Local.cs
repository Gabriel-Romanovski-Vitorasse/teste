namespace EloEventos.Models;

public class Local
{
    public required string Id { get; set; }
    public required string Nome { get; set; }
    public required string Endereco { get; set; }
    public string Complemento { get; set; } = "São Paulo, SP · 00000-000";
    public int Capacidade { get; set; }
    public int Vinculos { get; set; }
    public string? ProximoEvento { get; set; }
    public int? LugaresDoEvento { get; set; }

    public string EnderecoCompleto => string.IsNullOrWhiteSpace(Complemento) ? Endereco : $"{Endereco} · {Complemento}";
    public string CapacidadeTexto => $"{Capacidade} pessoas";
    public string VinculosTexto => Vinculos == 1 ? "1 evento" : $"{Vinculos} eventos";
}

public static class LocaisData
{
    public static List<Local> Criar() =>
    [
        new() { Id = "LOC-001", Nome = "Centro de Convenções Aurora", Endereco = "Av. dos Encontros, 120 · Vila Nova", Capacidade = 500, Vinculos = 3,
                ProximoEvento = "Conecta 2026, em 24/11", LugaresDoEvento = 300 },
        new() { Id = "LOC-002", Nome = "Laboratório Horizonte", Endereco = "Rua das Ideias, 85 · Centro", Capacidade = 40, Vinculos = 2,
                ProximoEvento = "Design de experiências de aprendizagem, em 26/11", LugaresDoEvento = 40 },
        new() { Id = "LOC-003", Nome = "Auditório Caminhos", Endereco = "Av. do Saber, 240 · Jardim Sol", Capacidade = 200, Vinculos = 1 },
        new() { Id = "LOC-004", Nome = "Espaço Nova Trilha", Endereco = "Rua do Futuro, 32 · Vila Nova", Capacidade = 150, Vinculos = 1,
                ProximoEvento = "Encontro de inovação e impacto, em 10/12", LugaresDoEvento = 150 },
        new() { Id = "LOC-005", Nome = "Sala Criativa", Endereco = "Rua das Ideias, 85 · Centro", Capacidade = 30, Vinculos = 1,
                ProximoEvento = "IA na prática docente, em 03/12", LugaresDoEvento = 30 },
    ];

    /// <summary>Lista compartilhada (usada também na seleção de local do cadastro de evento).</summary>
    public static readonly List<Local> Todos = Criar();
}
