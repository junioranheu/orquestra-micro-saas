using Microsoft.AspNetCore.Http;
using Moq;
using Orquestra.Application.UseCases.Companies.Base;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyInvoices.Create;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.CompanyUsers.Invite;
using Orquestra.Application.UseCases.CompanyUsers.UpdateCurrentMain;
using Orquestra.Application.UseCases.Integrations.WhatsApp.Create;
using Orquestra.Application.UseCases.Users.Get;
using Orquestra.Application.UseCases.Verifications.Create;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Messaging.Publishers;
using Orquestra.Infrastructure.Services.Email;
using Orquestra.Infrastructure.Services.Email.Models;
using Orquestra.Infrastructure.Services.Env;
using Orquestra.Infrastructure.Services.Env.Models;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Tests.Companies;

public sealed class CompanyBaseTests
{
    [Fact]
    public async Task Validate_ShouldThrow_WhenNameIsEmpty()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        CompanyBase sut = CreateSut(context, user);

        CompanyInput input = new()
        {
            Name = "",
            Email = "empresa@teste.com",
            Phone = "11999999999",
            CompanyType = CompanyTypeEnum.ClinicaOdontologia,
            ZipCode = "12345000",
            Country = "Brasil",
            PlanType = PlanTypeEnum.Basic
        };

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId, isCreate: true, mustValidateIfNameAlreadyExist: false, mustValidateIfEmailAlreadyExist: false));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenEmailIsInvalid()
    {
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        CompanyBase sut = CreateSut(context, user);

        CompanyInput input = new()
        {
            Name = "Empresa Test",
            Email = "email-invalido",
            Phone = "11999999999",
            CompanyType = CompanyTypeEnum.ClinicaOdontologia,
            ZipCode = "12345000",
            Country = "Brasil",
            PlanType = PlanTypeEnum.Basic
        };

        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId, isCreate: true, mustValidateIfNameAlreadyExist: false, mustValidateIfEmailAlreadyExist: false));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenPhoneIsInvalid()
    {
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        CompanyBase sut = CreateSut(context, user);

        CompanyInput input = new()
        {
            Name = "Empresa Test",
            Email = "empresa@teste.com",
            Phone = "abc",
            CompanyType = CompanyTypeEnum.ClinicaOdontologia,
            ZipCode = "12345000",
            Country = "Brasil",
            PlanType = PlanTypeEnum.Basic
        };

        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId, isCreate: true, mustValidateIfNameAlreadyExist: false, mustValidateIfEmailAlreadyExist: false));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenTypeIsInvalid()
    {
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        CompanyBase sut = CreateSut(context, user);

        CompanyInput input = new()
        {
            Name = "Empresa Test",
            Email = "empresa@teste.com",
            Phone = "11999999999",
            CompanyType = (CompanyTypeEnum)999,
            ZipCode = "12345000",
            Country = "Brasil",
            PlanType = PlanTypeEnum.Basic
        };

        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId, isCreate: true, mustValidateIfNameAlreadyExist: false, mustValidateIfEmailAlreadyExist: false));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenZipCodeIsInvalid()
    {
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        CompanyBase sut = CreateSut(context, user);

        CompanyInput input = new()
        {
            Name = "Empresa Test",
            Email = "empresa@teste.com",
            Phone = "11999999999",
            CompanyType = CompanyTypeEnum.ClinicaOdontologia,
            ZipCode = "abc",
            Country = "Brasil",
            PlanType = PlanTypeEnum.Basic
        };

        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId, isCreate: true, mustValidateIfNameAlreadyExist: false, mustValidateIfEmailAlreadyExist: false));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenCountryIsInvalid()
    {
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        CompanyBase sut = CreateSut(context, user);

        CompanyInput input = new()
        {
            Name = "Empresa Test",
            Email = "empresa@teste.com",
            Phone = "11999999999",
            CompanyType = CompanyTypeEnum.ClinicaOdontologia,
            ZipCode = "12345000",
            Country = "País Inválido",
            PlanType = PlanTypeEnum.Basic
        };

        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId, isCreate: true, mustValidateIfNameAlreadyExist: false, mustValidateIfEmailAlreadyExist: false));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenPlanTypeIsZero()
    {
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        CompanyBase sut = CreateSut(context, user);

        CompanyInput input = new()
        {
            Name = "Empresa Test",
            Email = "empresa@teste.com",
            Phone = "11999999999",
            CompanyType = CompanyTypeEnum.ClinicaOdontologia,
            ZipCode = "12345000",
            Country = "Brasil",
            PlanType = 0
        };

        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId, isCreate: true, mustValidateIfNameAlreadyExist: false, mustValidateIfEmailAlreadyExist: false));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenStatusFalseOnUpdate()
    {
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        CompanyBase sut = CreateSut(context, user);

        CompanyInput input = new()
        {
            CompanyId = Guid.NewGuid(),
            Name = "Empresa Test",
            Email = "empresa@teste.com",
            Phone = "11999999999",
            CompanyType = CompanyTypeEnum.ClinicaOdontologia,
            Status = false,
            ZipCode = "12345000",
            Country = "Brasil",
            PlanType = PlanTypeEnum.Basic
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Validate(input, user.UserId, isCreate: false, mustValidateIfNameAlreadyExist: false, mustValidateIfEmailAlreadyExist: false));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenNameAlreadyExists()
    {
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Guid companyId = Guid.NewGuid();
        Company existing = new()
        {
            CompanyId = companyId,
            Name = "Empresa Existente",
            Email = "existente@teste.com",
            Phone = "11999999999",
            CompanyType = CompanyTypeEnum.ClinicaOdontologia,
            Status = true
        };

        await context.Companies.AddAsync(existing);
        await context.SaveChangesAsync();

        CompanyBase sut = CreateSut(context, user);

        CompanyInput input = new()
        {
            Name = "Empresa Existente",
            Email = "nova@teste.com",
            Phone = "11988888888",
            CompanyType = CompanyTypeEnum.ClinicaOdontologia,
            Status = true,
            ZipCode = "12345000",
            Country = "Brasil",
            PlanType = PlanTypeEnum.Basic
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Validate(input, user.UserId, isCreate: true, mustValidateIfNameAlreadyExist: true, mustValidateIfEmailAlreadyExist: false));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenEmailAlreadyExists()
    {
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Guid companyId = Guid.NewGuid();
        Company existing = new()
        {
            CompanyId = companyId,
            Name = "Empresa Existente",
            Email = "existente@teste.com",
            Phone = "11999999999",
            CompanyType = CompanyTypeEnum.ClinicaOdontologia,
            Status = true
        };

        await context.Companies.AddAsync(existing);
        await context.SaveChangesAsync();

        CompanyBase sut = CreateSut(context, user);

        CompanyInput input = new()
        {
            Name = "Nova Empresa",
            Email = "existente@teste.com",
            Phone = "11988888888",
            CompanyType = CompanyTypeEnum.ClinicaOdontologia,
            Status = true,
            ZipCode = "12345000",
            Country = "Brasil",
            PlanType = PlanTypeEnum.Basic
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Validate(input, user.UserId, isCreate: true, mustValidateIfNameAlreadyExist: false, mustValidateIfEmailAlreadyExist: true));
    }

    [Fact]
    public async Task Validate_ShouldPass_WhenInputIsValid()
    {
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        CompanyBase sut = CreateSut(context, user);

        CompanyInput input = new()
        {
            Name = "Empresa Test",
            Email = "empresa@teste.com",
            Phone = "11999999999",
            CompanyType = CompanyTypeEnum.ClinicaOdontologia,
            Status = true,
            ZipCode = "12345000",
            Country = "Brasil",
            PlanType = PlanTypeEnum.Basic
        };

        await sut.Validate(input, user.UserId, isCreate: true, mustValidateIfNameAlreadyExist: true, mustValidateIfEmailAlreadyExist: true);

        Assert.Equal("Empresa Test", input.Name);
        Assert.Equal("empresa@teste.com", input.Email);
    }

    [Fact]
    public async Task Validate_ShouldPass_ForDifferentPlanTypes()
    {
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        CompanyBase sut = CreateSut(context, user);

        foreach (PlanTypeEnum plan in Enum.GetValues<PlanTypeEnum>())
        {
            if (plan == 0)
            {
                continue;
            }

            CompanyInput input = new()
            {
                Name = "Empresa Test",
                Email = "empresa@teste.com",
                Phone = "11999999999",
                CompanyType = CompanyTypeEnum.ClinicaOdontologia,
                Status = true,
                ZipCode = "12345000",
                Country = "Brasil",
                PlanType = plan
            };

            await sut.Validate(input, user.UserId, isCreate: true, mustValidateIfNameAlreadyExist: false, mustValidateIfEmailAlreadyExist: false);
            Assert.Equal(plan, input.PlanType);
        }
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenLogoFormFileTooLarge()
    {
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        CompanyBase sut = CreateSut(context, user);

        using MemoryStream ms = new(new byte[4 * 1024 * 1024]); // 4MB;
        FormFile fileMock = new(ms, 0, ms.Length, "Logo", "logo.png");

        CompanyInput input = new()
        {
            Name = "Empresa Test",
            Email = "empresa@teste.com",
            Phone = "11999999999",
            CompanyType = CompanyTypeEnum.ClinicaOdontologia,
            Status = true,
            ZipCode = "12345000",
            Country = "Brasil",
            PlanType = PlanTypeEnum.Basic,
            LogoFormFile = fileMock
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Validate(input, user.UserId, isCreate: true, mustValidateIfNameAlreadyExist: false, mustValidateIfEmailAlreadyExist: false));
    }

    [Fact]
    public async Task Validate_ShouldPass_WhenStatusTrueOnUpdate()
    {
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        CompanyBase sut = CreateSut(context, user);

        CompanyInput input = new()
        {
            CompanyId = Guid.NewGuid(),
            Name = "Empresa Test",
            Email = "empresa@teste.com",
            Phone = "11999999999",
            CompanyType = CompanyTypeEnum.ClinicaOdontologia,
            Status = true,
            ZipCode = "12345000",
            Country = "Brasil",
            PlanType = PlanTypeEnum.Basic
        };

        await sut.Validate(input, user.UserId, isCreate: false, mustValidateIfNameAlreadyExist: false, mustValidateIfEmailAlreadyExist: false);

        Assert.True(input.Status);
    }

    [Fact]
    public async Task Validate_ShouldPass_WhenZipCodeOrCountryEmpty()
    {
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        CompanyBase sut = CreateSut(context, user);

        CompanyInput input = new()
        {
            Name = "Empresa Test",
            Email = "empresa@teste.com",
            Phone = "11999999999",
            CompanyType = CompanyTypeEnum.ClinicaOdontologia,
            Status = true,
            ZipCode = "",
            Country = "",
            PlanType = PlanTypeEnum.Basic
        };

        await sut.Validate(input, user.UserId, isCreate: true, mustValidateIfNameAlreadyExist: false, mustValidateIfEmailAlreadyExist: false);

        Assert.Equal(string.Empty, input.ZipCode);
        Assert.Equal(string.Empty, input.Country);
    }

    #region helpers
    private static CompanyBase CreateSut(Context context, User user)
    {
        Mock<IEnvService> envMock = new();
        envMock.Setup(e => e.GetUrls()).Returns(new EnvOutput { UrlBackend = "http://localhost:5000", UrlFrontend = "ttp://localhost:5001" });

        Mock<ICreateVerification> createVerificationMock = new();

        createVerificationMock.Setup(c => c.Execute<Company>(It.IsAny<Guid>(), It.IsAny<VerificationTypeEnum>(), It.IsAny<string?>(), It.IsAny<DateTime?>())).
            ReturnsAsync((Guid entityId, VerificationTypeEnum type, string? reference, DateTime? expirationDate) =>
            new Verification
            {
                VerificationId = Guid.NewGuid(),
                Token = GenerateSafeToken32Bytes(urlSafe: true),
                EntityType = typeof(Company).Name,
                EntityId = entityId,
                VerificationType = type,
                Reference = reference,
                ExpirationDate = expirationDate,
                Used = false
            });

        Mock<IGenericPublisher> genericPublisherMock = new();
        genericPublisherMock.Setup(x => x.Publish(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        GetUser getUser = new(context);
        GetAllCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, Fixture.CreateIHttpContextAccessor(user));
        GetCompany getCompany = new(context, checkIfUserIsLinkedCompanyUser);
        InviteCompanyUser inviteCompanyUser = new(context, envMock.Object, createVerificationMock.Object, checkIfUserIsLinkedCompanyUser, getUser, getCompany, genericPublisherMock.Object);
        UpdateCurrentMainCompanyUser updateCurrentMainCompanyUser = new(context, checkIfUserIsLinkedCompanyUser);

        ICreateCompanyInvoice createCompanyInvoice = new Mock<ICreateCompanyInvoice>().Object;
        ICreateIntegrationWhatsApp createIntegrationWhatsApp = new Mock<ICreateIntegrationWhatsApp>().Object;

        CompanyBase companyBase = new(new CompanyBaseDependencies(
            context,
            envMock.Object,
            createVerificationMock.Object,
            inviteCompanyUser,
            updateCurrentMainCompanyUser,
            getUser,
            checkIfUserIsLinkedCompanyUser,
            createCompanyInvoice,
            createIntegrationWhatsApp,
            genericPublisherMock.Object
        ));

        return companyBase;
    }
    #endregion
}