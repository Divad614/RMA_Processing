using RMA_Processing.Domain.Contracts.Gateway;
using RMA_Processing.Domain.Contracts.Services;
using static RMA_Processing.Domain.Utils.LifetimeAttributes;

namespace RMA_Processing.Domain.Services
{
    [Scoped]
    public class ProcessingService : IProcessingService
    {
        private readonly IFortNoxGaateway _fortNoxGaateway;
        private readonly IZenDeskGateway _zenDeskGateway;

        public ProcessingService()
        {
            
        }
        public async Task<FileStream> ProcessOrderNumberAsync(string orderNumber, string? serialNumber)
        {

        }
    }
}