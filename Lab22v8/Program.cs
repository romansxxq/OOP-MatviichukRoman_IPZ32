// Bad LSP
// class CustomList
// {
//     protected List<int> items = new List<int>();
//     public virtual void Add(int item)
//     {
//         items.Add(item);
//     }
//     public void Print()
//     {
//         foreach (var item in items)
//         {
//             Console.Write(item + " ");
//         }
//         Console.WriteLine();
//     }
// }
// class ReadOnlyList : CustomList
// {
//     public override void Add(int item)
//     {
//         throw new InvalidOperationException("Cannot add items to a read-only list.");
//     }
// }
// class Program
// {
//     static void AddItems(CustomList list)
//     {
//         list.Add(10);

//     }
//     static void Main(string[] args)
//     {
//        Console.WriteLine("LSP Violation Example:");
//        CustomList normalist = new CustomList();
//         AddItems(normalist);
//         normalist.Print();
//         CustomList readOnlyList = new ReadOnlyList();
//         try
//         {
//             AddItems(readOnlyList);
//         }
//         catch (InvalidOperationException ex)
//         {
//             Console.WriteLine("Exception: " + ex.Message);
//         }
//     }
// }

// Good LSP
class Program
{
    static void AddItems(IMutiableList list)
    {
        list.Add(10);
    }
    static void Main(string[] args)
    {
        Console.WriteLine("LSP Compliance Example:");
        IMutiableList normalist = new CustomList();
        AddItems(normalist);
        normalist.Print();
        IReadOnlyList readOnlyList = new ReadOnlyList(new List<int> { 1, 2, 3 });
        readOnlyList.Print();
    }
}
interface IReadOnlyList
{
    void Print();
}
interface IMutiableList : IReadOnlyList
{
    void Add(int item);
}
class CustomList : IMutiableList
{
    protected List<int> items = new List<int>();
    public void Add(int item)
    {
        items.Add(item);
    }
    public void Print()
    {
        foreach (var item in items)
        {
            Console.Write(item + " ");
        }
        Console.WriteLine();
    }
}
class ReadOnlyList : IReadOnlyList
{
    protected List<int> items = new List<int>();
    public ReadOnlyList(List<int> initialItems)
    {
        items.AddRange(initialItems);
    }
    public void Print()
    {
        foreach (var item in items)
        {
            Console.Write(item + " ");
        }
        Console.WriteLine();
    }
}