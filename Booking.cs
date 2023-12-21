using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp
{
    class Booking
    {
        public Guid BookingId { get; set; }
        public string GuestName { get; set; }
        public int RoomNumber { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        public Booking(string guestName, int roomNumber, DateTime checkInDate, DateTime checkOutDate)
        {
            BookingId = Guid.NewGuid();
            GuestName = guestName;
            RoomNumber = roomNumber;
            CheckInDate = checkInDate;
            CheckOutDate = checkOutDate;
        }
    }
}
