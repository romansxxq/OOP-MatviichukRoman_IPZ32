using System;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace Lab7v8
{
    //Класс, що імітує роботу з файлами
    public class FileProcessor
    {
        private int _saveUserDataCallCount = 0;
        private readonly int _failTimes;

        //Перші 4 рази буде помилка, а далі успіх
        public FileProcessor(int failTimes = 4)
        {
            _failTimes = failTimes;
        }

        //Збереження даних користувача у файл
        public void SaveUserData(string path, string userData)
        {
            _saveUserDataCallCount++;
            Console.WriteLine($"[FileProcessor] Спроба {_saveUserDataCallCount} зберегти дані...");

            if (_saveUserDataCallCount <= _failTimes)
            {
                //Імітація тимчасової помилки вводу/виводу
                throw new IOException("Імітація помилки вводу/виводу під час збереження даних користувача.");
            }

            //Для демонстрації достатньо виводу в консоль, але можна справді записати:
            //File.WriteAllText(path, userData);

            Console.WriteLine($"[FileProcessor] Дані успішно збережено у файл: {path}");
        }
    }

    //Клас, що імітує роботу з мережею
    public class NetworkClient
    {
        private int _postUserDataCallCount = 0;
        private readonly int _failTimes;

        //За умовою: перші 2 рази HttpRequestException, потім успіх
        public NetworkClient(int failTimes = 2)
        {
            _failTimes = failTimes;
        }

        //Відправка даних користувача на сервер
        public bool PostUserData(string url, string userData)
        {
            _postUserDataCallCount++;
            Console.WriteLine($"[NetworkClient] Спроба {_postUserDataCallCount} відправити дані користувача...");

            if (_postUserDataCallCount <= _failTimes)
            {
                //Імітація тимчасової мережевої помилки
                throw new HttpRequestException("Імітація мережевої помилки під час відправки даних користувача.");
            }

            //Тут могла бути реальна HTTP-відправка через HttpClient
            Console.WriteLine($"[NetworkClient] Дані користувача успішно відправлено на {url}");
            return true;
        }
    }

    //Узагальнений допоміжний клас RetryHelper
    public static class RetryHelper
    {
        //Виконує операцію з повторними спробами (Retry) і експоненційною затримкою.
        public static T ExecuteWithRetry<T>(
            Func<T> operation,
            int retryCount = 3,
            TimeSpan initialDelay = default,
            Func<Exception, bool> shouldRetry = null)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));
            if (retryCount < 0)
                throw new ArgumentOutOfRangeException(nameof(retryCount));

            //Якщо затримку не передали – беремо 500 мс
            var baseDelay = initialDelay == default ? TimeSpan.FromMilliseconds(500) : initialDelay;
            int attempt = 0;

            while (true)
            {
                attempt++;

                try
                {
                    Console.WriteLine($"[RetryHelper] Спроба {attempt}...");

                    T result = operation();

                    Console.WriteLine($"[RetryHelper] Спроба {attempt} завершилась успішно.");
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[RetryHelper] Спроба {attempt} завершилась помилкою: {ex.GetType().Name} - {ex.Message}");

                    // Чи можна робити ще одну спробу?
                    bool canRetry =
                        attempt <= retryCount &&                // не перевищили ліміт невдалих спроб
                        (shouldRetry == null || shouldRetry(ex)); // і тип винятку дозволений для повтору

                    if (!canRetry)
                    {
                        Console.WriteLine("[RetryHelper] Повторні спроби більше не дозволені. Перегенеровуємо виняток.");
                        throw; // віддаємо виняток вище
                    }

                    //Експоненційна затримка: baseDelay * 2^(attempt-1)
                    double factor = Math.Pow(2, attempt - 1);
                    TimeSpan delay = TimeSpan.FromMilliseconds(baseDelay.TotalMilliseconds * factor);

                    Console.WriteLine($"[RetryHelper] Очікування {delay.TotalMilliseconds} мс перед наступною спробою...");
                    Thread.Sleep(delay);
                }
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var fileProcessor = new FileProcessor(failTimes: 4); // 4 рази IOException
            var networkClient = new NetworkClient(failTimes: 2); // 2 рази HttpRequestException

            string path = "userData.txt";
            string url = "https://api.example.com/user";
            string userData = "{ \"id\": 1, \"name\": \"Roman\", \"email\": \"roman134@gmail.com\" }";

            Console.WriteLine("Збереження даних користувача (FileProcessor.SaveUserData)\n");

            try
            {
                //Для void-методу повертаємо просто true (щоб узгодитись з Func<T>)
                RetryHelper.ExecuteWithRetry(
                    operation: () =>
                    {
                        fileProcessor.SaveUserData(path, userData);
                        return true;
                    },
                    retryCount: 4, // допускаємо до 4 невдалих спроб (5 – успішна)
                    initialDelay: TimeSpan.FromMilliseconds(500),
                    //shouldRetry: повторюємо для IOException та HttpRequestException
                    shouldRetry: ex => ex is IOException || ex is HttpRequestException
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[Main] Операція збереження даних користувача остаточно провалилась: {ex.Message}");
            }

            Console.WriteLine("\nВідправка даних користувача (NetworkClient.PostUserData)\n");

            try
            {
                bool result = RetryHelper.ExecuteWithRetry(
                    operation: () => networkClient.PostUserData(url, userData),
                    retryCount: 2, //до 2 невдалих спроб (3 – успішна)
                    initialDelay: TimeSpan.FromMilliseconds(300),
                    shouldRetry: ex => ex is IOException || ex is HttpRequestException
                );

                Console.WriteLine($"\nОстаточний результат PostUserData: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОперація відправки даних користувача остаточно провалилась: {ex.Message}");
            }
            Console.ReadKey();
        }
    }
}
