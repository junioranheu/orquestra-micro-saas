namespace Orquestra.Application.UseCases.Shared;

public sealed class PaginationInput
{
    const int _firstPage = 0;
    const int _amountRegisters = 10;

    public int Index { get; set; } = _firstPage;

    public int Limit { get; set; } = _amountRegisters;

    public bool IsSelectAll { get; set; } = false;
}