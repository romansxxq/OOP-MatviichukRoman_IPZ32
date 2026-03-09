class Program
{
    static void Main(string[] args)
    {
        BankAccount account1 = new BankAccount("Alice", 1000m);
        BankAccount account2 = new BankAccount("Bob", 500m);

        Console.WriteLine($"Initial Balance: {account1.Owner}: {account1.Balance:C}, {account2.Owner}: {account2.Balance:C}");

        try
        {
            account1.Deposit(200m);
            Console.WriteLine($"After Deposit: {account1.Owner}: {account1.Balance:C}");

            account1.Withdraw(150m);
            Console.WriteLine($"After Withdrawal: {account1.Owner}: {account1.Balance:C}");

            account1.Transfer(account2, 300m);
            Console.WriteLine($"After Transfer: {account1.Owner}: {account1.Balance:C}, {account2.Owner}: {account2.Balance:C}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
public class BankAccount
{
    public string Owner {get;}
    public decimal Balance {get; private set;}
    public BankAccount(string owner, decimal initialBalance)
    {
        Owner = owner;
        Balance = initialBalance;
    }
    public void Deposit(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Deposit amount must be positive.");
        }
        Balance += amount;
    }
    public void Withdraw(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Withdrawal amount must be positive.");
        }
        if (amount > Balance)
        {
            throw new InvalidOperationException("Insufficient funds for withdrawal.");
        }
        Balance -= amount;
    }
    public void Transfer(BankAccount targetAccount, decimal amount)
    {
        if (targetAccount == null)
        {
            throw new ArgumentNullException(nameof(targetAccount), "Target account cannot be null.");
        }
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Transfer amount must be positive.");
        }
        if (amount > Balance)
        {
            throw new InvalidOperationException("Insufficient funds for transfer.");
        }
        Withdraw(amount);
        targetAccount.Deposit(amount);
    }
}