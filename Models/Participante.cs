namespace EloEventos.Models;

public class Participante
{
    public required string Id { get; set; }
    public required string Nome { get; set; }
    public required string Email { get; set; }
    public required string Cpf { get; set; }
    public required string Instituicao { get; set; }
    public int Inscricoes { get; set; }

    public string Iniciais => Iniciais_(Nome);
    public string InscricoesTexto => Inscricoes == 1 ? "1 evento" : $"{Inscricoes} eventos";

    /// <summary>Primeira letra do primeiro e do último nome.</summary>
    public static string Iniciais_(string nome)
    {
        var partes = nome.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (partes.Length == 0) return "";
        if (partes.Length == 1) return partes[0][..1].ToUpperInvariant();
        return $"{char.ToUpperInvariant(partes[0][0])}{char.ToUpperInvariant(partes[^1][0])}";
    }
}

public static class ParticipantesData
{
    public static List<Participante> Criar() =>
    [
        new() { Id = "PAR-001", Nome = "Ana Beatriz Lima", Email = "ana.lima@example.com", Cpf = "000.000.000-00", Instituicao = "Universidade Horizonte", Inscricoes = 2 },
        new() { Id = "PAR-002", Nome = "Bruno Martins", Email = "bruno.m@example.com", Cpf = "000.000.000-00", Instituicao = "Instituto Caminhos", Inscricoes = 1 },
        new() { Id = "PAR-003", Nome = "Camila Rocha", Email = "camila.r@example.com", Cpf = "000.000.000-00", Instituicao = "Escola Nova Trilha", Inscricoes = 3 },
        new() { Id = "PAR-004", Nome = "Diego Santos", Email = "diego.s@example.com", Cpf = "000.000.000-00", Instituicao = "Universidade Horizonte", Inscricoes = 1 },
        new() { Id = "PAR-005", Nome = "Elisa Moreira", Email = "elisa.m@example.com", Cpf = "000.000.000-00", Instituicao = "Rede Aprender", Inscricoes = 2 },
        new() { Id = "PAR-006", Nome = "Felipe Azevedo", Email = "felipe.a@example.com", Cpf = "000.000.000-00", Instituicao = "Instituto Caminhos", Inscricoes = 1 },
    ];
}
