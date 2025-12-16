using static Orquestra.Utils.Fixtures.Get;
using static Orquestra.Utils.Fixtures.RegexPatterns;

namespace Orquestra.Utils.Fixtures;

public static partial class CommonForBases
{
    public static bool IsNameValid(string? name)
    {
        return !string.IsNullOrWhiteSpace(name) && name.Trim().Length is >= 2 and <= 120;
    }

    public static bool IsDescriptionValid(string? description)
    {
        return description is null || description.Trim().Length <= 255;
    }

    public static bool IsQuantityValid(int quantity)
    {
        return quantity > 0;
    }

    public static bool IsUnitPriceValid(decimal? unitPrice)
    {
        return unitPrice is null or >= 0;
    }

    public static bool IsFullNameValid(string fullName)
    {
        return RegexName().IsMatch(fullName);
    }

    public static void IsDateValid<T>(T input, Func<T, DateTime?> getDate, Action<T, DateTime?>? setDate = null)
    {
        if (input is null)
        {
            return;
        }

        DateTime? executionDate = getDate(input);

        // Workaround;
        if (executionDate?.Date == DateTime.MinValue || executionDate?.Date == new DateTime(2001, 1, 1))
        {
            setDate?.Invoke(input, null);
            return;
        }

        if (executionDate is null)
        {
            return;
        }

        // Normaliza pro horário de Brasília;
        DateTime normalized = ConvertToBrasiliaTime(executionDate.Value);

        setDate?.Invoke(input, normalized);

        DateTime date;

        try
        {
            date = normalized.Date;
        }
        catch
        {
            throw new ArgumentException("A data é inválida.");
        }

        if (date < ConvertToBrasiliaTime(GetDate()).Date)
        {
            throw new ArgumentException("A data não pode ser anterior a hoje.");
        }
    }
}