using System.Text.Json;
using System.Xml.Serialization;
public class Program
{
    public static void Main()
    {
        var employee = new Employee {Id = 1, first_name = "Pavlo", last_name = "Vakulin", experience = 2, profession = "Software Engineer"};
        //json serialization
        var options = new JsonSerializerOptions {WriteIndented = true};
        string json = JsonSerializer.Serialize(employee, options);

        File.WriteAllText("employee.json", json);
        Console.WriteLine("JSON:");
        Console.WriteLine(json);

        //xml serialization
        XmlSerializer serializer = new XmlSerializer(typeof(Employee));
        using (StreamWriter writer = new StreamWriter("employee.xml"))
        {
            serializer.Serialize(writer, employee);
        }

    }
}
[XmlRoot("Employee")]
public class Employee
{
    [XmlElement("ID")]
    public int Id {get;set;}
    [XmlElement("FirstName")]
    public string? first_name {get;set;}
    [XmlElement("LastName")]
    public string? last_name {get;set;}
    [XmlElement("Experience")]
    public int experience {get;set;}
    [XmlElement("Profession")]
    public string? profession {get;set;}
}