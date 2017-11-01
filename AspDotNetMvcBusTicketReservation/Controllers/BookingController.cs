using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AspDotNetMvcBusTicketReservation.Models;

namespace AspDotNetMvcBusTicketReservation.Controllers
{
    public class BookingController : Controller
    {
        protected AppDb db = new AppDb();
        [HttpGet]
        public ActionResult Index()
        {
            return View(db.SpGetBookingListByDate(Convert.ToDateTime(DateTime.Now.Date)));
        }

        [HttpPost]
        public ActionResult Index(string Date)
        {
            return View(db.SpGetBookingListByDate(Convert.ToDateTime(Date)));
        }
    }
}