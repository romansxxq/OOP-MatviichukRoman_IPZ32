//Створений клас Book, який наслідує Product
public class Book : Product
{
    //Додані властивості Author, Publisher, Year
    public string Author { get; set; }
    public string Publisher { get; set; }
    public int Year { get; set; }
    //Конструктор класу Book, який приймає параметри для ініціалізації властивостей
    public Book(string name, float price, string author, string pubisher, int year) : base(name, price)
    {
        Author = author;
        Publisher = pubisher;
        Year = year;
    }
    //Деструктор класу Book
    ~Book()
    {
        Console.WriteLine($"Book {Name} is being destroyed");
    }
    //Перевизначений метод DisplayInfo для виведення інформації про книгу
    public override string DisplayInfo()
    {
        return $"Book: {Name}, Price: {Price}, Author: {Author}, Publisher: {Publisher}, Year: {Year}";
    }
}