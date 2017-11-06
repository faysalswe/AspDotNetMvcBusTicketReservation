using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AspDotNetMvcBusTicketReservation.Models;
using System.Globalization;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using Microsoft.Reporting.WebForms;

namespace AspDotNetMvcBusTicketReservation.Controllers
{
    public class HomeController : Controller
    {
        protected AppDb db = new AppDb();
        public ActionResult Index()
        {
            return RedirectToAction("TravalRoute");
        }

        [HttpGet]
        public ActionResult TravalRoute()
        {
            ViewBag.Routes = new SelectList(db.Routes.ToList(), "Id", "Title");
            return View();
        }
        public IEnumerable<DateTime?> GetTravalDate(int id)
        {
            DateTime date = DateTime.Now.Date;

            var dates = db.Trips1.Where(m => m.Route == id && m.Date >= date).Select(x => x.Date).Distinct().ToList();
            return dates.ToList();
        }

        public JsonResult TravalDate(string id)
        {
            var stateListt = this.GetTravalDate(Convert.ToInt32(id));
            var statesList = stateListt.Select(m => new SelectListItem()
            {
                Text = m.Value.ToString(),
                Value = Convert.ToDateTime(m.Value).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture)
            });

            return Json(statesList, JsonRequestBehavior.AllowGet);
        }
        public IList<Trip> GetTravalTime(DateTime Date, int route)
        {
            return db.Trips1.Where(m => m.Date == Date && m.Route == route).ToList();
        }

        public JsonResult TravalTime(string id, string route)
        {
            var stateListt = this.GetTravalTime(Convert.ToDateTime(id), Convert.ToInt16(route));
            var statesList = stateListt.Select(m => new SelectListItem()
            {
                Text = m.Time.ToString(),
                Value = m.Time.ToString()
            });
            return Json(statesList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TravalRoute(string Routes, string Dates, string Times)
        {
            if (Routes != null && Dates != null && Times != null)
            {
                int routeid = int.Parse(Routes);
                DateTime d = Convert.ToDateTime(Dates);
                TimeSpan t = TimeSpan.Parse(Times);
                Trip aTrip = db.Trips1.Where(x => x.Route == routeid && x.Date == d && x.Time == t).First();
                TempData["tripId"] = aTrip.Id;
                Route aRoute = db.Routes.Find(routeid);
                Session["Fare"] = aRoute.Fare;
                return RedirectToAction("SelectSeat");
            }

            return RedirectToAction("TravalRoute");
        }


        [Authorize]
        [HttpGet]
        public ActionResult SelectSeat()
        {

            try
            {
                TempData["Fare"] = double.Parse(Session["Fare"].ToString());
                int tripid = Convert.ToInt32(TempData["tripId"].ToString());
                TempData["tripId"] = tripid;
                Response.Cookies["tripid"].Value = tripid.ToString();
                Trip seat = db.Trips1.Find(tripid);
                return View(seat);
            }
            catch (Exception ex)
            {
                return RedirectToAction("TravalRoute");
            }


        }


        [Authorize]
        [HttpPost]
        public ActionResult SelectSeat(string seatlist)
        {
            Response.Cookies["seatlist"].Value = seatlist;

            string[] inputs = seatlist.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            int i = 0;
            while (i < seatlist.Length)
            {
                i++;
            }

            if (seatlist != null)
            {
                Book aBooking = new Book();
                aBooking.seatList = seatlist;
                aBooking.TripId = Convert.ToInt32(TempData["tripId"].ToString());
                aBooking.Fare = (i / 3) * Convert.ToDecimal(TempData["Fare"].ToString());
                aBooking.Date = DateTime.Now;
                aBooking.UserId = User.Identity.GetUserId();
                db.Books.Add(aBooking);
                db.SaveChanges();
                TempData["bookId"] = aBooking.Id;
                TempData["seatlist"] = seatlist;

                return RedirectToAction("Payment");
            }
            ViewBag.msgslectseat = "please select seat";
            return RedirectToAction("SelectSeat");
        }

        [Authorize]
        [HttpGet]
        public ActionResult Payment()
        {
            try
            {
                TempData["bookId"] = Convert.ToInt32(TempData["bookId"].ToString());
                return View();
            }
            catch (Exception)
            {

                return View();
            }
        }

        [Authorize]
        private void UpdateBook(int id, string payment, string transactionId)
        {
            using (AppDb db = new AppDb())
            {
                var book = new Book() { Id = id, payment = payment, TransactionId = transactionId };
                db.Books.Attach(book);
                db.Entry(book).Property(x => x.payment).IsModified = true;
                db.Entry(book).Property(x => x.TransactionId).IsModified = true;
                db.SaveChanges();
            }
        }


        [Authorize]
        [HttpPost]
        public ActionResult Payment(string payment, string transactionId)
        {
            try
            {
                int id = int.Parse(TempData["bookId"].ToString());
                UpdateBook(id, payment, transactionId);

                if (Request.Cookies["seatlist"] != null)
                {
                    string seatlist = Request.Cookies["seatlist"].Value.ToString();
                    string[] inputs = new string[4];

                    inputs[0] = null;
                    inputs[1] = null;
                    inputs[2] = null;
                    inputs[3] = null;

                    string[] seatname = seatlist.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < seatname.Length; i++)
                    {
                        inputs[i] = seatname[i];
                    }
                    db.UpdateTripSeat(id.ToString(), Request.Cookies["tripid"].Value.ToString(), inputs[0], inputs[1], inputs[2], inputs[3]);
                }
                else
                {
                    return RedirectToAction("TravalRoute");
                }
                Book bookmodel = db.Books.Find(id);
                TempData["bookId"] = Convert.ToInt32(TempData["bookId"].ToString());
                return View("PaymentSuccessful", bookmodel);
            }
            catch (Exception)
            {

                return RedirectToAction("Index");
            }
            
        }

        [Authorize]
        [HttpGet]
        public ActionResult BookingHistory()
        {
            return View(db.SpGetBookingListByUserId(User.Identity.GetUserId()));
        }

        [Authorize]
        public FileResult GetReport()
        {
            try
            {

            
            LocalReport lr = new LocalReport();
            lr.ReportPath = Server.MapPath("~/Content/Report/Report.rdlc");
           
            List<Book> cm = new List<Book>();
            using (AppDb dc = new AppDb())
            {
                int id = int.Parse(TempData["bookId"].ToString());
                cm = dc.Books.Where(x => x.Id == id ).ToList();
                TempData["bookId"] = Convert.ToInt32(TempData["bookId"].ToString());
            }
            ReportDataSource rd = new ReportDataSource("BookDataSet", cm);
            lr.DataSources.Add(rd);

            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo = "";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = lr.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);

                return File(renderedBytes, mimeType);
            }
            catch (Exception)
            {

                throw; ;
            }
            
        }


        public FileResult GetReportById(int bookingid)
        {
            LocalReport lr = new LocalReport();
            lr.ReportPath = Server.MapPath("~/Content/Report/Report.rdlc");

            List<Book> cm = new List<Book>();
            using (AppDb dc = new AppDb())
            {
                
                cm = dc.Books.Where(x => x.Id == bookingid).ToList();
            }
            ReportDataSource rd = new ReportDataSource("BookDataSet", cm);
            lr.DataSources.Add(rd);

            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo = "";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = lr.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);

            return File(renderedBytes, mimeType);
        }
    }
}