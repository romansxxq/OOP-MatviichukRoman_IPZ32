using Polly;
using Polly.CircuitBreaker;
using Polly.Timeout;
using System;
using System.Threading;

namespace IndependentWork13
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Polly: ");

            // Виклик сценаріїв, щоб вони спрацювали
            FirstScenario();
            SecondScenario();
            ThirdScenario();

            Console.WriteLine("\nDone. Press any key to exit...");
            Console.ReadKey();
        }

        // СЦЕНАРІЙ 1: Нестабільний зовнішній API (Retry)
        static void FirstScenario()
        {
                        Console.WriteLine("Scenario 1:");
            int attemptCounter = 0;
            
            var retryPolicy = Policy.Handle<Exception>()
                .WaitAndRetry(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        // attemptCounter++; // Видалив тут, щоб не дублювати лічильник
                        Console.WriteLine($"[LOG] Retry #{retryCount} after {timeSpan.TotalSeconds} seconds due to: {exception.Message}");
                    });

            try
            {
                retryPolicy.Execute(() =>
                {
                    attemptCounter++;
                    Console.WriteLine($"Attempt {attemptCounter}: Calling unstable API...");
                    
                    // Імітуємо помилку перші 2 рази, на 3-й успіх (оскільки умова < 3)
                    if (attemptCounter < 3)
                    {
                        throw new Exception("Simulated API failure.");
                    }
                    Console.WriteLine("SUCCESS: API call successful.");
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Operation failed after retries: {ex.Message}");
            }
            Console.WriteLine();
        }

        // СЦЕНАРІЙ 2: Перевантажена база даних (Circuit Breaker)
        static void SecondScenario()
        {
            Console.WriteLine("Scenario 2:");
            
            var circuitBreakerPolicy = Policy.Handle<Exception>()
                .CircuitBreaker(
                    exceptionsAllowedBeforeBreaking: 2,
                    durationOfBreak: TimeSpan.FromSeconds(5), // Зменшив до 5 сек, щоб швидше побачити результат
                    onBreak: (exception, breakDelay) =>
                    {
                        Console.WriteLine($"[CB] Circuit BROKEN! Requests blocked for {breakDelay.TotalSeconds} seconds. Reason: {exception.Message}");
                    },
                    onReset: () =>
                    {
                        Console.WriteLine("[CB] Circuit RESET. Operations can proceed.");
                    },
                    onHalfOpen: () => 
                    {
                        Console.WriteLine("[CB] Circuit HALF-OPEN. Testing connection...");
                    });

            for (int i = 1; i <= 6; i++)
            {
                try
                {
                    circuitBreakerPolicy.Execute(() =>
                    {
                        Console.WriteLine($"Request {i}: Accessing database...");
                        if (i <= 2) // Перші 2 запити падають
                        {
                            throw new Exception("Simulated database overload.");
                        }
                        Console.WriteLine("SUCCESS: Database access successful.");
                    });
                }
                catch (BrokenCircuitException)
                {
                    Console.WriteLine("Request rejected: Circuit is OPEN.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Request failed: {ex.Message}");
                }
                Thread.Sleep(1500); // Пауза між запитами
            }
            Console.WriteLine();
        }

        // СЦЕНАРІЙ 3: Довга операція обробки файлу (Timeout)
        static void ThirdScenario()
        {
            Console.WriteLine("--- СЦЕНАРІЙ 3: Довга операція обробки файлу (Timeout) ---");
            
            var timeoutPolicy = Policy.Timeout(TimeSpan.FromSeconds(3), TimeoutStrategy.Pessimistic, onTimeout: (context, timespan, task) =>
            {
                Console.WriteLine($"[Timeout] Operation timed out after {timespan.TotalSeconds} seconds.");
            });

            try
            {
                timeoutPolicy.Execute(() =>
                {
                    Console.WriteLine("Starting long file processing operation...");
                    Thread.Sleep(5000); // Імітація: 5 сек, а ліміт 3 сек
                    Console.WriteLine("File processing completed.");
                });
            }
            catch (TimeoutRejectedException)
            {
                Console.WriteLine("Error: File processing operation was cancelled due to timeout.");
            }
            catch (Exception ex)
            {
                 Console.WriteLine($"Other error: {ex.Message}");
            }
            Console.WriteLine();
        }
    }
}