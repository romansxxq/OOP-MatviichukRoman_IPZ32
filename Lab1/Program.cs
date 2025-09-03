public class Figure
{

    // public double Area { get; private set; }
    private double _area;

    public double Area
    {
        get { return _area; }
        private set { _area = value; }
    }
    public Figure(double area)
    {
        Area = area;
        // Console.WriteLine("Constructor called");
    }
    ~Figure()
    {
        // Console.WriteLine("Destructor called");
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