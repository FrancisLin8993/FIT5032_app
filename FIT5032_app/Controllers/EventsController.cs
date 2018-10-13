using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FIT5032_app.Models;

namespace FIT5032_app.Controllers
{
    public class EventsController : Controller
    {
        private AppBookingModel db = new AppBookingModel();

        // GET: Events
        public ActionResult Index(String date)
        {
            if (null == date)
            {
                return View(db.Events.OrderByDescending(e => e.StartDateTime).ToList());
            }
            else
            {                
                //Check whether the query string can be parsed to a date.
                if(DateTime.TryParse(date, out DateTime dateValue))
                {
                    //Convert the date, and query the events which start date are greater than the current date.
                    DateTime convertedDate = DateTime.Parse(date);
                    var query = from e in db.Events
                                where DbFunctions.TruncateTime(e.StartDateTime) >= convertedDate
                                select e;
                    return View(query.OrderByDescending(e => e.StartDateTime).ToList());
                }
                else
                {
                    return HttpNotFound();
                }

                
            }
            
        }

        // GET: Events/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // GET: Events/Create
        //Only admin user can create events.
        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Create([Bind(Include = "EventId,EventName,StartDateTime,EventLength,Description,Available")] Event @event)
        {
            if (ModelState.IsValid)
            {
                //Check whether the input date time is valid.
                if (@event.IsDateTimeValid())
                {
                    db.Events.Add(@event);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "Your selected date must be within 1 years of the current date, and the event start time should be between 9am to 7pm";
                    return View(@event);
                }
                
            }

            return View(@event);
        }

        // GET: Events/Edit/5
        //Only admin user can edit events.
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Edit([Bind(Include = "EventId,EventName,StartDateTime,EventLength,Description,Available")] Event @event)
        {
            if (ModelState.IsValid)
            {
                //Check whether the input date time is valid.
                if (@event.IsDateTimeValid())
                {
                    db.Entry(@event).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "Sorry, your selected date must be within 1 years of the current date.";
                    return View(@event);
                }
                
            }
            return View(@event);
        }

        // GET: Events/Delete/5
        //Only admin can delete events.
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Event @event = db.Events.Find(id);
            //Check whether the event has booking records(children records in database)
            var booking = db.Bookings.Where(b => b.EventId == @event.EventId).ToList();
            if (booking.Count != 0)
            {
                ViewBag.Error = "Sorry, you cannot delete an event with booking records";
                return View(@event);
            }
            else
            {
                db.Events.Remove(@event);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            
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
