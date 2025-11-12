namespace Orquestra.Application.UseCases.ClientsFollowUps.Delete;

public interface IDeleteClientFollowUp
{
    Task Execute(Guid userIdAuth, Guid clientFollowUpId);
}