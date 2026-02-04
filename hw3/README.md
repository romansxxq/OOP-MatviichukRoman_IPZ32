# Домашня робота №3
## Тема: Принципи ISP та DIP

## 1. ISP - Interface Segregation Principle

**Ідея така:** клієнт не має залежати від методів, які він не використовує.  
Тобто інтерфейси мають бути **“вузькими”** і **цільовими**, а не “універсальними комбайнами”.

### Приклад інтерфейсу, що порушує ISP

Уявимо, що я написав “мегаінтерфейс” для принтера:
```csharp
public interface IMultiFunctionDevice
{
    void Print(string text);
    void Scan(string path);
    void Fax(string number);
}
```
А тепер є простий принтер, який вміє тільки друкувати. Але через цей інтерфейс він змушений реалізувати все:
```csharp
public class OldPrinter : IMultiFunctionDevice
{
    public void Print(string text) => Console.WriteLine(text);

    public void Scan(string path)
        => throw new NotSupportedException("Цей принтер не вміє сканувати");

    public void Fax(string number)
        => throw new NotSupportedException("Цей принтер не вміє факсувати");
}
```
Проблеми:
- з’являються NotSupportedException (тобто дизайн уже “кричить”, що щось не так)
- код стає крихким: клієнт може випадково викликати Scan() і все впаде
- клас отримує зайві зобов’язання
### Вирішення (ISP-варіант): розділяємо на вузькі інтерфейси
```csharp
public interface IPrinter
{
    void Print(string text);
}

public interface IScanner
{
    void Scan(string path);
}

public interface IFax
{
    void Fax(string number);
}
```
Тепер простий принтер реалізує тільки те, що треба:
```csharp
public class OldPrinter : IPrinter
{
    public void Print(string text) => Console.WriteLine(text);
}
```
А багатофункціональний — комбінує:
```csharp
public class MfdDevice : IPrinter, IScanner, IFax
{
    public void Print(string text) => Console.WriteLine($"Print: {text}");
    public void Scan(string path) => Console.WriteLine($"Scan to: {path}");
    public void Fax(string number) => Console.WriteLine($"Fax to: {number}");
}
```
## 2. DIP - Dependency Inversion Principle
**Суть DIP:** модулі високого рівня (бізнес-логіка) не повинні залежати від модулів низького рівня (конкретних реалізацій).
І ті, і ті повинні залежати від абстракцій.

Звучить сухо, але по факту це означає:
- бізнес-логіка не повинна знати, як саме ми відправляємо email / пишемо в базу / логуємо
- вона має знати тільки “контракт” (інтерфейс)
### Поганий приклад (порушення DIP)
```csharp
public class OrderService
{
    private readonly SmtpEmailSender _sender = new SmtpEmailSender();

    public void CreateOrder()
    {
        // бізнес-логіка...
        _sender.Send("user@mail.com", "Order created!");
    }
}

public class SmtpEmailSender
{
    public void Send(string to, string text)
    {
        // реальна відправка через SMTP
    }
}
```
### Мінуси:
- `OrderService` жорстко прив’язаний до SMTP
- тестування болюче: в тесті реально піде email або треба “костилити”
- заміна реалізації (наприклад, SendGrid) = правки в бізнес-коді
### Хороший приклад (DIP + DI)
Робимо абстракцію та інжектимо залежність:
```csharp
public interface IEmailSender
{
    void Send(string to, string text);
}

public class SmtpEmailSender : IEmailSender
{
    public void Send(string to, string text)
    {
        // реальна відправка
    }
}

public class OrderService
{
    private readonly IEmailSender _sender;

    // Constructor Injection
    public OrderService(IEmailSender sender)
    {
        _sender = sender;
    }

    public void CreateOrder()
    {
        // бізнес-логіка...
        _sender.Send("user@mail.com", "Order created!");
    }
}
```
## 3. Переваги DIP через Dependency Injection
Реальні плюси, які я відчув би навіть на лабораторних:
- Легко міняти реалізації без правок бізнес-логіки
(SMTP -> SendGrid -> FakeSender)
- Тести стають простими: можна підставити фейк/мок
- Код менш зв’язаний (low coupling), легше підтримувати
- Краще масштабування: більше компонентів - без “павутини” залежностей
## 4. Як “вузькі” інтерфейси (ISP) сприяють DI та тестуванню
Коли інтерфейси широкі, DI стає незручним:
- треба інжектити “монстра” з купою методів
- тест-фейки виходять товсті, бо треба реалізувати все підряд
- частина методів взагалі не потрібна конкретному класу

Коли інтерфейс вузький:
- в конструктор інжектиться тільки те, що реально треба класу
(IPrinter замість IMultiFunctionDevice)
- фейк для тесту пишеться за 30 секунд
- тест чіткіший: видно, які саме залежності важливі для поведінки
#### Міні-приклад тесту з фейком (без бібліотек):
```csharp
public class FakeEmailSender : IEmailSender
{
    public List<(string to, string text)> Sent = new();

    public void Send(string to, string text)
        => Sent.Add((to, text));
}

// приклад використання
var fake = new FakeEmailSender();
var service = new OrderService(fake);

service.CreateOrder();

Console.WriteLine(fake.Sent.Count); // очікую 1
```
Тобто DIP дозволив підставити фейк, а ISP зробив так, що фейк не треба “роздувати” зайвими методами.
## Висновок
Принцип ISP допомагає робити інтерфейси маленькими і зрозумілими щоб класи не реалізовували зайві методи і не залежали від того що їм не потрібно  
Принцип DIP вчить залежати від абстракцій а не від конкретних реалізацій завдяки чому код стає більш гнучким і зручним для змін  
Dependency Injection дозволяє легко підставляти різні реалізації залежностей без переписування бізнес логіки  
Разом ISP і DIP роблять код більш чистим простішим для тестування і підтримки а також зменшують кількість помилок при розвитку проєкту