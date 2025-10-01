namespace Orquestra.Application.UseCases.Shared;

public sealed class DropdownOptionOutput<T>
{
    public required T Value { get; set; }
    public required string Label { get; set; }
}