﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using WebApplication.Infrastructure;

namespace WebApplication.Controllers
{
    public class MonthsController : Controller
    {
        private readonly DataDb _dataDb = new DataDb();

        // GET: Months
        public ActionResult Index([Bind(Prefix = "id")]Guid? siteId)
        {
            if (siteId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Site site = _dataDb.Sites.Find(siteId);
            if (site == null)
            {
                return HttpNotFound();
            }
            return View(site);
        }

        // GET: Months/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Month month = _dataDb.Months.Find(id);
            if (month == null)
            {
                return HttpNotFound();
            }
            return View(month);
        }

        // GET: Months/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Months/Create/Id
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id, MonthTime")] Guid id, Month month)
        {
            // TODO: check that the user is allowed to do this.
            if (ModelState.IsValid)
            {
                month.Id = Guid.NewGuid();
                month.SiteId = id;
                _dataDb.Months.Add(month);
                _dataDb.SaveChanges();
                return RedirectToAction("Attention", new { Id = month.Id });
            }

            return View(month);
        }

        // GET: Months/Attention/117ca2a3-fb5a-4882-8e74-23cccf07db73
        public ActionResult Attention(Guid? id, string message) {return this.GetView(id, message);}
        public ActionResult Arrive(Guid? id, string message) { return this.GetView(id, message); }
        public ActionResult Shop(Guid? id, string message) { return this.GetView(id, message); }
        private ActionResult GetView(Guid? id, string message)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Month month = _dataDb.Months.Find(id);
            if (month == null)
            {
                return HttpNotFound();
            }
            ViewBag.Message = message;
            return View(month);
        }

        // POST: Months/Attention/117ca2a3-fb5a-4882-8e74-23cccf07db73
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Attention(Month month) // TODO: replace the [Bind(Include = "MarketingSpend")]
        {
            if (ModelState.IsValid)
            {
                _dataDb.Entry(month).State = EntityState.Unchanged;
                _dataDb.Entry(month).Property(e => e.MarketingSpend).IsModified = true;
                _dataDb.Entry(month).Property(e => e.RegionalTv).IsModified = true;
                _dataDb.Entry(month).Property(e => e.NationalTv).IsModified = true;
                _dataDb.Entry(month).Property(e => e.OverseasTv).IsModified = true;
                _dataDb.Entry(month).Property(e => e.WebsiteUrl).IsModified = true;
                _dataDb.Entry(month).Property(e => e.WebsiteVisitors).IsModified = true;
                _dataDb.Entry(month).Property(e => e.FacebookUrl).IsModified = true;
                _dataDb.Entry(month).Property(e => e.TwitterUrl).IsModified = true;
                _dataDb.Entry(month).Property(e => e.FlickrUrl).IsModified = true;
                _dataDb.Entry(month).Property(e => e.InstagramUrl).IsModified = true;
                _dataDb.Entry(month).Property(e => e.YoutubeUrl).IsModified = true;
                _dataDb.Entry(month).Property(e => e.VimeoUrl).IsModified = true;
                _dataDb.Entry(month).Property(e => e.PinterestUrl).IsModified = true;
                _dataDb.SaveChanges();
                return RedirectToAction("Attention", new { id = month.Id, message = "Updated." });
            }
            return View(month);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Arrive(Month month) // TODO: replace the [Bind(Include = "HoursMonday")]
        {
            if (ModelState.IsValid)
            {
                _dataDb.Entry(month).State = EntityState.Unchanged;
                _dataDb.Entry(month).Property(e => e.HoursMonday).IsModified = true;
                _dataDb.Entry(month).Property(e => e.HoursTuesday).IsModified = true;
                _dataDb.SaveChanges();
                return RedirectToAction("Arrive", new { id = month.Id, message = "Updated." });
            }
            return View(month);
        }


        // GET: Months/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Month month = _dataDb.Months.Find(id);
            if (month == null)
            {
                return HttpNotFound();
            }
            return View(month);
        }

        // POST: Months/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Month month = _dataDb.Months.Find(id);
            _dataDb.Months.Remove(month);
            _dataDb.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dataDb.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
