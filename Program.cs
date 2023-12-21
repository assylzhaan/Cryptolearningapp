using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp
{

    class Program
    {

        static List<Room> rooms = new List<Room>();
        static List<Booking> bookings = new List<Booking>();

        static void Main()
        {
            LoadData();

            while (true)
            {
                DisplayMenu();

                Console.Write("Выберите опцию: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        DisplayRooms();
                        break;
                    case "2":
                        BookRoom();
                        break;
                    case "3":
                        CancelBooking();
                        break;
                    case "4":
                        SearchRooms();
                        break;
                    case "5":
                        SaveData();
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Неверный ввод. Попробуйте еще раз.");
                        break;
                }
            }
        }


        static void DisplayMenu()
        {
            Console.WriteLine("1. Просмотреть все номера");
            Console.WriteLine("2. Бронировать номер");
            Console.WriteLine("3. Отменить бронь");
            Console.WriteLine("4. Расширенный поиск");
            Console.WriteLine("5. Выход");
        }


        // Метод для просмотра всех номеров
        static void DisplayRooms()
        {
            Console.WriteLine("Список номеров:");
            foreach (var room in rooms)
            {
                Console.WriteLine($"Номер: {room.Number}, Тип: {room.Type}, Стоимость: {room.Price}, Статус: {(room.IsBooked ? "Забронирован" : "Свободен")}");
            }
        }
        // Метод для бронирования номера
        static void BookRoom()
        {
            DisplayRooms();

            Console.Write("Введите ваше имя: ");
            string guestName = Console.ReadLine();

            Console.Write("Введите номер комнаты для бронирования: ");
            int roomNumber = int.Parse(Console.ReadLine());

            Console.Write("Введите дату прибытия (yyyy-MM-dd): ");
            DateTime checkInDate = DateTime.Parse(Console.ReadLine());

            Console.Write("Введите дату отъезда (yyyy-MM-dd): ");
            DateTime checkOutDate = DateTime.Parse(Console.ReadLine());

            Room selectedRoom = rooms.Find(room => room.Number == roomNumber);

            if (selectedRoom != null && !selectedRoom.IsBooked)
            {
                if (IsRoomAvailable(roomNumber, checkInDate, checkOutDate))
                {
                    Booking booking = new Booking(guestName, roomNumber, checkInDate, checkOutDate);
                    bookings.Add(booking);
                    selectedRoom.IsBooked = true;

                    Console.WriteLine($"Номер {roomNumber} успешно забронирован для {guestName}.");
                    LogAction($"Бронирование: {guestName} забронировал номер {roomNumber} с {checkInDate.ToShortDateString()} по {checkOutDate.ToShortDateString()}.");
                }
                else
                {
                    Console.WriteLine("Выбранный номер недоступен на указанные даты.");
                }
            }
            else
            {
                Console.WriteLine("Выбранный номер недоступен или уже забронирован.");
            }
        }

        static void CancelBooking()
        {
            Console.Write("Введите идентификатор брони для отмены: ");
            Guid bookingId;

            if (Guid.TryParse(Console.ReadLine(), out bookingId))
            {
                Booking bookingToRemove = bookings.Find(booking => booking.BookingId == bookingId);

                if (bookingToRemove != null)
                {
                    Room bookedRoom = rooms.Find(room => room.Number == bookingToRemove.RoomNumber);
                    bookedRoom.IsBooked = false;

                    bookings.Remove(bookingToRemove);

                    Console.WriteLine($"Бронь успешно отменена для номера {bookingToRemove.RoomNumber}.");
                    LogAction($"Отмена брони: Бронь {bookingId} отменена.");
                }
                else
                {
                    Console.WriteLine("Бронь с указанным идентификатором не найдена.");
                }
            }
            else
            {
                Console.WriteLine("Неверный формат идентификатора брони.");
            }
        }

        static void SearchRooms()
        {
            Console.WriteLine("Выберите опцию поиска:");
            Console.WriteLine("1. По типу");
            Console.WriteLine("2. По стоимости");
            Console.WriteLine("3. По датам доступности");
            Console.Write("Ваш выбор: ");
            string searchChoice = Console.ReadLine();

            switch (searchChoice)
            {
                case "1":
                    Console.Write("Введите тип номера для поиска: ");
                    string roomType = Console.ReadLine();
                    var roomsByType = rooms.FindAll(room => room.Type.Equals(roomType, StringComparison.OrdinalIgnoreCase));
                    DisplayFilteredRooms(roomsByType);
                    break;
                case "2":
                    Console.Write("Введите максимальную стоимость для поиска: ");
                    decimal maxPrice = decimal.Parse(Console.ReadLine());
                    var roomsByPrice = rooms.FindAll(room => room.Price <= maxPrice);
                    DisplayFilteredRooms(roomsByPrice);
                    break;
                case "3":
                    Console.Write("Введите дату прибытия (yyyy-MM-dd): ");
                    DateTime searchCheckInDate = DateTime.Parse(Console.ReadLine());
                    Console.Write("Введите дату отъезда (yyyy-MM-dd): ");
                    DateTime searchCheckOutDate = DateTime.Parse(Console.ReadLine());
                    var availableRooms = rooms.FindAll(room => IsRoomAvailable(room.Number, searchCheckInDate, searchCheckOutDate));
                    DisplayFilteredRooms(availableRooms);
                    break;
                default:
                    Console.WriteLine("Неверный ввод. Поиск отменен.");
                    break;
            }
        }

        static void DisplayFilteredRooms(List<Room> filteredRooms)
        {
            Console.WriteLine("Результаты поиска:");
            foreach (var room in filteredRooms)
            {
                Console.WriteLine($"Номер: {room.Number}, Тип: {room.Type}, Стоимость: {room.Price}, Статус: {(room.IsBooked ? "Забронирован" : "Свободен")}");
            }
        }

        static void LogAction(string action)
        {
            Console.WriteLine(action);
        }

        static bool IsRoomAvailable(int roomNumber, DateTime checkInDate, DateTime checkOutDate)
        {

            var conflictingBooking = bookings.Find(booking =>
                booking.RoomNumber == roomNumber &&
                !(checkOutDate <= booking.CheckInDate || checkInDate >= booking.CheckOutDate));

            return conflictingBooking == null;
        }
        // Метод для загрузки данных из файлов
        static void LoadData()
        {
            try
            {
                string[] roomLines = File.ReadAllLines("rooms.txt");
                foreach (var roomLine in roomLines)
                {
                    string[] roomData = roomLine.Split(',');
                    int number = int.Parse(roomData[0]);
                    string type = roomData[1];
                    decimal price = decimal.Parse(roomData[2]);
                    rooms.Add(new Room(number, type, price));
                }

                string[] bookingLines = File.ReadAllLines("bookings.txt");
                foreach (var bookingLine in bookingLines)
                {
                    string[] bookingData = bookingLine.Split(',');
                    Guid bookingId = Guid.Parse(bookingData[0]);
                    string guestName = bookingData[1];
                    int roomNumber = int.Parse(bookingData[2]);
                    DateTime checkInDate = DateTime.Parse(bookingData[3]);
                    DateTime checkOutDate = DateTime.Parse(bookingData[4]);
                    bookings.Add(new Booking(guestName, roomNumber, checkInDate, checkOutDate) { BookingId = bookingId });
                    rooms.Find(room => room.Number == roomNumber).IsBooked = true;
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Ошибка при загрузке данных: {ex.Message}");
            }
        }

        // Метод для сохранения данных в файлы
        static void SaveData()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter("rooms.txt"))
                {
                    foreach (var room in rooms)
                    {
                        writer.WriteLine($"{room.Number},{room.Type},{room.Price}");
                    }
                }

                using (StreamWriter writer = new StreamWriter("bookings.txt"))
                {
                    foreach (var booking in bookings)
                    {
                        writer.WriteLine($"{booking.BookingId},{booking.GuestName},{booking.RoomNumber},{booking.CheckInDate},{booking.CheckOutDate}");
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Ошибка при сохранении данных: {ex.Message}");
            }
        }
    }

}


