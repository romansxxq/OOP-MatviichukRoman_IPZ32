# Самостійна робота No16
## Тема: Схема розподілу відповідальностей модуля.
## Мета: Навчитися застосовувати принцип єдиної відповідальності (SRP) для декомпозиції складного модуля на менші, більш сфокусовані класи, а також візуалізувати розподіл відповідальностей за допомогою діаграми класів (UML).

### Завдання:
- Реалізував клас, який порушує SRP `ConfigurationManager`:
```csharp
public class ConfigurationManager
{
    public void LoadConfig(string filePath)
    {
        Console.WriteLine($"Loading configuration...");
        Thread.Sleep(2000);
        Console.WriteLine("Configuration loaded.");
    }
    public bool ValidateConfig()
    {
        Console.WriteLine($"Validating configuration...");
        Thread.Sleep(1000);
        Console.WriteLine("Configuration is valid.");
        return true;
    }
    public void SaveConfig(string filePath)
    {
        Console.WriteLine($"Saving configuration...");
        Thread.Sleep(500);
        Console.WriteLine("Configuration saved.");
    }
    public void NotifyChanges()
    {
        Console.WriteLine($"Notifying about configuration changes...");
        Thread.Sleep(200);
        Console.WriteLine("Notification sent.");
    }
    public void ConfigurationService()
    {
        LoadConfig("config.json");
        if (ValidateConfig())
        {
            SaveConfig("config.json");
            NotifyChanges();
        }
    }
}
class Program
{
    static void Main(string[] args)
    {
        ConfigurationManager configManager = new ConfigurationManager();
        configManager.ConfigurationService();
    }
}
```
Клас `ConfigurationManager` порушував принцип єдиної відповідальності (SRP), оскільки поєднував у собі декілька незалежних функцій: завантаження конфігурації, її валідацію, збереження та сповіщення про зміни.

Такий підхід призводить до появи антипатерну God Object, коли один клас концентрує надмірну кількість логіки та відповідальностей, що ускладнює супровід, тестування та розширення системи.

Для усунення цієї проблеми було виконано декомпозицію функціональності на окремі класи з чітко визначеними відповідальностями та застосовано Dependency Injection.

Для усунення антипатерну God Object та дотримання SRP функціональність було декомпозовано на окремі інтерфейси та класи:

- `IConfigLoader` — завантаження конфігурації
- `IConfigValidator` — валідація конфігурації
- `IConfigSaver` — збереження конфігурації
- `IChangeNotifier` — сповіщення про зміни

Клас `ConfigurationService` координує роботу компонентів, отримуючи їх через конструктор (Dependency Injection).

Код можна глянути [тут](Program.cs)

#### UML-діаграма:
![UML-diagram](image/diagram%20uml.png)
### Висновок:
У результаті рефакторингу було усунуто антипатерн God Object, забезпечено дотримання принципу SRP, а також застосовано Dependency Injection, що підвищило гнучкість, зрозумілість та підтримуваність коду.