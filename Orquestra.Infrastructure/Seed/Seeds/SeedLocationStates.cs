using Microsoft.EntityFrameworkCore;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Infrastructure.Seed.Seeds;

public sealed class SeedLocationStates
{
    public static async Task Seed(Context context)
    {
        #region seed
        if (!await context.LocationStates.AsNoTracking().AnyAsync())
        {
            await context.LocationStates.AddAsync(new LocationState() { LocationStateId = 1, Name = "Acre", Abbreviation = "AC", Status = true });
            await context.LocationStates.AddAsync(new LocationState() { LocationStateId = 2, Name = "Alagoas", Abbreviation = "AL", Status = true });
            await context.LocationStates.AddAsync(new LocationState() { LocationStateId = 3, Name = "Amapá", Abbreviation = "AP", Status = true });
            await context.LocationStates.AddAsync(new LocationState() { LocationStateId = 4, Name = "Amazonas", Abbreviation = "AM", Status = true });
            await context.LocationStates.AddAsync(new LocationState() { LocationStateId = 5, Name = "Bahia", Abbreviation = "BA", Status = true });
            await context.LocationStates.AddAsync(new LocationState() { LocationStateId = 6, Name = "Ceará", Abbreviation = "CE", Status = true });
            await context.LocationStates.AddAsync(new LocationState() { LocationStateId = 7, Name = "Espírito Santo", Abbreviation = "ES", Status = true });
            await context.LocationStates.AddAsync(new LocationState() { LocationStateId = 8, Name = "Goiás", Abbreviation = "GO", Status = true });
            await context.LocationStates.AddAsync(new LocationState() { LocationStateId = 9, Name = "Maranhão", Abbreviation = "MA", Status = true });
            await context.LocationStates.AddAsync(new LocationState() { LocationStateId = 10, Name = "Mato Grosso", Abbreviation = "MT", Status = true });
            await context.LocationStates.AddAsync(new LocationState() { LocationStateId = 11, Name = "Mato Grosso do Sul", Abbreviation = "MS", Status = true });
            await context.LocationStates.AddAsync(new LocationState() { LocationStateId = 12, Name = "Minas Gerais", Abbreviation = "MG", Status = true });
            await context.LocationStates.AddAsync(new LocationState() { LocationStateId = 13, Name = "Pará", Abbreviation = "PA", Status = true });
            await context.LocationStates.AddAsync(new LocationState() { LocationStateId = 14, Name = "Paraíba", Abbreviation = "PB", Status = true });
            await context.LocationStates.AddAsync(new LocationState() { LocationStateId = 15, Name = "Paraná", Abbreviation = "PR", Status = true });
            await context.LocationStates.AddAsync(new LocationState() { LocationStateId = 16, Name = "Pernambuco", Abbreviation = "PE", Status = true });
            await context.LocationStates.AddAsync(new LocationState() { LocationStateId = 17, Name = "Piauí", Abbreviation = "PI", Status = true });
            await context.LocationStates.AddAsync(new LocationState() { LocationStateId = 18, Name = "Rio de Janeiro", Abbreviation = "RJ", Status = true });
            await context.LocationStates.AddAsync(new LocationState() { LocationStateId = 19, Name = "Rio Grande do Norte", Abbreviation = "RN", Status = true });
            await context.LocationStates.AddAsync(new LocationState() { LocationStateId = 20, Name = "Rio Grande do Sul", Abbreviation = "RS", Status = true });
            await context.LocationStates.AddAsync(new LocationState() { LocationStateId = 21, Name = "Rondônia", Abbreviation = "RO", Status = true });
            await context.LocationStates.AddAsync(new LocationState() { LocationStateId = 22, Name = "Roraima", Abbreviation = "RR", Status = true });
            await context.LocationStates.AddAsync(new LocationState() { LocationStateId = 23, Name = "Santa Catarina", Abbreviation = "SC", Status = true });
            await context.LocationStates.AddAsync(new LocationState() { LocationStateId = 24, Name = "São Paulo", Abbreviation = "SP", Status = true });
            await context.LocationStates.AddAsync(new LocationState() { LocationStateId = 25, Name = "Sergipe", Abbreviation = "SE", Status = true });
            await context.LocationStates.AddAsync(new LocationState() { LocationStateId = 26, Name = "Tocantins", Abbreviation = "TO", Status = true });
            await context.LocationStates.AddAsync(new LocationState() { LocationStateId = 27, Name = "Distrito Federal", Abbreviation = "DF", Status = true });
        }
        #endregion
    }
}