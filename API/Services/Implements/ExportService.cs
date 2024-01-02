using API.Services.Interfaces;
using OfficeOpenXml;

namespace API.Services.Implements;

public class ExportService : IExportService
{
    private readonly IDashboardService _dashboardService;

    public ExportService(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<byte[]> ExportManagerAsync()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        var dashboardData = await _dashboardService.GetManagerDashboard();

        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Dashboard Report");

            worksheet.Cells["A1"].Value = "Số Ticket trong ngày";
            worksheet.Cells["B1"].Value = "Số Contract trong ngày";
            worksheet.Cells["C1"].Value = "Số Solution trong ngày";
            
                worksheet.Cells[2, 1].Value = dashboardData.TotalTicketOfDay;
                worksheet.Cells[2, 2].Value = dashboardData.TotalContractOfDay;
                worksheet.Cells[2, 3].Value = dashboardData.TotalSolutionOfDay;
           
            return package.GetAsByteArray();
        }
    }
}

