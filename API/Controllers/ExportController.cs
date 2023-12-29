using API.Services.Implements;
using API.Services.Interfaces;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("/v1/itsds/export")]
    public class ExportController : BaseController
    {
        private readonly IExportService _exportService;


        public ExportController(IExportService exportService)
        {
            _exportService = exportService;
        }

        [Authorize(Roles = Roles.MANAGER)]
        [HttpGet("/manager")]
        public async Task<IActionResult> GetCustomerTicketDashboard()
        {
            var fileContents = await _exportService.ExportManagerAsync();
            Response.Headers.Add("Content-Disposition", "attachment; filename=dashboard_report.xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            // Trả về file Excel
            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }
}
