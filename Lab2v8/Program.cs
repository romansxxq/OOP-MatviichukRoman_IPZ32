class RationalNumber {
    public int numerator;
    public int denominator;

    public int Numerator {
        get { return numerator; }
        set { numerator = value; }
    }

    public int Denominator {
        get { return denominator; }
        set { 
            if (value == 0) {
                throw new ArgumentException("Denominator cannot be zero.");
            }
            denominator = value; 
        }
    }

    public RationalNumber(int numerator, int denominator){
        Numerator = numerator;
        Denominator = denominator;
    }

    private void Simplify() {
        int gcd = GCD(Math.Abs(numerator), Math.Abs(denominator));
        numerator /= gcd;
        denominator /= gcd;
    }

    private int GCD(int a, int b) {
        while (b != 0) {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    public int this[int index] {
        get {
            if (index == 0) return numerator;
            else if (index == 1) return denominator;
            else throw new IndexOutOfRangeException("Index must be 0 or 1.");
        }
        set {
            if (index == 0) numerator = value;
            else if (index == 1) {
                if (value == 0) throw new ArgumentException("Denominator cannot be zero.");
                denominator = value;
            }
            else throw new IndexOutOfRangeException("Index must be 0 or 1.");
        }
    }

    public static RationalNumber operator +(RationalNumber r1, RationalNumber r2) {
        int newNumerator = r1.numerator * r2.denominator + r2.numerator * r1.denominator;
        int newDenominator = r1.denominator * r2.denominator;
        RationalNumber result = new RationalNumber(newNumerator, newDenominator);
        result.Simplify();
        return result;
    }
    public static RationalNumber operator -(RationalNumber r1, RationalNumber r2) {
        int newNumerator = r1.numerator * r2.denominator - r2.numerator * r1.denominator;
        int newDenominator = r1.denominator * r2.denominator;
        RationalNumber result = new RationalNumber(newNumerator, newDenominator);
        result.Simplify();
        return result;
    }
    public static RationalNumber operator *(RationalNumber r1, RationalNumber r2) {
        int newNumerator = r1.numerator * r2.numerator;
        int newDenominator = r1.denominator * r2.denominator;
        RationalNumber result = new RationalNumber(newNumerator, newDenominator);
        result.Simplify();
        return result;
    }
    public static RationalNumber operator /(RationalNumber r1, RationalNumber r2) {
        if (r2.numerator == 0) throw new DivideByZeroException("Cannot divide by a rational number with a numerator of zero.");
        int newNumerator = r1.numerator * r2.denominator;
        int newDenominator = r1.denominator * r2.numerator;
        RationalNumber result = new RationalNumber(newNumerator, newDenominator);
        result.Simplify();
        return result;
    }
    public static bool operator ==(RationalNumber r1, RationalNumber r2) {
        return r1.numerator * r2.denominator == r2.numerator * r1.denominator;
    }
    public static bool operator !=(RationalNumber r1, RationalNumber r2) {
        return !(r1 == r2);
    }
    public override string ToString() {
        return $"{numerator}/{denominator}";
    }
    public override bool Equals(object? obj) {
        if (obj is RationalNumber other) {
            return this == other;
        }
        return false;
    }
    public override int GetHashCode() {
        return (numerator, denominator).GetHashCode();
    }
}
class Program {
    static void Main(string[] args) {
        RationalNumber r1 = new RationalNumber(1, 2);
        RationalNumber r2 = new RationalNumber(3, 4);

        RationalNumber sum = r1 + r2;
        RationalNumber difference = r1 - r2;
        RationalNumber product = r1 * r2;
        RationalNumber quotient = r1 / r2;

        Console.WriteLine($"r1: {r1}");
        Console.WriteLine($"r2: {r2}");
        Console.WriteLine($"Sum: {sum}");
        Console.WriteLine($"Difference: {difference}");
        Console.WriteLine($"Product: {product}");
        Console.WriteLine($"Quotient: {quotient}");

        Console.WriteLine($"r1 == r2: {r1 == r2}");
        Console.WriteLine($"r1 != r2: {r1 != r2}");

        Console.WriteLine($"r1[0] (Numerator): {r1[0]}");
        Console.WriteLine($"r1[1] (Denominator): {r1[1]}");

        r1[0] = 5; // Change numerator
        r1[1] = 10; // Change denominator
        Console.WriteLine($"Updated r1: {r1}");
    }
}