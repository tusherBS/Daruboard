using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using AppLimit.CloudComputing.SharpBox;
using Daruyanagi.Models;

namespace Daruyanagi.Controllers
{
    public class PageController : Controller
    {
        private PageRepository repository = new PageRepository();

        //
        // GET: /Page/

        public ActionResult Index()
        {
            var pages = repository.List();

            return View(pages);
        }

        //
        // GET: /Home/Details/5

        public ActionResult Details(string title)
        {
            return RedirectToAction("Details", "Wiki", new { title = title });
        }

        //
        // GET: /Home/Create

        [Authorize]
        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Home/Create

        [HttpPost, Authorize]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /Home/Edit/5
 
        [Authorize]
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Home/Edit/5

        [HttpPost, Authorize]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Home/Delete/5
 
        [Authorize]
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Home/Delete/5

        [HttpPost, Authorize]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
