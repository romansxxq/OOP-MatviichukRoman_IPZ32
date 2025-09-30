public class Book : Product
{
    public string Author { get; set; }
    public string Publisher { get; set; }
    public int Year { get; set; }

    public Book(string name, float price, string author, string pubisher) : base(name, price)
    {
        Author = author;
        Publisher = pubisher;
    }
    public override string DisplayInfo()
    {
        return $"Book: {Name}, Price: {Price}, Author: {Author}, Publisher: {Publisher}, Year: {Year}";
    }
}