using EloEventos.Controls;

namespace EloEventos.Models;

/// <summary>Evento vinculado com seu status (para listas e etiquetas).</summary>
public record EventoStatus(string Evento, string Status)
{
    public PillKind Kind => StatusKinds.Of(Status);
}

/// <summary>Cor da etiqueta para cada status usado no sistema.</summary>
public static class StatusKinds
{
    public static PillKind Of(string status) => status switch
    {
        "Publicado" or "Confirmada" or "Emitido" => PillKind.Success,
        "Esgotado" or "Pendente" => PillKind.Warning,
        "Cancelada" => PillKind.Danger,
        _ => PillKind.Neutral,
    };
}

public class Organizador
{
    public required string Id { get; set; }
    public required string Nome { get; set; }
    public required string Email { get; set; }
    public string Telefone { get; set; } = "(11) 0000-0000";
    public bool Juridica { get; set; }
    public string Cnpj { get; set; } = "00.000.000/0000-00";
    public string RazaoSocial { get; set; } = "";
    public string Cpf { get; set; } = "000.000.000-00";
    public int Vinculos { get; set; }
    public List<EventoStatus> Eventos { get; set; } = [];

    public string Tipo => Juridica ? "Pessoa jurídica" : "Pessoa física";
    public string VinculosTexto => Vinculos == 1 ? "1 evento" : $"{Vinculos} eventos";
}

public static class OrganizadoresData
{
    public static List<Organizador> Criar() =>
    [
        new() { Id = "ORG-001", Nome = "Conecta Educação", Email = "contato@conecta.example", Juridica = true,
                RazaoSocial = "Conecta Educação e Encontros Ltda.", Vinculos = 3,
                Eventos = [new("Conecta 2026: futuros da educação", "Publicado"),
                           new("Design de experiências de aprendizagem", "Publicado"),
                           new("Educação que transforma", "Encerrado")] },
        new() { Id = "ORG-002", Nome = "Luiza Costa", Email = "luiza.c@example.com", Juridica = false, Vinculos = 1,
                Eventos = [new("Facilitação de grupos criativos", "Publicado")] },
        new() { Id = "ORG-003", Nome = "Instituto Caminhos", Email = "eventos@caminhos.example", Juridica = true,
                RazaoSocial = "Instituto Caminhos de Aprendizagem", Vinculos = 2,
                Eventos = [new("IA na prática docente", "Esgotado"), new("Encontro de inovação e impacto", "Rascunho")] },
        new() { Id = "ORG-004", Nome = "André Melo", Email = "andre.m@example.com", Juridica = false, Vinculos = 1,
                Eventos = [new("Encontro de inovação e impacto", "Rascunho")] },
        new() { Id = "ORG-005", Nome = "Rede Aprender", Email = "ola@aprender.example", Juridica = true,
                RazaoSocial = "Rede Aprender Educação S.A.", Vinculos = 2,
                Eventos = [new("Design de experiências de aprendizagem", "Publicado"), new("Educação que transforma", "Encerrado")] },
    ];
}
