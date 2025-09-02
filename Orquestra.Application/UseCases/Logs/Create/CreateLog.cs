using Microsoft.Extensions.Logging;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Logs.Create;

public sealed class CreateLog(Context context, ILogger<CreateLog> logger) : ICreateLog
{
    private readonly Context _context = context;
    private readonly ILogger<CreateLog> _logger = logger;

    public async Task Execute(Log input)
    {
        try
        {
            _context.ChangeTracker.Clear();
            await _context.AddAsync(input);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Houve uma falha ao registrar um log na base de dados.");
        }
    }
}