# Домашнє завдання (Лекція №23)
# Тема: Юніт-тестування: прицнипи, фреймворки, моки.

---

## Коротка доповідь: “Юніт-тестування: теорія та практика”

Юніт-тестування - це автотест перевірки бізнес-логіки (функцій, методів, класів) на коректність. Воно дозволяє швидко виявляти помилки на ранніх етапах розробки, забезпечує впевненість у стабільності змін, сприяє рефакторингу та автоматизації перевірок.

### Переваги юніт-тестування:
- Локалізація помилок: легко знайти причину збою.
- Швидкість виконання: тести працюють швидко, не залежать від зовнішніх систем.
- Автоматизація: тести можна запускати при кожній зміні коду.

### Обмеження юніт-тестування:
- Не перевіряє взаємодію між компонентами.
- Не гарантує правильну роботу всієї системи.
- Може не виявити проблеми інтеграції або конфігурації.

### Порівняння з інтеграційним тестуванням
Інтеграційне тестування перевіряє взаємодію між модулями, роботу з базами даних, API, сторонніми сервісами. Воно складніше, повільніше, але дозволяє знайти помилки, які юніт-тести не виявляють.

---
## Приклад класу для тестування

```
public class Calculator
{
    public int Add(int a, int b) => a + b;
    public int Subtract(int a, int b) => a - b;
    public int Divide(int a, int b)
    {
        if (b == 0) throw new ArgumentException("Division by zero");
        return a / b;
    }
}
```

### Реалізація unit-тестів з xunit:

```
using Xunit;

public class CalculatorTests
{
    [Fact]
    public void Add_ReturnsSum()
    {
        var calc = new Calculator();
        Assert.Equal(5, calc.Add(2, 3));
    }

    [Fact]
    public void Add_ZeroEdgeCase()
    {
        var calc = new Calculator();
        Assert.Equal(2, calc.Add(2, 0));
    }

    [Fact]
    public void Subtract_ReturnsDifference()
    {
        var calc = new Calculator();
        Assert.Equal(1, calc.Subtract(3, 2));
    }

    [Fact]
    public void Subtract_NegativeEdgeCase()
    {
        var calc = new Calculator();
        Assert.Equal(-1, calc.Subtract(2, 3));
    }

    [Fact]
    public void Divide_ReturnsQuotient()
    {
        var calc = new Calculator();
        Assert.Equal(2, calc.Divide(6, 3));
    }

    [Fact]
    public void Divide_ByZeroThrows()
    {
        var calc = new Calculator();
        Assert.Throws<ArgumentException>(() => calc.Divide(6, 0));
    }
}
```

---

## Mock-об’єкти: коли використовувати?

Mock-об’єкти доцільно використовувати, коли методи залежать від зовнішніх ресурсів (бази даних, API, файлової системи), або коли потрібно ізолювати тестований код від складних або повільних залежностей. Якщо методи не мають зовнішніх залежностей (наприклад, прості математичні операції), можна обійтися без моків.