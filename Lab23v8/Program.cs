//Bad ISP + DIP

// class Program
// {
//     static void Main(string[] args)
//     {
//         var payrollSystem = new PayrollSystem();
//         payrollSystem.ProcessPayroll("EMP001");
//     }
// }
// // Порушення ISP: багато функцій в одному інтерфейсі
// interface IEmployeeService
// {
//     int CalculateSalary(string employeeId);
//     void ExportToPdf(string employeeId, int salary);
//     void SaveToDatabase(string employeeId, int salary);
// }
// class PayrollSystem
// {
//     private SalaryCalculator salaryCalculator = new SalaryCalculator();
//     private PdfExporter pdfExporter = new PdfExporter();
//     private SqlDatabase sqlDatabase = new SqlDatabase();

//     public void ProcessPayroll(string employeeId)
//     {
//         var salary = salaryCalculator.CalculateSalary(employeeId);
//         pdfExporter.ExportToPdf(employeeId, salary);
//         sqlDatabase.SaveToDatabase(employeeId, salary);
//     } 
// }
// class SalaryCalculator : IEmployeeService
// {
//     public int CalculateSalary(string employeeId)
//     {
//         return 25000;
//     }
//     public void ExportToPdf(string employeeId, int salary)
//     {
//         throw new NotImplementedException();
//     }
//     public void SaveToDatabase(string employeeId, int salary)
//     {
//         throw new NotImplementedException();
//     }
// }
// class PdfExporter : IEmployeeService
// {
//     public void ExportToPdf(string employeeId, int salary)
//     {

//         Console.WriteLine($"Wait...Exporting {employeeId} with salary {salary} to PDF.");
//         Thread.Sleep(2000);
//         Console.WriteLine($"Exporting {employeeId} with salary {salary} to PDF.");
//     }
//     public int CalculateSalary(string employeeId)
//     {
//         throw new NotImplementedException();
//     }
//     public void SaveToDatabase(string employeeId, int salary)
//     {
//         throw new NotImplementedException();
//     }
// }
// class SqlDatabase : IEmployeeService
// {
//     public void SaveToDatabase(string employeeId, int salary)
//     {
//         Console.WriteLine($"Wait...Saving {employeeId} with salary {salary} to database.");
//         Thread.Sleep(2000);
//         Console.WriteLine($"Saving {employeeId} with salary {salary} to database.");
//     }
//     public int CalculateSalary(string employeeId)
//     {
//         throw new NotImplementedException();
//     }
//     public void ExportToPdf(string employeeId, int salary)
//     {
//         throw new NotImplementedException();
//     }
// }

//Good ISP + DIP

class Program
{
    static void Main(string[] args)
    {
        ISalaryCalculator salaryCalculator = new SalaryCalculator();
        IPdfExporter pdfExporter = new PdfExporter();
        IDatabaseSaver databaseSaver = new SqlDatabase();

        var payrollSystem = new PayrollSystem(salaryCalculator, pdfExporter, databaseSaver);
        payrollSystem.GeneratePayroll("EMP001");

    }
}
class PayrollSystem
{
    private readonly ISalaryCalculator _salaryCalculator;
    private readonly IPdfExporter _pdfExporter;
    private readonly IDatabaseSaver _databaseSaver;

    public PayrollSystem(
        ISalaryCalculator salaryCalculator,
        IPdfExporter pdfExporter,
        IDatabaseSaver databaseSaver
    )
    {
        _salaryCalculator = salaryCalculator;
        _pdfExporter = pdfExporter;
        _databaseSaver = databaseSaver;
    }
    public void GeneratePayroll(string employeeId)
    {
        var salary = _salaryCalculator.CalculateSalary(employeeId);
        // string report = $"Employee ID: {employeeId}, Salary: {salary}";
        _pdfExporter.ExportToPdf(employeeId, salary);
        _databaseSaver.SaveToDatabase(employeeId, salary);
    }
}
interface ISalaryCalculator
{
    decimal CalculateSalary(string employeeId);
}
interface IPdfExporter
{
    void ExportToPdf(string employeeId, decimal salary);
}
interface IDatabaseSaver
{
    void SaveToDatabase(string employeeId, decimal salary);
}
class SalaryCalculator : ISalaryCalculator
{
    public decimal CalculateSalary(string employeeId)
    {
        return 25000;
    }
}
class PdfExporter : IPdfExporter
{
    public void ExportToPdf(string employeeId, decimal salary)
    {
        Console.WriteLine($"Wait...Exporting {employeeId} with salary {salary} to PDF.");
        Thread.Sleep(2000);
        Console.WriteLine($"Exported {employeeId} with salary {salary} to PDF.\n");
    }
}
class SqlDatabase : IDatabaseSaver
{
    public void SaveToDatabase(string employeeId, decimal salary)
    {
        Console.WriteLine($"Wait...Saving {employeeId} with salary {salary} to database.");
        Thread.Sleep(2000);
        Console.WriteLine($"Saved {employeeId} with salary {salary} to database.\n");
    }
}
