using ComicBookLibraryManagerWebApp.ViewModels;
using ComicBookShared.Data;
using ComicBookShared.Models;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ComicBookLibraryManagerWebApp.Controllers
{
    /// <summary>
    /// Controller for the "Comic Books" section of the website.
    /// </summary>
    public class ComicBooksController : BaseController
    {
        private ComicBooksRepository _comicBooksRepository = null;
        private SeriesRepository _seriesRepository = null;
        private ArtistsRepository _artistsRepository = null;

        public ComicBooksController()
        {
            _comicBooksRepository = new ComicBooksRepository(Context);
            _seriesRepository = new SeriesRepository(Context);
            _artistsRepository = new ArtistsRepository(Context);
        }
        public ActionResult Index()
        {
            var comicBooks = _comicBooksRepository.GetList();

            return View(comicBooks);
        }

        public ActionResult Detail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            var comicBook = _comicBooksRepository.Get((int)id);

            if (comicBook == null)
            {
                return HttpNotFound();
            }

            // Sort the artists.
            comicBook.Artists = comicBook.Artists
                .OrderBy(a => a.Role.Name).ToList();

            return View(comicBook);
        }

        public ActionResult Add()
        {
            var viewModel = new ComicBooksAddViewModel();
            
            viewModel.Init(Repository, _seriesRepository, _artistsRepository);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Add(ComicBooksAddViewModel viewModel)
        {
            ValidateComicBook(viewModel.ComicBook);

            if (ModelState.IsValid)
            {
                var comicBook = viewModel.ComicBook;
                comicBook.AddArtist(viewModel.ArtistId, viewModel.RoleId);

                _comicBooksRepository.Add(comicBook);
               
                TempData["Message"] = "Your comic book was successfully added!";

                return RedirectToAction("Detail", new { id = comicBook.Id });
            }
            
            viewModel.Init(Repository, _seriesRepository, _artistsRepository);

            return View(viewModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            var comicBook = _comicBooksRepository.Get((int)id, 
                includeRelatedEntities: false);

            if (comicBook == null)
            {
                return HttpNotFound();
            }

            var viewModel = new ComicBooksEditViewModel()
            {
                ComicBook = comicBook
            };
            viewModel.Init(Repository, _seriesRepository, _artistsRepository);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(ComicBooksEditViewModel viewModel)
        {
            ValidateComicBook(viewModel.ComicBook);

            if (ModelState.IsValid)
            {
                var comicBook = viewModel.ComicBook;

                ///<summary>
                ///Add concurrency to Edit post method.  
                ///If another user has updated or deleted the object before this user has, 
                ///A message and redirect is laid out. 
                ///</summary>
                /// 

                try
                {
                    _comicBooksRepository.Update(comicBook);

                    TempData["Message"] = "Your comic book was successfully updated!";

                    return RedirectToAction("Detail", new { id = comicBook.Id });
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    string message = null;

                    var entityPropertyValues = ex.Entries.Single().GetDatabaseValues();
                    if (entityPropertyValues == null)
                    {
                        message = 
                            "The comic book being updated has been deleted by another user. Click cancel button to return to the list page.";

                        viewModel.ComicBookHasBeenDeleted = true;
                    }
                    else
                    {
                        message = 
                            "The comic book being updated has already been updated by another user. If you still want to make your changes, click the save button.";

                        comicBook.RowVersion =
                            ((ComicBook)entityPropertyValues.ToObject()).RowVersion;
                    }

                    ModelState.AddModelError(string.Empty, message);
                    //throw;
                }
            }

            viewModel.Init(Repository, _seriesRepository, _artistsRepository);

            return View(viewModel);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            var comicBook = _comicBooksRepository.Get((int)id);

            if (comicBook == null)
            {
                return HttpNotFound();
            }

            var viewModel = new ComicBooksDeleteViewModel()
            {
                ComicBook = comicBook
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Delete(ComicBooksDeleteViewModel viewModel)
        {
            try
            {
                _comicBooksRepository.Delete(viewModel.ComicBook.Id
                    , viewModel.ComicBook.RowVersion);

                TempData["Message"] = "Your comic book was successfully deleted!";

                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                string message = null;

                var entityPropertyValues = ex.Entries.Single().GetDatabaseValues();
                if (entityPropertyValues == null)
                {
                    message =
                        "The comic book being deleted has been deleted by another user. Click cancel button to return to the list page.";

                    viewModel.ComicBookHasBeenDeleted = true;
                }
                else
                {
                    message =
                        "The comic book being deleted has already been updated by another user. " +
                        "If you still want to delete, click the save button again.";

                    viewModel.ComicBook.RowVersion =
                        ((ComicBook)entityPropertyValues.ToObject()).RowVersion;
                }
                ModelState.AddModelError(string.Empty, message);
                return View(viewModel);
            }
            //var comicBook = new ComicBook() { Id = id };
        }

        /// <summary>
        /// Validates a comic book on the server
        /// before adding a new record or updating an existing record.
        /// </summary>
        /// <param name="comicBook">The comic book to validate.</param>
        private void ValidateComicBook(ComicBook comicBook)
        {
            // If there aren't any "SeriesId" and "IssueNumber" field validation errors...

            if (ModelState.IsValidField("ComicBook.SeriesId") &&
                ModelState.IsValidField("ComicBook.IssueNumber"))
            {
                // Then make sure that the provided issue number is unique for the provided series.
                if (_comicBooksRepository.ValidateDuplicateBook(comicBook))
                {
                    ModelState.AddModelError("ComicBook.IssueNumber",
                        "The provided Issue Number has already been entered for the selected Series.");
                }
            }
        }
    }
}