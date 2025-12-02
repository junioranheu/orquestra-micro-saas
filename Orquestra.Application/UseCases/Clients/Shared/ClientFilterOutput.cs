using Orquestra.Application.UseCases.Shared;

namespace Orquestra.Application.UseCases.Clients.Shared;

public sealed class ClientFilterOutput
{
    public List<DropdownOptionOutput<int>>? FullNames { get; set; } = [];
    public List<DropdownOptionOutput<int>>? Emails { get; set; } = [];
}

public sealed class ClientFilterOutputStringify
{
    public List<string?> FullNames { get; set; } = [];
    public List<string?> Emails { get; set; } = [];
}