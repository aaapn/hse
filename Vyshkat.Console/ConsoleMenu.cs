using System.Globalization;
using Vyshkat.Domain.Interfaces;
using Vyshkat.Domain.Models;
using Vyshkat.Domain.Services;

namespace Vyshkat.ConsoleApp;

/// <summary>Пользовательский ввод и вывод</summary>
public sealed class ConsoleMenu
{
    private readonly InventoryManager _manager;
    private readonly VehicleFactory _factory;

    public ConsoleMenu(InventoryManager manager, VehicleFactory factory)
    {
        _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public void Run() {
        try {
            while (true) {
                PrintMenu();
                int choice = ReadInt("Выберите пункт (1–7): ", 1, 7);
                System.Console.WriteLine();

                switch (choice) {
                    case 1:
                        AddVehicle();
                        break;
                    case 2:
                        AddItem();
                        break;
                    case 3:
                        PrintAllItems();
                        break;
                    case 4:
                        PrintBeginnerVehicles();
                        break;
                    case 5:
                        PrintInventoryReport();
                        break;
                    case 6:
                        PrintEnergyConsumption();
                        break;
                    case 7:
                        System.Console.WriteLine("До встречи в «ВышКат»!");
                        return;
                }

                System.Console.WriteLine();
            }
        }
        catch (EndOfStreamException) {
            // Если перенаправляем ввода из файла, то корректно завершаем работу на EOF
            System.Console.WriteLine("\nВвод завершён. Выход из программы.");
        }
    }

    private static void PrintMenu() {
        System.Console.WriteLine("=== ВышКат — управление парком ===");
        System.Console.WriteLine("1) Добавить транспорт");
        System.Console.WriteLine("2) Добавить вещь");
        System.Console.WriteLine("3) Показать инвентарь");
        System.Console.WriteLine("4) Показать транспорт для новичков");
        System.Console.WriteLine("5) Отчёт по инвентарю");
        System.Console.WriteLine("6) Энергопотребление");
        System.Console.WriteLine("7) Выход");
    }

    private void AddVehicle() {
        System.Console.WriteLine("Выберите вид транспорта:");
        System.Console.WriteLine("1) Электросамокат");
        System.Console.WriteLine("2) Велосипед");
        System.Console.WriteLine("3) Электровелосипед");
        System.Console.WriteLine("4) Электрокар");

        VehicleType type = (VehicleType)ReadInt("Ваш выбор: ", 1, 4);
        int id = ReadInt("Введите ID (положительное число): ", 1, int.MaxValue);
        int ease = ReadInt("Простота для новичка (1–10): ", 1, 10);
        bool brakes = ReadBool("Тормоза исправны (да/нет): ");
        bool steering = ReadBool("Рулевое управление исправно (да/нет): ");

        int? battery = null;
        decimal? energy = null;
        if (type != VehicleType.Bicycle) {
            battery = ReadInt("Заряд батареи (0–100%): ", 0, 100);
            energy = ReadDecimal("Суточный расход энергии (кВт·ч, >= 0): ");
        }

        // Есои повторно вводится ID, то все ранее введённые характеристики сохраняются
        id = ReadFreeId(id);

        try {
            var data = new VehicleData(id, ease, brakes, steering, battery, energy);
            Vehicle vehicle = _factory.CreateVehicle(type, data);
            OperationResult result = _manager.AddVehicle(vehicle);
            PrintResult(result);
        } catch (ArgumentException ex) {
            // Зашита если вызвали доменную модель с неверными данными
            System.Console.WriteLine($"Не удалось создать транспорт: {ex.Message}");
        }
    }

    private void AddItem() {
        System.Console.WriteLine("Выберите категорию вещи:");
        System.Console.WriteLine("1) Шлем");
        System.Console.WriteLine("2) Зарядная станция");
        System.Console.WriteLine("3) Аптечка первой помощи");

        int type = ReadInt("Ваш выбор: ", 1, 3);
        int id = ReadInt("Введите ID (натуральное число): ", 1, int.MaxValue);
        decimal energy = type == 2 ? ReadDecimal("Суточный расход зарядной станции (кВт·ч, >= 0): "): 0m;

        id = ReadFreeId(id);

        InventoryItem item = type switch
        {
            1 => new Helmet(id),
            2 => new ChargingStation(id, energy),
            3 => new FirstAidKit(id),
            _ => throw new InvalidOperationException("Неизвестный вид вещи.")
        };

        PrintResult(_manager.AddItem(item));
    }

    private int ReadFreeId(int id) {
        while (_manager.GetItems().Any(item => item.Id == id))
        {
            System.Console.WriteLine($"Ошибка: ID {id} уже занят.");
            id = ReadInt("Введите другой ID: ", 1, int.MaxValue);
        }

        return id;
    }

    private void PrintAllItems() {
        var items = _manager.GetItems().OrderBy(item => item.Id).ToArray();
        if (items.Length == 0) {
            System.Console.WriteLine("Инвентарь пуст.");
            return;
        }

        System.Console.WriteLine("Инвентарь парка:");
        foreach (IInventory item in items) {
            string category = item is Vehicle ? "Транспорт" : "Вещь";
            System.Console.WriteLine($"{category}: {item.TypeName}; ID: {item.Id}");
        }
    }

    private void PrintBeginnerVehicles() {
        var vehicles = _manager.GetBeginnerVehicles().OrderBy(vehicle => vehicle.Id).ToArray();
        if (vehicles.Length == 0) {
            System.Console.WriteLine("Транспорта для новичков пока нет.");
            return;
        }

        System.Console.WriteLine("Транспорт для новичков (простота >= 6):");
        foreach (Vehicle vehicle in vehicles) {
            System.Console.WriteLine($"{vehicle.TypeName}; ID: {vehicle.Id}; простота: {vehicle.EaseOfUse}/10");
        }
            

    private void PrintInventoryReport() {
        InventoryReport report = _manager.GetInventoryReport();
        System.Console.WriteLine("=== Отчёт по инвентарю ===");
        System.Console.WriteLine($"Количество транспорта: {report.VehicleCount}");
        System.Console.WriteLine($"Количество вещей: {report.ItemCount}");
        System.Console.WriteLine($"Общее количество объектов: {report.TotalCount}");
    }

    private void PrintEnergyConsumption() {
        System.Console.WriteLine("=== Энергопотребление ===");
        System.Console.WriteLine($"Суммарный суточный расход: {_manager.GetEnergyConsumption():0.##} кВт·ч");
        System.Console.WriteLine("Учитываются электротранспорт и зарядные станции.");
    }

    private static void PrintResult(OperationResult result) {
        System.Console.WriteLine((result.Success ? "Успешно: " : "Отказ: ") + result.Message);
    }

    private static int ReadInt(string prompt, int min, int max) {
        while (true) {
            System.Console.Write(prompt);
            string input = ReadLine();
            if (int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out int number)
                && number >= min && number <= max)
                return number;

            System.Console.WriteLine($"Ошибка ввода. Введите целое число от {min} до {max}.");
        }
    }

    private static decimal ReadDecimal(string prompt) {
        const NumberStyles styles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign
            | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;
        while (true) {
            System.Console.Write(prompt);
            string input = ReadLine();
            if ((decimal.TryParse(input, styles, CultureInfo.GetCultureInfo("ru-RU"), out decimal value)
                || decimal.TryParse(input, styles, CultureInfo.InvariantCulture, out value)) && value >= 0)
                return value;

            System.Console.WriteLine("Ошибка ввода. Введите неотрицательное число (например, 2,5 или 2.5).");
        }
    }

    private static bool ReadBool(string prompt) {
        while (true) {
            System.Console.Write(prompt);
            string input = ReadLine().Trim().ToLowerInvariant();
            switch (input)
            {
                case "да":
                case "д":
                case "yes":
                case "y":
                case "true":
                    return true;
                case "нет":
                case "н":
                case "no":
                case "n":
                case "false":
                    return false;
                default:
                    System.Console.WriteLine("Ошибка ввода. Ответьте «да» или «нет».");
                    break;
            }
        }
    }

    private static string ReadLine() => System.Console.ReadLine() ?? throw new EndOfStreamException();
}
