﻿using ComicBookLibraryManagerWebApp.ViewModels;
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
    /// Controller for adding/deleting comic book artists.
    /// </summary>
    public class ComicBookArtistsController : BaseController
    {
        //private Context Context = null;
        //private bool _disposed = false;

        //public ComicBookArtistsController()
        //{
        //    Context = new Context();
        //}

        public ActionResult Add(int comicBookId)
        {
            var comicBook = Context.ComicBooks
                            .Include(cb => cb.Series)
                            .Where(cb => cb.Id == comicBookId)
                            .SingleOrDefault();

            if (comicBook == null)
            {
                return HttpNotFound();
            }

            var viewModel = new ComicBookArtistsAddViewModel()
            {
                ComicBook = comicBook
            };

            // TODO Pass the Context class to the view model "Init" method.
            viewModel.Init(Context);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Add(ComicBookArtistsAddViewModel viewModel)
        {
            ValidateComicBookArtist(viewModel);

            if (ModelState.IsValid)
            {
                // TODO Add the comic book artist.
                var comicBookArtist = new ComicBookArtist()
                {
                    ComicBookId = viewModel.ComicBookId,
                    ArtistId = viewModel.ArtistId,
                    RoleId = viewModel.RoleId
                };
                
                Context.ComicBookArtists.Add(comicBookArtist);
                Context.SaveChanges();

                TempData["Message"] = "Your artist was successfully added!";

                return RedirectToAction("Detail", "ComicBooks", new { id = viewModel.ComicBookId });
            }
            
            viewModel.ComicBook = Context.ComicBooks
                                    .Include(cb => cb.Series)
                                    .Where(cb => cb.Id == viewModel.ComicBookId)
                                    .SingleOrDefault();
            
            viewModel.Init(Context);

            return View(viewModel);
        }

        public ActionResult Delete(int comicBookId, int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            // Include the "ComicBook.Series", "Artist", and "Role" navigation properties.
            var comicBookArtist = Context.ComicBookArtists
                    .Include(cba => cba.Artist)
                    .Include(cba => cba.Role)
                    .Include(cba => cba.ComicBook.Series)
                    .Where(cba => cba.Id == (int)id)
                    .SingleOrDefault();

            if (comicBookArtist == null)
            {
                return HttpNotFound();
            }

            return View(comicBookArtist);
        }

        [HttpPost]
        public ActionResult Delete(int comicBookId, int id)
        {
            // TODO Delete the comic book artist.
            var comicBookArtist = new ComicBookArtist() { Id = id };
            Context.Entry(comicBookArtist).State = EntityState.Deleted;
            Context.SaveChanges();

            TempData["Message"] = "Your artist was successfully deleted!";

            return RedirectToAction("Detail", "ComicBooks", new { id = comicBookId });
        }

        /// <summary>
        /// Validates a comic book artist on the server
        /// before adding a new record.
        /// </summary>
        /// <param name="viewModel">The view model containing the values to validate.</param>
        private void ValidateComicBookArtist(ComicBookArtistsAddViewModel viewModel)
        {
            // If there aren't any "ArtistId" and "RoleId" field validation errors...
            if (ModelState.IsValidField("ArtistId") &&
                ModelState.IsValidField("RoleId"))
            {
                // Then make sure that this artist and role combination 
                // doesn't already exist for this comic book.
                if (Context.ComicBookArtists
                    .Any(a => a.ComicBookId == viewModel.ComicBookId &&
                            a.RoleId == viewModel.RoleId &&
                            a.ArtistId == viewModel.ArtistId))
                {
                    ModelState.AddModelError("ArtistId",
                        "This artist and role combination already exists for this comic book.");
                }
            }
        }

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