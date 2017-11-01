using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace AspDotNetMvcBusTicketReservation.Models
{
    [MetadataType(typeof(BookingListByUserIdMetaData))]
    public partial class BookingListByUserId
    {
        
    }

    public class BookingListByUserIdMetaData
    {
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> TravelDate { get; set; }
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> PurchaseDate { get; set; }

    }
}