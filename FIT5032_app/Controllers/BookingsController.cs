﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FIT5032_app.Models;
using FIT5032_app.Utils;
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
            if (TempData["error"] != null)
            {
                ViewBag.Error = TempData["error"].ToString();
            }
            var userdb = new ApplicationDbContext();
             
            var userId = User.Identity.GetUserId();
            var appuser = (from u in userdb.Users
                           select new { user = u }).ToArray();
            //Query all the booking record and join the user information
            var adminQuery = from u in appuser
                        join b in db.Bookings on u.user.Id equals b.UserId                       
                        select new BookingEmailViewModel
                        {
                            Booking = b,
                            User = u.user
                        };
            //Query the booking record for a specific user
            var query = from u in appuser
                        join b in db.Bookings on u.user.Id equals b.UserId
                        where b.UserId == userId
                        select new BookingEmailViewModel
                        {
                            Booking = b,
                            User = u.user
                        };
            //if the login user is admin, he can view all the booking record.
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
            var userdb = new ApplicationDbContext();
            //Create a new bookingEmailViewModel
            BookingEmailViewModel bevm = new BookingEmailViewModel();
            bevm.Booking = db.Bookings.Find(id);
            //find the user email according to the user id in the booking record.
            bevm.User = userdb.Users.Find(bevm.Booking.UserId);
            //Booking booking = db.Bookings.Find(id);
            if (bevm == null)
            {
                return HttpNotFound();
            }
            return View(bevm);
        }

        // GET: Bookings/Create
        public ActionResult Create()
        {            
                var userdb = new ApplicationDbContext();

                var userId = User.Identity.GetUserId();
                var appuser = (from u in userdb.Users
                               select new { userId = u.Id, email = u.Email }).ToArray();

                //A user cannot book a event twice, cannot book past events.
                //Query events that the current user has not booked yet.
                var query = (from e in db.Events
                             where e.Available == true && e.StartDateTime >= DateTime.Now
                             select new { EventId = e.EventId, EventName = e.EventName })
                            .Except(from e in db.Events
                                    join b in db.Bookings on e.EventId equals b.EventId
                                    where b.UserId == userId
                                    select new { EventId = e.EventId, EventName = e.EventName }).Distinct();

            if (query.Count() == 0)
            {
                TempData["error"] = "Sorry, you have no available events to book";
                return RedirectToAction("Index");
            }
            
            
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
            EmailSender es = new EmailSender();
            

            //If the login user is admin, an email has to be entered when creating bookings
            if (User.IsInRole("admin"))
            {
                //Get the value of email form
                String email = form[1];
                //Make a query of the email input
                var query = (from u in userdb.Users
                            where u.Email == email
                            select new { user = u }).ToList().FirstOrDefault();
                //If the result is null, display error
                if (query == null)
                {
                    ViewBag.Error = "Sorry, the email you entered does not exist";                 
                }
                else
                {
                    booking.UserId = query.user.Id;
                    ModelState.Clear();
                    TryValidateModel(booking);

                    if (ModelState.IsValid)
                    {
                        db.Bookings.Add(booking);
                        db.SaveChanges();
                        es.Send(email, "Booking Successful", "Your booking is successful.");
                        return RedirectToAction("Index");
                    }
                }
                
                
            }
            //If the login user is not admin, user does not need to enter email
            else
            {
                booking.UserId = User.Identity.GetUserId();
                ModelState.Clear();
                TryValidateModel(booking);
                var existBooking = (from b in db.Bookings
                                   where b.EventId == booking.EventId
                                   && b.UserId == booking.UserId
                                   select new { b }).ToList();

                if (existBooking.Count != 0)
                {
                    TempData["error"] = "Sorry, you have already booked this event.";
                    return RedirectToAction("Index");
                }

                var emailQuery = (from u in userdb.Users
                             where u.Id == booking.UserId
                             select new { email = u.Email }).ToList().FirstOrDefault();
                if (ModelState.IsValid)
                {
                    db.Bookings.Add(booking);
                    db.SaveChanges();
                    es.Send(emailQuery.email, "Booking Successful", "Your booking is successful.");
                    return RedirectToAction("Index");
                }

            }
            ViewBag.EventId = new SelectList(db.Events, "EventId", "EventName", booking.EventId);
            return View(booking);


        }

        


        // GET: Bookings/Edit/5
        // Bookings are not enabled to be edited.
        /*
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
        }*/

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
            EmailSender es = new EmailSender();

            var userdb = new ApplicationDbContext();

            Booking booking = db.Bookings.Find(id);
            //Query the email of a user in the corresponding booking record.
            var emailQuery = (from u in userdb.Users
                              where u.Id == booking.UserId
                              select new { email = u.Email }).ToList().FirstOrDefault();
            db.Bookings.Remove(booking);
            db.SaveChanges();
            es.Send(emailQuery.email, "Booking Successfully Cancelled", "Your booking is successfully cancelled.");
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
