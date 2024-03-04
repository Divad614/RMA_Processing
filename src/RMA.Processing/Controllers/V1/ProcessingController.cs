using Microsoft.AspNetCore.Mvc;
using RMA_Processing.Domain.Contracts.Services;

namespace RMA_Processing.Api.Controllers.V1
{
    [ApiController]
    [Route("v1/[controller]")]
    [Produces("application/json")]
    public class ProcessingController : ControllerBase
    {
        private readonly IProcessingService _processingService;

        public ProcessingController(IProcessingService processingService)
        {
             _processingService = processingService;
        }

        [HttpPost]
        [Route("process_order_number")]
        public async Task<ActionResult> ProccessInput(string orderNumber, string? serialNumber)
        {
            var response = await _processingService.ProcessOrderNumberAsync(orderNumber, serialNumber);
            return Ok(response);
        }
    }
}