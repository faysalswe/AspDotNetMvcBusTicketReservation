using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AspDotNetMvcBusTicketReservation.Models;

namespace AspDotNetMvcBusTicketReservation.Controllers
{
    public class TripsController : Controller
    {
        private AppDb db = new AppDb();

        public ActionResult Index()
        {
            var trips1 = db.Trips1.Include(t => t.tb_Bus).Include(t => t.tb_Driver).Include(t => t.tb_Route);
            return View(trips1.ToList());
        }

        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Trip trip = db.Trips1.Find(id);
        //    if (trip == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(trip);
        //}

        // GET: Trips/Create
        public ActionResult Create()
        {
            ViewBag.Bus = new SelectList(db.Buses, "Id", "SerialNo");
            ViewBag.Driver = new SelectList(db.Drivers, "Id", "DriverSerial");
            ViewBag.Route = new SelectList(db.Routes, "Id", "Title");
            return View();
        }

        // POST: Trips/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Date,Time,Route,Bus,Driver,A1,A2,A3,A4,B1,B2,B3,B4,C1,C2,C3,C4,D1,D2,D3,D4,E1,E2,E3,E4,F1,F2,F3,F4,G1,G2,G3,G4,H1,H2,H3,H4,I1,I2,I3,I4")] Trip trip)
        {
            if (ModelState.IsValid)
            {
                db.Trips1.Add(trip);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Bus = new SelectList(db.Buses, "Id", "SerialNo", trip.Bus);
            ViewBag.Driver = new SelectList(db.Drivers, "Id", "DriverSerial", trip.Driver);
            ViewBag.Route = new SelectList(db.Routes, "Id", "Title", trip.Route);
            return View(trip);
        }

        // GET: Trips/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trip trip = db.Trips1.Find(id);
            if (trip == null)
            {
                return HttpNotFound();
            }
            ViewBag.Bus = new SelectList(db.Buses, "Id", "SerialNo", trip.Bus);
            ViewBag.Driver = new SelectList(db.Drivers, "Id", "DriverSerial", trip.Driver);
            ViewBag.Route = new SelectList(db.Routes, "Id", "Title", trip.Route);
            return View(trip);
        }

        // POST: Trips/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Date,Time,Route,Bus,Driver,A1,A2,A3,A4,B1,B2,B3,B4,C1,C2,C3,C4,D1,D2,D3,D4,E1,E2,E3,E4,F1,F2,F3,F4,G1,G2,G3,G4,H1,H2,H3,H4,I1,I2,I3,I4")] Trip trip)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trip).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Bus = new SelectList(db.Buses, "Id", "SerialNo", trip.Bus);
            ViewBag.Driver = new SelectList(db.Drivers, "Id", "DriverSerial", trip.Driver);
            ViewBag.Route = new SelectList(db.Routes, "Id", "Title", trip.Route);
            return View(trip);
        }

        // GET: Trips/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trip trip = db.Trips1.Find(id);
            if (trip == null)
            {
                return HttpNotFound();
            }
            return View(trip);
        }

        // POST: Trips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Trip trip = db.Trips1.Find(id);
            db.Trips1.Remove(trip);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    
        public ActionResult BookingBydate(DateTime? date)
        {
            if(date == null)
            {
                date = DateTime.Now;
            }
           return View(db.SpGetBookingListByDate(date).ToList());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
