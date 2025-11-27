using QuestPDF.Infrastructure;

namespace Orquestra.Infrastructure.Services.PDF;

public interface IPDFService
{
    byte[] GeneratePdfFromModel<T>(T model, Action<IContainer, T> buildContent, string titleDocument, bool addSignatureSection, bool showPageCounter, byte[]? logoBytes = null);
}