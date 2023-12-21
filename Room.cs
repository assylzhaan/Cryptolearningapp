using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp
{
    class Room
    {
        public int Number { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }
        public bool IsBooked { get; set; }

        public Room(int number, string type, decimal price)
        {
            Number = number;
            Type = type;
            Price = price;
            IsBooked = false;
        }
    }
}
