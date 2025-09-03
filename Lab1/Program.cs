public class Figure
{

    public double Area { get; private set; }

    public Figure(double area)
    {
        Area = area;
    }
    ~Figure()
    {
        Console.WriteLine("Destructor called");
    }
    public void GetFigure()
    {
        Console.WriteLine($"Area: {Area}");
    }
}

public static class Program
{
    static void Main(string[] args)
    {
        Figure figure = new Figure(25.5);
        figure.GetFigure();
    }
}