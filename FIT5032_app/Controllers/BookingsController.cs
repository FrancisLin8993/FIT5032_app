using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FIT5032_app.Models;
using Microsoft.AspNet.Identity;

namespace FIT5032_app.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        private AppBookingModel db = new AppBookingModel();

        // GET: Bookings
        
        public ActionResult Index()
        {
             var userdb = new ApplicationDbContext();
             
            var userId = User.Identity.GetUserId();
            var appuser = (from u in userdb.Users
                           select new { user = u }).ToArray();

            var adminQuery = from u in appuser
                        join b in db.Bookings on u.user.Id equals b.UserId                       
                        select new BookingEmailViewModel
                        {
                            Booking = b,
                            User = u.user
                        };

            var query = from u in appuser
                        join b in db.Bookings on u.user.Id equals b.UserId
                        where b.UserId == userId
                        select new BookingEmailViewModel
                        {
                            Booking = b,
                            User = u.user
                        };

            if (User.IsInRole("admin"))
            {
                return View(adminQuery);
            }
            else
            {
                return View(query);
            }
                


            
            //var allBookings = db.Bookings.ToList();
            /*var bookings = db.Bookings.Where(b => b.UserId == userId).ToList();
            if (User.IsInRole("admin"))
            {
                return View(allBookings);
                return View(query);
            }
            else
            {
                vm.Booking = db.Bookings.Where(b => b.UserId == userId).ToList();
                return View(vm);
                return View(bookings);
            }*/

        }



        // GET: Bookings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // GET: Bookings/Create
        public ActionResult Create()
        {
            var userdb = new ApplicationDbContext();

            var userId = User.Identity.GetUserId();
            var appuser = (from u in userdb.Users
                           select new { userId = u.Id, email = u.Email }).ToArray();
            if (User.IsInRole("admin"))
            {
                ViewBag.Id = new SelectList(appuser.Where(u => u.email != "admin@admin.com"), "Id", "UserName");
            }


            var query = (from e in db.Events
                         where e.Available == true
                         select new {EventId = e.EventId, EventName = e.EventName})
                        .Except(from e in db.Events
                                join b in db.Bookings on e.EventId equals b.EventId
                                where b.UserId == userId
                                select new { EventId = e.EventId, EventName = e.EventName}).Distinct();

            //db.Events.Where(e => e.Available == true, "EventId", "EventName")
            ViewBag.EventId = new SelectList(query, "EventId", "EventName");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BookingId,EventId")] Booking booking, FormCollection form)
        {
            var userdb = new ApplicationDbContext();
            if (User.IsInRole("admin"))
            {
                String email = form[1];

                var query = (from u in userdb.Users
                            where u.Email == email
                            select new { userId = u.Id }).ToList().FirstOrDefault();

                booking.UserId = query.userId;
                ModelState.Clear();
                TryValidateModel(booking);

                if (ModelState.IsValid)
                {
                    db.Bookings.Add(booking);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            else
            {
                booking.UserId = User.Identity.GetUserId();
                ModelState.Clear();
                TryValidateModel(booking);

                if (ModelState.IsValid)
                {
                    db.Bookings.Add(booking);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

            }
            ViewBag.EventId = new SelectList(db.Events, "EventId", "EventName", booking.EventId);
            return View(booking);


        }

        // GET: Bookings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            ViewBag.EventId = new SelectList(db.Events, "EventId", "EventName", booking.EventId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookingId,UserId,EventId")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                db.Entry(booking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EventId = new SelectList(db.Events, "EventId", "EventName", booking.EventId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Booking booking = db.Bookings.Find(id);
            db.Bookings.Remove(booking);
            db.SaveChanges();
            return RedirectToAction("Index");
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
