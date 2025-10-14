using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Schedules.Base;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Schedules.Delete;

public sealed class DeleteSchedule(ScheduleBaseDependencies deps) : ScheduleBase(deps), IDeleteSchedule
{
    private readonly Context _context = deps.Context;

    public async Task Execute(Guid userIdAuth, Guid scheduleId)
    {
        Schedule? schedule = await _context.Schedules.
                             // AsNoTracking(). // Propositalmente sem AsNoTracking;
                             Where(x => x.ScheduleId == scheduleId).
                             FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warnings.NotFoundSchedule);

        CompanyUser? user = await _context.CompanyUsers.
                            AsNoTracking().
                            Where(x => 
                               x.CompanyId == schedule.CompanyId && 
                               x.UserId == userIdAuth && 
                               x.Status == true
                            ).FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warnings.NotFoundUser);

        Guid[]? specificMember = schedule.UsersIds;

        bool hasPermission = 
            (specificMember is null || specificMember?.Length == 0) || // O agendamento não foi definido para um membro em específico;
            (user.CompanyUserRole == CompanyUserRoleEnum.Administrator) || // O usuário é um administrador da empresa;
            ((specificMember is not null && specificMember?.Length > 0) && specificMember.Contains(user.UserId)); // O usuário, de fato, tem acesso porque foi adicionado como um membro específico;

        if (!hasPermission)
        {
            throw new UnauthorizedAccessException("Você não tem permissão para excluir este agendamento.");
        }

        schedule.Status = false;

        _context.Update(schedule);
        await _context.SaveChangesAsync();
    }
}