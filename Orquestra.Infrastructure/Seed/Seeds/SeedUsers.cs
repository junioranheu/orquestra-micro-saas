using Microsoft.EntityFrameworkCore;
using Orquestra.Domain.Consts;
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
            await context.Users.AddAsync(new User() { UserId = Guid.NewGuid(), FullName = $"Administrador do {SystemConsts.App.NameApp}", Email = "adm@gmail.com", Password = EncryptPassword("123456"), Role = UserRoleEnum.Administrator, RecoverPasswordQuestion = RecoverPasswordQuestionEnum.MotherName, RecoverPasswordAnswer = "Sandra" });
            await context.Users.AddAsync(new User() { UserId = Guid.NewGuid(), FullName = "Junior Souza", Email = "junioranheu@gmail.com", Password = EncryptPassword("123456"), Role = UserRoleEnum.Common, RecoverPasswordQuestion = RecoverPasswordQuestionEnum.MotherName, RecoverPasswordAnswer = "Sandra" });
        }
        #endregion
    }
}