# Звіт з аналізу SOLID принципів (SRP, OCP) в Open-Source проєкті
## 1. Обраний проєкт
- **Назва:** eShopOnWeb
- **Посилання на GitHub:** https://github.com/dotnet-architecture/eShopOnWeb.git
## 2. Аналіз SRP (Single Responsibility Principle)
### 2.1. Приклади дотримання SRP
#### Клас: `Basket`
- **Відповідальність:** Управління внутрішнім станом кошика та забезпечення цілісності бізнес-логіки додавання товарів.
- **Обґрунтування:** Клас чітко дотримується принципу єдиної відповідальності, оскільки він займається виключно логікою поведінки сутності.

1. **Інкапсуляція даних:** Список `_items` є приватним (private). Ніхто ззовні (наприклад, Контролер) не може просто взяти і очистити кошик або додати туди некоректний товар напряму. Вони змушені використовувати методи класу Basket.

2. **Ізольована логіка:** Метод AddItem містить логіку перевірки: "Якщо товар вже є — збільши кількість, якщо немає — додай новий". Це бізнес-правило, і воно живе саме тут, а не розмазане по сервісах.

3. **Відсутність зайвого:** Клас не знає про базу даних (SQL), не знає про інтерфейс користувача (HTML) і не відправляє емейли.
```csharp
public class Basket : BaseEntity, IAggregateRoot
{
    // SRP: Стан захищений. Ззовні цей список змінити не можна, тільки читати.
    private readonly List<BasketItem> _items = new List<BasketItem>();
    public IReadOnlyCollection<BasketItem> Items => _items.AsReadOnly();

    public void AddItem(int catalogItemId, decimal unitPrice, int quantity = 1)
    {
        // SRP: Вся логіка "як правильно додати товар" знаходиться ВНУТРІШНІ.
        // Якщо правило зміниться (наприклад, макс. 10 товарів),
        // ми змінимо лише цей метод.
        if (!Items.Any(i => i.CatalogItemId == catalogItemId))
        {
            _items.Add(new BasketItem(catalogItemId, quantity, unitPrice));
            return;
        }
        
        // Логіка оновлення існуючого товару теж тут
        var existingItem = Items.First(i => i.CatalogItemId == catalogItemId);
        existingItem.AddQuantity(quantity);
    }
}
``` 
#### Клас: `OrderService`
```csharp
public async Task CreateOrderAsync(int basketId, Address shippingAddress)
{
    // SRP: Отримання даних делеговано репозиторію
    var basket = await _basketRepository.FirstOrDefaultAsync(basketSpec);

    // SRP: Валідація делегована класу Guard (бібліотека Ardalis.GuardClauses)
    Guard.Against.Null(basket, nameof(basket));
    Guard.Against.EmptyBasketOnCheckout(basket.Items);

    // SRP: Логіка формування URL делегована _uriComposer
    // ... _uriComposer.ComposePicUri(...)
    
    // SRP: Створення сутності делеговано конструктору Order
    var order = new Order(basket.BuyerId, shippingAddress, items);

    await _orderRepository.AddAsync(order);
}
```
- **Відповідальність:** Оркестрація (координація) процесу створення замовлення з існуючого кошика.

- **Обґрунтування:** Цей клас є чудовим прикладом того, як сервіс не намагається робити все сам, а делегує роботу іншим спеціалізованим класам. Він діє як "диригент":

1. **Делегування валідації:** Замість того, щоб писати if (basket == null) throw ..., він використовує Guard.Against, делегуючи перевірку окремому механізму.

2. **Делегування доступу до даних:** Він не пише SQL-запити. Він просить _basketRepository та _itemRepository дати дані.

3. **Делегування бізнес-логіки: **Він не розраховує структуру замовлення вручну, а створює сутність new Order(...), яка сама знає, як себе побудувати.

4. **Делегування роботи з шляхами:** Формування URL картинки віддано _uriComposer.

Якщо зміниться логіка збереження в БД — ми змінимо Репозиторій. Якщо зміниться логіка валідації — ми змінимо Guard. OrderService змінюється лише тоді, коли змінюється сам процес перетворення кошика в замовлення.

### 2.2. Приклади порушення SRP
#### Клас: `Program.cs`
- Множинні відповідальності: Файл виконує роль "Божественного об'єкта" (God Object) конфігурації. Він одночасно займається:

1. Налаштуванням інфраструктури: Підключення до SQL Server та Azure KeyVault.

2. Конфігурацією безпеки: Налаштування Identity, Cookies та прав доступу.

3. Реєстрацією UI технологій: Підключення і MVC, і Razor Pages, і Blazor.

4. Виконанням логіки (Seeding): Імперативний виклик методів наповнення бази даних даними (SeedAsync) безпосередньо у процедурі старту.
- Проблеми:
1. Крихкість змін: Цей файл доводиться змінювати при будь-якій зміні в інфраструктурі (зміна БД, логування, додавання нового сервісу).

2. Merge Conflicts: Оскільки всі налаштування в одному місці, це "гаряча точка" для конфліктів версій при роботі в команді.

3. Змішування контекстів: Блок коду з try-catch для наповнення БД (Seeding) є виконанням бізнес-операції, яка "забруднює" чистий код конфігурації запуску.

```csharp
var builder = WebApplication.CreateBuilder(args);

// Відповідальність 1: Налаштування інфраструктури БД
builder.Services.AddDbContext<CatalogContext>(c => ... );

// Відповідальність 2: Налаштування UI (Blazor + MVC)
builder.Services.AddServerSideBlazor();
builder.Services.AddControllersWithViews();

// Відповідальність 3: Налаштування безпеки
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()...

var app = builder.Build();

// ПОРУШЕННЯ: Логіка виконання (Seeding) вшита в конфігурацію
using (var scope = app.Services.CreateScope())
{
    try
    {
        // Цей блок коду виконує роботу з даними, а не просто налаштовує додаток
        var catalogContext = scopedProvider.GetRequiredService<CatalogContext>();
        await CatalogContextSeed.SeedAsync(catalogContext, app.Logger);
    }
    catch (Exception ex) { ... }
}

// Відповідальність 4: HTTP Pipeline
app.UseStaticFiles();
app.UseAuthentication();
app.Run();

```

## 3. Аналіз OCP (Open/Closed Principle)
### 3.1. Приклади дотримання OCP
#### Сценарій/Модуль: Фільтрація товарів у каталозі (CatalogFilterSpecification)
- **Механізм розширення:** Патерн "Специфікація" (Specification Pattern) та абстрактний клас Specification<T>.
- **Обґрунтування:** Архітектура доступу до даних закрита для змін (Closed): нам не потрібно змінювати код класу EfRepository або додавати нові методи типу GetItemsByBrand кожного разу, коли змінюються вимоги до пошуку. Водночас вона відкрита для розширення (Open): ми можемо додати нову логіку фільтрації (наприклад, за ціною або наявністю на складі), просто створивши новий клас специфікації, який успадковується від Specification<T>.

```csharp
public class CatalogFilterSpecification : Specification<CatalogItem>
{
    // Щоб змінити логіку фільтрації, ми не чіпаємо Репозиторій.
    // Ми створюємо або змінюємо лише цей конкретний клас-специфікацію.
    public CatalogFilterSpecification(int? brandId, int? typeId)
    {
        Query.Where(i => (!brandId.HasValue || i.CatalogBrandId == brandId) &&
            (!typeId.HasValue || i.CatalogTypeId == typeId));
    }
}
```
### Сценарій/Модуль: Відправка сповіщень (IEmailSender)
- Механізм розширення: Використання інтерфейсу (interface) для інверсії залежностей.

- Обґрунтування: Бізнес-логіка (наприклад, сервіс замовлень) залежить лише від абстракції IEmailSender.

- Closed (Закрито): Код, який викликає метод SendEmailAsync, не потрібно змінювати, якщо ми вирішимо змінити поштового провайдера.
- Open (Відкрито): Ми можемо легко розширити систему, додавши нову реалізацію, наприклад SendGridEmailSender або SmtpEmailSender, просто створивши новий клас і зареєструвавши його в Program.cs.
```csharp
public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string message);
}
```
### 3.2. Приклади порушення OCP
Сценарій/Модуль: Наповнення бази початковими даними (CatalogContextSeed.cs)
- Проблема: Клас містить жорстко закодований (hardcoded) список брендів та типів товарів у приватному методі GetPreconfiguredCatalogBrands(). Порушення полягає в тому, що клас не закритий для модифікації. Якщо бізнес вимагає додати новий бренд (наприклад, "Samsung"), розробник змушений відкривати C# файл, змінювати вихідний код і перекомпілювати проєкт.
- Наслідки: 
1. Неможливість розширення без втручання в код: Ми не можемо просто "підкласти" файл з новими брендами (наприклад, brands.json), щоб система їх підхопила.
2. Ризик помилок: Втручання в робочий код (C#) заради зміни даних збільшує ризик випадково зламати синтаксис або логіку збереження.
3. Повторне розгортання: Зміна назви бренду вимагає повного циклу Deploy, хоча це мала б бути зміна конфігурації.
```csharp
// src/Infrastructure/Data/CatalogContextSeed.cs

public class CatalogContextSeed
{
    // ... логіка збереження ...

    // ПОРУШЕННЯ OCP:
    // Цей метод мав би бути замінений на завантаження із зовнішнього джерела (JSON/XML).
    // Зараз, щоб додати новий бренд, ми ЗМІНЮЄМО код цього класу.
    static IEnumerable<CatalogBrand> GetPreconfiguredCatalogBrands()
    {
        return new List<CatalogBrand>
        {
            new CatalogBrand("Azure"),
            new CatalogBrand(".NET"),
            new CatalogBrand("Visual Studio"),
            new CatalogBrand("SQL Server"),
            new CatalogBrand("Other") // <--- Хочеш додати "Samsung"? Змінюй код тут.
        };
    }
}
```
## 4. **Загальні висновки**
Проаналізувавши архітектуру проєкту eShopOnWeb, можна зробити висновок, що він демонструє високий рівень дотримання принципів SOLID, зокрема SRP та OCP, хоча і містить певні прагматичні відхилення.

### Щодо Single Responsibility Principle (SRP):

- Сильні сторони: Проєкт чітко розділений на шари (ApplicationCore, Infrastructure, Web), де кожен клас має визначену зону відповідальності. Використання Доменних Сервісів (як OrderService) та бібліотеки GuardClauses дозволяє уникнути перевантаження класів зайвою логікою валідації чи низькорівневими операціями.

- **Компроміси:** Знайдені порушення (зокрема в Program.cs та CatalogContextSeed) є типовими для .NET-екосистеми. Вони зумовлені необхідністю мати єдину точку входу для конфігурації додатка та зручністю розгортання демо-даних. У реальному enterprise-проєкті ці порушення варто було б усунути шляхом винесення конфігурації в окремі модулі, а даних — у зовнішні файли.

### Щодо Open/Closed Principle (OCP):

- **Сильні сторони:** Це найсильніша сторона архітектури eShopOnWeb. Використання патерну Specification дозволяє безмежно розширювати можливості запитів до бази даних без модифікації коду Репозиторіїв. Активне використання Dependency Injection (на прикладі IUriComposer та IEmailSender) робить систему гнучкою до заміни інфраструктурних компонентів.

- **Вплив:** Такий підхід значно знижує ризик внесення багів (регресій) у вже протестований код при додаванні нових функцій.