namespace Orquestra.Application.UseCases.Clients.Delete;

public interface IDeleteClient
{
    Task Execute(Guid userIdAuth, Guid clientId);
}