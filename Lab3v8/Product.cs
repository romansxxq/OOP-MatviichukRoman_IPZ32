//Базовий клас Product з властивостями Name та Price
public class Product
{
    public string Name { get; set; }
    public float Price { get; set; }
    //Конструктор класу Product
    public Product(string name, float price)
    {
        Name = name;
        Price = price;
    }
    //Віртуальний метод DisplayInfo для виведення інформації про продукт
    public virtual string DisplayInfo()
    {
        return $"Product: {Name}, Price: {Price}";
    }
}