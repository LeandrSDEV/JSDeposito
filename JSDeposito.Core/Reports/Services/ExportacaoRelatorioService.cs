using System.Text;
using ClosedXML.Excel;
using JSDeposito.Core.Reports.DTOs;

namespace JSDeposito.Core.Reports.Services;

public class ExportacaoRelatorioService
{
    // 🔹 CSV
    public byte[] ExportarCsv(IEnumerable<FaturamentoPeriodoDto> dados)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Data;Valor");

        foreach (var item in dados)
        {
            sb.AppendLine($"{item.Data:yyyy-MM-dd};{item.ValorTotal}");
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    // 🔹 EXCEL
    public byte[] ExportarExcel(IEnumerable<FaturamentoPeriodoDto> dados)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Faturamento");

        sheet.Cell(1, 1).Value = "Data";
        sheet.Cell(1, 2).Value = "Valor";

        var linha = 2;
        foreach (var item in dados)
        {
            sheet.Cell(linha, 1).Value = item.Data;
            sheet.Cell(linha, 2).Value = item.ValorTotal;
            linha++;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
