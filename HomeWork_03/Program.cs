using System;
using System.Threading;
using System.Threading.Channels;

namespace HomeWork_03
{
    public class Program
    {
        private static int gameNumber;                  // Случайное число.
        private static int userTry;                     // Введенное число пользователя.
        private static int countPlayers;                // Количество игроков.
        private static bool isDraw;                     // Ничья?
        private static bool isFinish;                   // Игра окончена?
        private static bool isPlayer;                   // Ход игрока? Для игры против бота.  
        private static int modeNumber;                  // Выбор режима.

        // Список имен для многопользовательской игры.
        private static string[] names;

        /// <summary>
        /// Точка входу в программу.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Initialize();
        }

        /// <summary>
        /// Запуск стартовых процессов.
        /// </summary>
        private static void Initialize()
        {
            // Распечатываем правила на консоль.
            PrintRules();

            // Выбрать режим игры.
            modeNumber = SelectDifficulty();


            if (modeNumber == 1)
            {
                SetNames(1);            // Устанавливаем имя игрокам.
                StartSettings();                  // Настройка игры 
                StartSoloPlay();                  // Старт игры против бота.
            }
            else
            {
                SetNames(countPlayers);           // Устанавливаем имя игрокам.
                StartSettings();                  // Настройка игры 
                StartMultiPlay();                 // Старт многопользовательской игры.
            }
        }

        /// <summary>
        /// Получить количество игроков.
        /// </summary>
        /// <returns>Количество игроков.</returns>
        private static int GetCountPlayers()
        {
            Console.Write("Введите количество игроков от 2 до 5: ");
            int count = Convert.ToInt32(Console.ReadLine());

            while (count is < 2 or > 5)
            {

                Console.Write("Ошибка! Введите количество игроков от 2 до 5: ");
                count = Convert.ToInt32(Console.ReadLine());
            }

            return count;
        }
        
        /// <summary>
        /// Распечатать правила на консоль.
        /// </summary>
        private static void PrintRules()
        {
            Console.ForegroundColor = ConsoleColor.Green;           // Изменить цвет текста на зеленый.
            Console.WriteLine("\t\t***** Добро пожаловать в игру! *****\n");


            Console.ForegroundColor = ConsoleColor.Yellow;        // Изменить цвет текста на желтый.
            Console.WriteLine("\tПравила игры:");
            Console.WriteLine("\t1) Бот загадывает число с диапазоном, заданным игроком.");
            Console.WriteLine("\t2) Игроки по очереди вводят число от 2 до 7.");
            Console.WriteLine("\t3) Загаданное число после каждого хода вычитается числом, " +
                              "которое ввёл игрок.\n\t\t   И оставшееся значение выводится на экран.");
            Console.WriteLine("\t4) Если после хода игрока, загаданное число равняется нулю," +
                              "\n\t\t   то походивший игрок оказывается победителем.\n");

            // Сбросить цвет текста.
            Console.ResetColor();
        }

        /// <summary>
        /// Выбрать режим игры.
        /// </summary>
        /// <returns>Идентификатор сложности.</returns>
        private static int SelectDifficulty()
        {
            Console.WriteLine("Выберите сложность игры.");
            Console.WriteLine("1) SOLO.");
            Console.WriteLine("2) Multiplayer.");
            Console.Write("Ваш выбор: ");
            int modeNumber = Convert.ToInt32(Console.ReadLine());

            while (modeNumber is < 1 or > 2)       // Пока значение не будет корректным дя выбора.
            {
                // Ввести данные еще раз.
                Console.WriteLine("Введенное значение должно быть 1 или 2.");
                Console.WriteLine("1) SOLO.");
                Console.WriteLine("2) Multiplayer.");
                Console.Write("Ваш выбор: ");
                modeNumber = Convert.ToInt32(Console.ReadLine());
            }

            if (modeNumber == 2)
                countPlayers = GetCountPlayers();

            return modeNumber;
        }

        /// <summary>
        /// Запуск настроек перед началом игры.
        /// </summary>
        private static void StartSettings()
        {
            isDraw = false;                    // Устанавливаем значение "Ничья" на "нет"
            isFinish = false;                  // Устанавливаем значение "Конец игры" на "нет"
            InitializeGameNumber();            // Устанавливаем случайное число.
        }

        /// <summary>
        /// Запуск игры против бота.
        /// </summary>
        private static void StartSoloPlay()
        {
            int botTry;
            Random rnd = new Random();

            while (!isFinish)
            {
                isPlayer = true;        // ход игрока.
                userTry = GetUserNumber(names[0]);

                gameNumber -= userTry;
                Console.WriteLine($"Загаданное число: {gameNumber}.");

                if (CheckWin())
                    break;
                

                isPlayer = false;       // ход бота.

                botTry = gameNumber is < 8 and > 1 ? gameNumber : rnd.Next(1, 7);
                Console.WriteLine($"\nБот ввел число: {botTry}.");

                gameNumber -= botTry;
                Console.WriteLine($"Загаданное число: {gameNumber}.");

                if (CheckWin())
                    break;
            }

            if (isDraw)
            {
                Console.WriteLine("Не плохо, не плохо! У вас ничья!");
            }
            else
            {
                string winner = isPlayer ? $"игрока - {names[0]}" : "мега-бота";
                Console.WriteLine($"Поздравляем {winner} с победой! До скорых встреч!");
            }

            if (isDraw ^ (isFinish && !isPlayer))
            {
                ExitOrRevenge();
            }
        }

        /// <summary>
        /// Запуск игры в многопользовательском режиме.
        /// </summary>
        private static void StartMultiPlay()
        {
            string name = String.Empty;        // Имя игрока, делающего ход. Изначально пустое имя

            while (!isFinish)
            {
                for (int i = 0; i < countPlayers; i++)
                {
                    name = names[i];
                    userTry = GetUserNumber(name);

                    gameNumber -= userTry;
                    Console.WriteLine($"Загаданное число: {gameNumber}.");

                    if (CheckWin())
                        break;
                }
            }

            Console.WriteLine(isDraw
                ? "Не плохо, не плохо! У вас ничья!"
                : $"Поздравляем {name} с победой! До скорых встреч!");

            ExitOrRevenge();
        }

        /// <summary>
        /// Выход из игры или реванш?
        /// </summary>
        private static void ExitOrRevenge()
        {
            Console.Write("Хотите реванш? (Y/N): ");
            if (Console.ReadKey().Key == ConsoleKey.Y)
            {
                StartSettings();

                if (modeNumber == 1) StartSoloPlay();
                else StartMultiPlay();
            }
            else
            {
                Console.WriteLine("\nСпасибо за игру! Внимание, сейчас игра закроется!");
                Thread.Sleep(4000);
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Проверка на конец игры.
        /// </summary>
        /// <returns>Игра окончена или нет.</returns>
        private static bool CheckWin()
        {
            switch (gameNumber)
            {
                case < 0:
                    isDraw = true;
                    isFinish = true;
                    return true;
                case 0:
                    isFinish = true;
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Получить число введенное пользователем.
        /// </summary>
        /// <returns>Число пользователя.</returns>
        private static int GetUserNumber(string name)
        {
            Console.Write($"{name}, введите число от 2 до 7: ");
            int userNum = Convert.ToInt32(Console.ReadLine());

            // Проверка введенного числа на условие.
            while (userNum is < 2 or > 7)
            {
                Console.WriteLine("Ошибка! Число должно быть от 2 до 7!");
                Console.Write("Введите число от 2 до 7: ");
                userNum = Convert.ToInt32(Console.ReadLine());
            }

            return userNum;
        }

        /// <summary>
        /// Установить имена игрокам.
        /// </summary>
        /// <param name="countPlayers">Количество игроков.</param>
        private static void SetNames(int countPlayers)
        {
            names = new string[countPlayers];
            int i = 0;      // Итератор


            for (; i < countPlayers; i++)
            {
                Console.Write(countPlayers == 1
                    ? "Введите имя игрока: "
                    : $"Введите имя {i + 1}-го игрока: ");
                names[i] = Console.ReadLine();
            }
        }

        /// <summary>
        /// Установка случайного числа.
        /// </summary>
        private static void InitializeGameNumber()
        {
            // Создаем случайное число с интервалом, заданным игроком.
            Random rnd = new Random();
            Console.Write("\nВидите минимальное значение числа: ");
            ushort min = Convert.ToUInt16(Console.ReadLine());
            Console.Write("Видите максимальное значение числа: ");
            ushort max = Convert.ToUInt16(Console.ReadLine());
            gameNumber = min < max ? rnd.Next(min, max + 1) : rnd.Next(max, min + 1);

            Console.WriteLine($"Бот загадал число: {gameNumber}.");
        }
    }
}
