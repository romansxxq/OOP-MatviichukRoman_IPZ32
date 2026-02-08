# Лабораторна робота No23. Варіант №8
## Тема: ISP & DIP: рефакторинг і DI через конструктор.
## Мета: Застосувати принципи розділення інтерфейсу (ISP) та інверсії залежностей (DIP) для рефакторингу існуючого коду, а також реалізувати Dependency Injection (DI) через конструктор для зменшення зв’язаності та покращення тестування.

## Завдання:
### Початкова реалізація
### Проблеми:
## Порушення ISP
Було створено один загальний інтерфейс, який містив багато методів. Класи реалізовували методи, які їм не були потрібні, через що використовувався `NotImplementedException`.

Це порушує Interface Segregation Principle (ISP).
## Порушення DIP
Клас `PayrollSystem` створював залежності напряму:

```csharp 
private SalaryCalculator salaryCalculator = new SalaryCalculator();
private PdfExporter pdfExporter = new PdfExporter();
private SqlDatabase sqlDatabase = new SqlDatabase();
```
Через це система була жорстко прив’язана до конкретних реалізацій.

Це порушує принцип Dependency Inversion Principle.
## Реалізація після рефакторингу
## Виправлення ISP

Було створено окремі інтерфейси:

- ISalaryCalculator
- IPdfExporter
- IDatabaseSaver

Кожен інтерфейс відповідає лише за одну відповідальність.
## Виправлення DIP та впровадження DI
Залежності більше не створюються всередині класу `PayrollSystem`.

Вони передаються через конструктор:
```csharp
public PayrollSystem(
        ISalaryCalculator salaryCalculator,
        IPdfExporter pdfExporter,
        IDatabaseSaver databaseSaver
    )
```
Це дозволяє легко змінювати реалізації та тестувати систему.
## Налаштування залежностей у Main:
У методі `Main` виконується конфігурація:
```csharp
    static void Main(string[] args)
    {
        ISalaryCalculator salaryCalculator = new SalaryCalculator();
        IPdfExporter pdfExporter = new PdfExporter();
        IDatabaseSaver databaseSaver = new SqlDatabase();

        var payrollSystem = new PayrollSystem(salaryCalculator, pdfExporter, databaseSaver);
        payrollSystem.GeneratePayroll("EMP001");

    }
```
Таким чином головний клас не залежить від конкретних реалізацій.
## Висновки:
У ході виконання лабораторної роботи я:
- Проаналізував порушення принципів SOLID
- Реалізував розділення інтерфейсів (ISP)
- Застосував інверсію залежностей (DIP)
- Впровадив Dependency Injection через конструктор
- Покращив архітектуру програми

Під час виконання роботи я зрозумів що використання принципів SOLID допомагає створювати масштабовані надійні та легко підтримувані програмні системи