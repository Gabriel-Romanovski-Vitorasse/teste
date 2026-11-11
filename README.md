# elo · Sistema de Gestão de Eventos (WPF / .NET 8)

Implementação em WPF das 9 telas do arquivo Figma **Sistema de Gestão de Eventos** (página *Home*).

## Como rodar

Requisitos: Windows 10/11 e [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (ou Visual Studio 2022 com a carga ".NET desktop").

```powershell
cd EloEventos
dotnet run
```

Na primeira compilação o projeto:
- restaura o pacote **SharpVectors.Wpf**, que exibe os ícones SVG exatamente como foram exportados do Figma;
- **baixa os ícones e as imagens do Figma** para `Assets/` (veja `FigmaAssets.targets`).
  Os links do Figma expiram **7 dias após 10/11/2026**. Depois do primeiro build os arquivos ficam no projeto.
  Se expirarem antes, exporte cada ícone ou imagem do Figma e salve no caminho indicado na mensagem de erro.

## Estrutura

| Pasta | Conteúdo |
|---|---|
| `Themes/Tokens.xaml` | Cores, raios e fonte extraídos do Figma |
| `Themes/Controls.xaml` | Estilos: botões, campos, select, abas, tabela, etiquetas, cards |
| `Controls/` | Componentes reutilizáveis (`StatusPill`, `Notice`, `PersonChip`, `Field`, `StatCard`, `Pager`, `SpacedStackPanel`…) |
| `Views/` | Uma `UserControl` por tela |
| `Models/` | Dados de exemplo usados nas telas |
| `Services/Navigator.cs` | Navegação entre telas |
| `Fonts/` | Inter (licença OFL em `Inter-LICENSE.txt`) |

## Telas

| Tela | Frame no Figma | Interações |
|---|---|---|
| Eventos · Catálogo e gestão | 27:18595 | abas por tipo, busca, filtro de status, ordenação por data, "Detalhes →", "+ Criar evento" |
| Evento · Detalhes e inscrição | 27:18813 | editar evento, inscrever participante (abre nova inscrição) |
| Evento · Cadastro e edição | 27:18950 | troca Conferência/Workshop, resumo atualizado ao digitar, salvar rascunho/publicar |
| Participantes | 27:19136 | busca, filtro por instituição, selecionar e editar, novo cadastro com validação |
| Palestrantes | 27:19361 | busca, filtro por área, edição, emissão de certificado do palestrante |
| Organizadores | 27:19574 | Pessoa Física/Jurídica troca os campos exigidos, eventos vinculados |
| Locais | 27:19797 | busca, filtro por capacidade, edição com validação de capacidade |
| Inscrições | 27:19980 | pagar (vira Confirmada), cancelar (libera vaga), nova inscrição, indicadores ao vivo |
| Certificados | 27:20215 | emitir, prévia, baixar como PNG, copiar link de validação |

## Diferenças conhecidas em relação ao Figma

- **Certificados (27:20215):** o limite de chamadas do plano Starter do Figma foi atingido antes de ler esta tela.
  O layout segue as posições e tamanhos do frame, mas **textos, cores do certificado e o ícone `award` são aproximações**.
  O selo `award` só aparece se `Assets/Icons/award.svg` existir.
- O WPF não tem espaçamento entre letras, então o `letter-spacing: -0.8px` dos títulos de página não foi aplicado.
- Os dados são de exemplo e ficam na memória: fechar o app descarta as alterações.
