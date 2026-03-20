# Домашнє завдання (Лекція №24)
# Тема: Принципи рефакторингу, виявлення та усунення code smells.

---
## Code Smells та рефакторинг: практичний аналіз

### 1. Порушення принципу SRP (Single Responsibility Principle)
**Code smell:** В класі `ConfigurationManager` (IndependentWork16v8) всі дії — завантаження, валідація, збереження, сповіщення — реалізовані в одному класі.

**До рефакторингу:**
```csharp
public class ConfigurationManager
{
    public void LoadConfig(string filePath) { ... }
    public bool ValidateConfig() { ... }
    public void SaveConfig(string filePath) { ... }
    public void NotifyChanges() { ... }
    public void ConfigurationService() { ... }
}
```

**Після рефакторингу:** Кожна відповідальність винесена в окремий клас/інтерфейс.
```csharp
IConfigLoader loader = new ConfigLoader();
IConfigValidator validator = new ConfigValidator();
IConfigSaver saver = new ConfigSaver();
IChangeNotifier notifier = new ChangeNotifier();
// ... використання цих сервісів
```
**Техніка:** Виділення класів за відповідальностями.

---

### 2. Дублювання коду
**Code smell:** В класах `Book`, `Food`, `Product` (Lab3v8) дублюються деструктори та методи `DisplayInfo`.

**До рефакторингу:**
```csharp
~Product() { Console.WriteLine($"Product {Name} is being destroyed"); }
~Book() { Console.WriteLine($"Book {Name} is being destroyed"); }
~Food() { Console.WriteLine($"Food {Name} is being destroyed"); }
```

**Після рефакторингу:** Деструктор винесено в базовий клас, метод `DisplayInfo` — використовується поліморфізм.
```csharp
public class Product
{
    ~Product() { Console.WriteLine($"{GetType().Name} {Name} is being destroyed"); }
    public virtual string DisplayInfo() { ... }
}
```
**Техніка:** Узагальнення через базовий клас.

---

### 3. Погане іменування та неявна логіка
**Code smell:** В Lab3v8 змінна `avgPriceBook` використовується для двох різних виводів, а назва не відображає суть.

**До рефакторингу:**
```csharp
var avgPriceBook = products.OfType<Book>().Average(b => b.Price);
Console.WriteLine($"Average price of book: {avgPriceBook} UAH");
Console.WriteLine($"Average price of food: {avgPriceFood} UAH");
```

**Після рефакторингу:**
```csharp
var averageBookPrice = products.OfType<Book>().Average(b => b.Price);
var averageFoodPrice = products.OfType<Food>().Average(f => f.Price);
Console.WriteLine($"Average price of books: {averageBookPrice} UAH");
Console.WriteLine($"Average price of food: {averageFoodPrice} UAH");
```
**Техніка:** Покращення іменування змінних.

---

### Чому рефакторинг без тестів ризикований?

Якщо змінити логіку, наприклад, розділити клас `ConfigurationManager` на декілька сервісів, можна ненавмисно порушити взаємодію між ними. Без юніт-тестів неможливо швидко перевірити, чи працює новий код так само, як старий. Тести гарантують, що рефакторинг не зламав функціонал.

**Приклад:** Якщо після рефакторингу SRP забути викликати `NotifyChanges()`, система не повідомить про зміни, і це буде непомітно без тесту, який перевіряє цю поведінку.