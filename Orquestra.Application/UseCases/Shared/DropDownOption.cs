namespace Orquestra.Application.UseCases.Shared;

public static class DropDownOption
{
    public sealed class DropdownOptionOutput<T>
    {
        public required T Value { get; set; }
        public required string Label { get; set; }
    }

    /// <summary>
    /// Extrai valores de string a partir de uma propriedade selecionada,
    /// removendo nulos e espaços em branco, eliminando duplicados e
    /// retornando o resultado ordenado alfabeticamente.
    /// </summary>
    /// <typeparam name="T">Tipo dos itens da coleção de origem.</typeparam>
    /// <param name="source">Coleção de origem.</param>
    /// <param name="selector">Função que seleciona a propriedade string a ser filtrada.</param>
    /// <returns>Lista de strings distintas, limpas e ordenadas.</returns>
    public static List<string?> CleanDistinctOrdered<T>(IEnumerable<T> source, Func<T, string?> selector)
    {
        return [.. source.Select(selector).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().OrderBy(x => x)];
    }

    /// <summary>
    /// Converte uma coleção de itens em uma lista de opções de dropdown,
    /// atribuindo o texto via <c>ToString()</c> e um valor numérico incremental
    /// começando em 1.
    /// </summary>
    /// <typeparam name="T">Tipo dos itens da coleção.</typeparam>
    /// <param name="items">Coleção de itens que será convertida.</param>
    /// <returns>
    /// Uma lista de <see cref="DropdownOptionOutput{Int32}"/> contendo
    /// o texto correspondente ao item e um valor incremental.
    /// Retorna <c>null</c> se a coleção de entrada for nula.
    /// </returns>
    public static List<DropdownOptionOutput<int>>? MapToDropdown(List<string?>? items)
    {
        return items?.Select((x, index) => new DropdownOptionOutput<int> { Label = x ?? string.Empty, Value = index + 1 }).ToList();
    }
}