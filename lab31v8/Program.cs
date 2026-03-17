class Program
{
    static void Main(string[] args)
    {
        IDataSource dataSource = new DataSource();
        IPdfGenerator pdfGenerator = new PdfGenerator();
        ReportService reportService = new ReportService(dataSource, pdfGenerator);
        reportService.CreateReport();
    }
}
public interface IDataSource
{
    string GetData();
}
public interface IPdfGenerator
{
    void GeneratePdf(string data);
}
public class DataSource : IDataSource
{
    public string GetData()
    {
        return "Sample data from DataSource.";
    }
}
public class PdfGenerator : IPdfGenerator
{
    public void GeneratePdf(string data)
    {
        Console.WriteLine($"Generating PDF with data: {data}");
    }
}
public class ReportService
{
    private readonly IDataSource _dataSource;
    private readonly IPdfGenerator _pdfGenerator;

    public ReportService(IDataSource dataSource, IPdfGenerator pdfGenerator)
    {
        _dataSource = dataSource;
        _pdfGenerator = pdfGenerator;
    }

    public void CreateReport()
    {
        string data = _dataSource.GetData();
        _pdfGenerator.GeneratePdf(data);
    }
}