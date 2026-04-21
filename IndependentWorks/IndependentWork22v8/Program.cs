namespace IndependentWork22v8;

class Program
{
    static void Main()
    {
        IComponent dev1 = new Employee("Alice", "Backend Developer");
        dev1 = new SeniorityDecorator(dev1);

        IComponent dev2 = new Employee("Bob", "Frontend Developer");
        dev2 = new ManagerDecorator(dev2);

        IComponent lead = new Employee("Charlie", "Team Lead");
        lead = new SeniorityDecorator(new ManagerDecorator(lead));

        var backendTeam = new Team("Backend Team");
        backendTeam.AddMember(lead);
        backendTeam.AddMember(dev1);
        backendTeam.AddMember(dev2);

        IComponent qa1 = new Employee("Dave", "QA Engineer");
        IComponent qaLead = new Employee("Eve", "QA Engineer");
        qaLead = new SeniorityDecorator(qaLead);

        var qaTeam = new Team("QA Team");
        qaTeam.AddMember(qaLead);
        qaTeam.AddMember(qa1);

        var project = new Team("Alpha Project");
        project.AddMember(backendTeam);
        project.AddMember(qaTeam);

        IComponent decoratedProject = new ManagerDecorator(project);

        Console.WriteLine("Project Structure:");
        decoratedProject.Display(0);

        Console.ReadKey();
    }
}

// Composite
public interface IComponent
{
    public void Display(int indent);
}

public class Employee : IComponent
{
    public string Name { get; set; }
    public string Position { get; set; }
    
    public Employee(string name, string position)
    {
        Name = name;
        Position = position;
    }
    
    public void Display(int indent)
    {
        Console.WriteLine($"{new string(' ', indent)}Employee: {Name}, Position: {Position}");
    }
}

public class Team : IComponent
{
    public string Name { get; set; }
    private readonly List<IComponent> _members = new List<IComponent>();
    
    public Team(string name)
    {
        Name = name;
    }
    
    public void AddMember(IComponent member) => _members.Add(member);
    public void RemoveMember(IComponent member) => _members.Remove(member);
    
    public void Display(int indent)
    {
        Console.WriteLine($"{new string(' ', indent)}Team: {Name}");
        foreach (var member in _members)
        {
            member.Display(indent + 4);
        }
    }
}

// Decorator 
public abstract class EmployeeDecorator : IComponent
{
    protected IComponent _component;
    
    public EmployeeDecorator(IComponent employee)
    {
        _component = employee;
    }
    
    public virtual void Display(int indent)
    {
        _component.Display(indent);
    }
}

public class ManagerDecorator : EmployeeDecorator
{
    public ManagerDecorator(IComponent employee) : base(employee) { }
    
    public override void Display(int indent)
    {
        Console.Write($"{new string(' ', indent)}[Manager] ");
        base.Display(0);
    }
}

public class SeniorityDecorator : EmployeeDecorator
{
    public SeniorityDecorator(IComponent employee) : base(employee) { }
    
    public override void Display(int indent)
    {
        Console.Write($"{new string(' ', indent)}[Senior] ");
        base.Display(0);
    }
}