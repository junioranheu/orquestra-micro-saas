using Microsoft.EntityFrameworkCore;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Encrypt;

namespace Orquestra.Infrastructure.Seed.Seeds;

public sealed class SeedUsers
{
    public static async Task Seed(Context context)
    {
        #region seed
        if (!await context.Users.AsNoTracking().AnyAsync())
        {
            await context.Users.AddAsync(new User() { UserId = Guid.NewGuid(),  FullName = "Adm", Email = "adm", Password = EncryptPassword("123"), Role = UserRoleEnum.Administrator });
        }
        #endregion
    }
}