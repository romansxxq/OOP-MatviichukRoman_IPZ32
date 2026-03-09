using System;
using Xunit;
using Lab30v8;

namespace Lab30v8.Tests;

public class BankAccountTests
{
    // Costructor Tests
    [Fact]
    public void Constructor_WithValidData_CreatesAccountSuccessfully()
    {
        // Arrange & Act
        BankAccount account = new BankAccount("Alice", 1000m);

        // Assert
        Assert.Equal("Alice", account.Owner);
        Assert.Equal(1000m, account.Balance);
    }

    [Fact]
    public void Constructor_WithZeroBalance_CreatesAccountWithZero()
    {
        // Arrange & Act
        BankAccount account = new BankAccount("Bob", 0m);

        // Assert
        Assert.Equal("Bob", account.Owner);
        Assert.Equal(0m, account.Balance);
    }

    // Deposit Tests
    [Fact]
    public void Deposit_WithPositiveAmount_IncreasesBalance()
    {
        // Arrange
        BankAccount account = new BankAccount("Charlie", 500m);

        // Act
        account.Deposit(200m);

        // Assert
        Assert.Equal(700m, account.Balance);
    }

    [Theory]
    [InlineData(100)]
    [InlineData(1)]
    [InlineData(9999)]
    public void Deposit_WithVariousPositiveAmounts_CorrectlyCalculatesBalance(int depositAmount)
    {
        // Arrange
        BankAccount account = new BankAccount("David", 1000m);
        decimal expectedBalance = 1000m + depositAmount;

        // Act
        account.Deposit(depositAmount);

        // Assert
        Assert.Equal(expectedBalance, account.Balance);
    }

    [Fact]
    public void Deposit_WithNegativeAmount_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        BankAccount account = new BankAccount("Eve", 500m);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => account.Deposit(-100m));
    }

    [Fact]
    public void Deposit_WithZeroAmount_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        BankAccount account = new BankAccount("Frank", 500m);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => account.Deposit(0m));
    }

    // Withdraw Tests
    [Fact]
    public void Withdraw_WithValidAmount_DecreasesBalance()
    {
        // Arrange
        BankAccount account = new BankAccount("Grace", 1000m);

        // Act
        account.Withdraw(300m);

        // Assert
        Assert.Equal(700m, account.Balance);
    }

    [Fact]
    public void Withdraw_WithExactBalance_ReducesBalanceToZero()
    {
        // Arrange
        BankAccount account = new BankAccount("Henry", 500m);

        // Act
        account.Withdraw(500m);

        // Assert
        Assert.Equal(0m, account.Balance);
    }

    [Theory]
    [InlineData(100, 900)]
    [InlineData(500, 500)]
    [InlineData(1, 999)]
    [InlineData(999, 1)]
    public void Withdraw_WithVariousValidAmounts_CorrectlyCalculatesBalance(int withdrawAmount, int expectedBalance)
    {
        // Arrange
        BankAccount account = new BankAccount("Ivy", 1000m);

        // Act
        account.Withdraw(withdrawAmount);

        // Assert
        Assert.Equal(expectedBalance, account.Balance);
    }

    [Fact]
    public void Withdraw_WithNegativeAmount_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        BankAccount account = new BankAccount("Jack", 500m);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => account.Withdraw(-100m));
    }

    [Fact]
    public void Withdraw_WithZeroAmount_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        BankAccount account = new BankAccount("Karen", 500m);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => account.Withdraw(0m));
    }

    [Fact]
    public void Withdraw_WithAmountGreaterThanBalance_ThrowsInvalidOperationException()
    {
        // Arrange
        BankAccount account = new BankAccount("Leo", 300m);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => account.Withdraw(500m));
    }

    [Fact]
    public void Withdraw_WithAmountGreaterThanBalance_DoesNotChangeBalance()
    {
        // Arrange
        BankAccount account = new BankAccount("Mia", 300m);

        // Act & Assert
        try
        {
            account.Withdraw(500m);
        }
        catch (InvalidOperationException) { }

        // Assert
        Assert.Equal(300m, account.Balance);
    }

    //Transfer tests
    [Fact]
    public void Transfer_WithValidAmount_TransfersMoneyBetweenAccounts()
    {
        // Arrange
        BankAccount account1 = new BankAccount("Noah", 1000m);
        BankAccount account2 = new BankAccount("Olivia", 500m);

        // Act
        account1.Transfer(account2, 300m);

        // Assert
        Assert.Equal(700m, account1.Balance);
        Assert.Equal(800m, account2.Balance);
    }

    [Fact]
    public void Transfer_WithExactBalance_TransfersAllMoney()
    {
        // Arrange
        BankAccount account1 = new BankAccount("Peter", 500m);
        BankAccount account2 = new BankAccount("Quinn", 200m);

        // Act
        account1.Transfer(account2, 500m);

        // Assert
        Assert.Equal(0m, account1.Balance);
        Assert.Equal(700m, account2.Balance);
    }

    [Theory]
    [InlineData(100, 900, 600)]
    [InlineData(250, 750, 750)]
    [InlineData(1, 999, 501)]
    public void Transfer_WithVariousValidAmounts_CorrectlyTransfersMoneyBetweenAccounts(
        int transferAmount, int expectedSenderBalance, int expectedReceiverBalance)
    {
        // Arrange
        BankAccount sender = new BankAccount("Rachel", 1000m);
        BankAccount receiver = new BankAccount("Steve", 500m);

        // Act
        sender.Transfer(receiver, transferAmount);

        // Assert
        Assert.Equal(expectedSenderBalance, sender.Balance);
        Assert.Equal(expectedReceiverBalance, receiver.Balance);
    }

    [Fact]
    public void Transfer_WithNullTargetAccount_ThrowsArgumentNullException()
    {
        // Arrange
        BankAccount account = new BankAccount("Tracy", 500m);

        // Act & Assert
#pragma warning disable CS8625
        Assert.Throws<ArgumentNullException>(() => account.Transfer(null, 100m));
#pragma warning restore CS8625
    }

    [Fact]
    public void Transfer_WithNegativeAmount_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        BankAccount account1 = new BankAccount("Uma", 500m);
        BankAccount account2 = new BankAccount("Victor", 200m);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => account1.Transfer(account2, -100m));
    }

    [Fact]
    public void Transfer_WithZeroAmount_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        BankAccount account1 = new BankAccount("Wendy", 500m);
        BankAccount account2 = new BankAccount("Xavier", 200m);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => account1.Transfer(account2, 0m));
    }

    [Fact]
    public void Transfer_WithAmountGreaterThanBalance_ThrowsInvalidOperationException()
    {
        // Arrange
        BankAccount account1 = new BankAccount("Yara", 300m);
        BankAccount account2 = new BankAccount("Zoe", 200m);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => account1.Transfer(account2, 500m));
    }

    [Fact]
    public void Transfer_WithAmountGreaterThanBalance_DoesNotChangeBalances()
    {
        // Arrange
        BankAccount account1 = new BankAccount("Adam", 300m);
        BankAccount account2 = new BankAccount("Beth", 200m);

        // Act & Assert
        try
        {
            account1.Transfer(account2, 500m);
        }
        catch (InvalidOperationException) { }

        // Assert
        Assert.Equal(300m, account1.Balance);
        Assert.Equal(200m, account2.Balance);
    }

    // ===== COMPLEX SCENARIO TESTS =====
    [Fact]
    public void MultipleOperations_DepositWithdrawAndTransfer_CalculatesCorrectly()
    {
        // Arrange
        BankAccount account1 = new BankAccount("Alice", 1000m);
        BankAccount account2 = new BankAccount("Bob", 500m);

        // Act
        account1.Deposit(200m);
        account1.Withdraw(150m);
        account1.Transfer(account2, 300m);

        // Assert
        Assert.Equal(750m, account1.Balance);
        Assert.Equal(800m, account2.Balance);
    }

    [Fact]
    public void TransferBetweenMultipleAccounts_ChainedTransfers_CalculatedCorrectly()
    {
        // Arrange
        BankAccount account1 = new BankAccount("Person1", 1000m);
        BankAccount account2 = new BankAccount("Person2", 0m);
        BankAccount account3 = new BankAccount("Person3", 0m);

        // Act
        account1.Transfer(account2, 500m);
        account2.Transfer(account3, 200m);
        account1.Transfer(account3, 100m);

        // Assert
        Assert.Equal(400m, account1.Balance);
        Assert.Equal(300m, account2.Balance);
        Assert.Equal(300m, account3.Balance);
    }

    [Fact]
    public void Account_WithLargeAmounts_OperatesCorrectly()
    {
        // Arrange
        BankAccount account = new BankAccount("Millionaire", 1000000m);

        // Act
        account.Deposit(500000m);
        account.Withdraw(200000m);

        // Assert
        Assert.Equal(1300000m, account.Balance);
    }

    [Fact]
    public void Account_WithSmallDecimals_OperatesCorrectly()
    {
        // Arrange
        BankAccount account = new BankAccount("PennyPincher", 10.50m);

        // Act
        account.Deposit(5.25m);
        account.Withdraw(0.99m);

        // Assert
        Assert.Equal(14.76m, account.Balance);
    }
}
