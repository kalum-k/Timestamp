﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTracker.Services;
using WebApplication6.Areas.Identity.Data;
using WebApplication6.Data;
using static TimeTracker.Enum.Alerts;

namespace TimeTracker.Controllers
{
    [Authorize(Roles = "Admin,Users")]
    public class TimeTrackerController : Controller
    {
        private readonly TimeTrackerDbContext _dbContext;
        public TimeTrackerController(TimeTrackerDbContext timeTacker)
        {
            _dbContext = timeTacker;
        }
        public IActionResult Index(string searchString)
        {
            return View();
        }
        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> Timetracker(string id ,string searchString)
        {
            if (id == null)
            {
                return NotFound();
            }
            var timeFromDbFirst = _dbContext.timeTackers
                .Where(u => u.IdUser == id)
                .OrderByDescending(y => y.Id);
                //.Take(10);

            if (timeFromDbFirst == null)
            {
                return NotFound();
            }
            return View(timeFromDbFirst);
        }

        public async Task<IActionResult> TimetrackerMonth(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var timeFromDbFirst = _dbContext.timeTackers
                .Where(u => u.IdUser == id && u.CurrentDate.Month == DateTime.Now.Month)
                .OrderByDescending(y => y.Id);
                //.Take(10);

            if (timeFromDbFirst == null)
            {
                return NotFound();
            }
            return View(timeFromDbFirst);
        }



        //Get
        public IActionResult Create()
        {
            return View("Create");
        }


        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TimeTrackers timeTacker)
        {
            
            if (ModelState.IsValid)
            {
                if (timeTacker.CurrentDate != DateTime.Today)
                {
                    TempData["err"] = "Not Successfully";
                    return NotFound();
                }
                else
                {

                    _dbContext.Add(timeTacker);
                    //ViewBag.Alert = CommonServices.ShowAlert(Alert.Success, "time added");
                    TempData["success"] = "Time Created";
                    _dbContext.SaveChanges();
                    return RedirectToAction("Index");
                }

            }
            return View(timeTacker);
        }


        //GET
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var timeFromDb = _dbContext.timeTackers.Find(id);

            if (timeFromDb == null)
            {
                return NotFound();
            }

            return View(timeFromDb);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TimeTrackers timeTackers)
        {
            if (ModelState.IsValid)
            {

                _dbContext.timeTackers.Update(timeTackers);
                TempData["success"] = "Time Updates";
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(timeTackers);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var timeFromDb = _dbContext.timeTackers.Find(id);

            if (timeFromDb == null)
            {
                return NotFound();
            }

            return View(timeFromDb);
        }

        //POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _dbContext.timeTackers.Find(id);
            if (obj == null)
            {
                return NotFound();
            }

            _dbContext.timeTackers.Remove(obj);
            _dbContext.SaveChanges();
            TempData["success"] = "Deleted";
            return RedirectToAction("Index");

        }
        

    }
}
