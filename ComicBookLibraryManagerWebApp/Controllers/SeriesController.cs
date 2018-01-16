using ComicBookShared.Data;
using ComicBookShared.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ComicBookLibraryManagerWebApp.Controllers
{
    /// <summary>
    /// Controller for the "Series" section of the website.
    /// </summary>
    public class SeriesController : BaseController
    {
        //TODO build Series Repo and connect repo to controller via strings placed below.
        //TODO make sure that this can go
        
            //private SeriesRepository _seriesRepository;
        //public SeriesController()
        //{
        //    _seriesRepository = new SeriesRepository(Context);
        //}

        //private Context _context = null;
        //private bool _disposed = false;
        //public SeriesController()
        //{
        //    _context = new Context();
        //}

        public ActionResult Index()
        {
            // TODO make sure this can go
            //var series = Context.Series
            //        .OrderBy(s => s.Title)
            //        .ToList();
            var series = Repository.GetSeriesList();
            return View(series);
        }

        public ActionResult Detail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //var series = Context.Series
            //        .Include(s => s.ComicBooks.Select(cb => cb.Series))
            //        .Where(s => s.Id == id)
            //        .SingleOrDefault(); 
            var series = Repository.GetSeriesById((int)id);
            
            if (series == null)
            {
                return HttpNotFound();
            }

            // Sort the comic books.
            series.ComicBooks = series.ComicBooks
                .OrderByDescending(cb => cb.IssueNumber).ToList();

            return View(series);
        }

        public ActionResult Add()
        {
            var series = new Series();

            return View(series);
        }

        [HttpPost]
        public ActionResult Add(Series series)
        {
            ValidateSeries(series);

            if (ModelState.IsValid)
            {
                Repository.AddSeries(series);

                TempData["Message"] = "Your series was successfully added!";

                return RedirectToAction("Detail", new { id = series.Id });
            }

            return View(series);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //var series = Context.Series
            //        .Where(s => s.Id == id)
            //        .SingleOrDefault(); ;
            var series = Repository.GetSeriesById((int)id);

            if (series == null)
            {
                return HttpNotFound();
            }

            return View(series);
        }

        [HttpPost]
        // TODO REPAIR this 
        public ActionResult Edit(Series series)
        {
            ValidateSeries(series);

            if (ModelState.IsValid)
            {
                var seriesToUpdate = series;

                // TODO Update the series.
                Context.Entry(seriesToUpdate).State = EntityState.Modified;
                Context.SaveChanges();
                
                TempData["Message"] = "Your series was successfully updated!";

                return RedirectToAction("Detail", new { id = series.Id });
            }

            return View(series);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //var series = Context.Series
            //        .Where(s => s.Id == id)
            //        .SingleOrDefault(); 
            var series = Repository.GetSeriesById((int)id);

            if (series == null)
            {
                return HttpNotFound();
            }

            return View(series);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var seriesToDelete = new Series() { Id = id };

            //var set = Context.Set<Series>();
            //var entity = set.Find(id);
            //set.Remove(entity);
            //Context.SaveChanges();
            Repository.DeleteSeries((int)id);

            TempData["Message"] = "Your series was successfully deleted!";

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Validates a series on the server
        /// before adding a new record or updating an existing record.
        /// </summary>
        /// <param name="series">The series to validate.</param>
        private void ValidateSeries(Series series)
        {
            // If there aren't any "Title" field validation errors...
            if (ModelState.IsValidField("Series.Title"))
            {
                // TODO Refactor into series repo
                // Then make sure that the provided title is unique.
               if (Context.Series.Any(s => s.Id != series.Id &&
                        s.Title == series.Title))
                {
                    ModelState.AddModelError("Series.IdNumber",
                        "The provided Id Number has already been entered for the selected Series.");
                }
                ////TODO make sure that this can go
                //if (false)
                //{
                //    ModelState.AddModelError("Title",
                //        "The provided Title is in use by another series.");
                //}
            }
        }

        //TODO make sure that this can go
        //protected override void Dispose(bool disposing)
        //{
        //    if (_disposed)
        //        return;
        //    if (disposing)
        //    {
        //        Context.Dispose();
        //    }

        //    _disposed = true;
        //    base.Dispose(disposing);
        //}
    }
}