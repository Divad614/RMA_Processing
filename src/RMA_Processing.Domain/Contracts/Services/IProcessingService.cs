namespace RMA_Processing.Domain.Contracts.Services
{
    public interface IProcessingService
    {
        Task<FileStream> ProcessOrderNumberAsync(string orderNumber, string? serialNumber);
    }
}
