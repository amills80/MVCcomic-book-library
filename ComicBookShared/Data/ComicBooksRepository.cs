using ComicBookShared.Models;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicBookShared.Data
{
    public class ComicBooksRepository : BaseRepository<ComicBook>
    {
        public ComicBooksRepository(Context context)
            : base(context)
        {
        }

        public override IList<ComicBook> GetList()
        {
            return Context.ComicBooks
                // Toggle off Entity Tracking w AsNoTracking()
                    .AsNoTracking()
                    .Include(cb => cb.Series)
                    .OrderBy(cb => cb.Series.Title)
                    .ThenBy(cb => cb.IssueNumber)
                    .ToList();
        }

        public override ComicBook Get(int id, bool includeRelatedEntities = true)
        {
            //// EXPLICIT LOADING / Automatic Relationship Fix-up
            var comicBook = Context.ComicBooks
                    .Where(cb => cb.Id == id)
                    .SingleOrDefault();

            if (includeRelatedEntities)
            {
                Context.Series
                   .Where(s => s.Id == comicBook.SeriesId)
                   .Single();

                Context.ComicBookArtists
                    .Include(cba => cba.Artist)
                    .Include(cba => cba.Role)
                    .Where(cba => cba.ComicBookId == id)
                    .ToList();
                //var comicBookEntry = Context.Entry(comicBook);

                //comicBookEntry.Reference(cb => cb.Series).Load();
                //comicBookEntry.Collection(cb => cb.Artists)
                //    .Query()
                //    .Include(a => a.Artist)
                //    .Include(a => a.Role)
                //    .ToList();
            }

            return comicBook;


            ////ORIGINAL  Eagerly-loading Query Method 
            //var comicBooks = Context.ComicBooks.AsQueryable();

            //if (includeRelatedEntities)
            //{
            //    comicBooks = comicBooks
            //        .Include(cb => cb.Series)
            //        .Include(cb => cb.Artists.Select(a => a.Artist))
            //        .Include(cb => cb.Artists.Select(a => a.Role));
            //}

            //return comicBooks
            //    .Where(cb => cb.Id == id)
            //    .SingleOrDefault();
        }

        public void Delete(int id, byte[] rowVersion)
        {
            var comicBook = new ComicBook()
            {
                Id = id,
                RowVersion = rowVersion
            };
            Context.Entry(comicBook).State = EntityState.Deleted;
            Context.SaveChanges();
        }

        public bool ValidateDuplicateBook(ComicBook comicBook)
        {
            return Context.ComicBooks.Any(cb => cb.Id != comicBook.Id &&
                                            cb.SeriesId == comicBook.SeriesId &&
                                            cb.IssueNumber == comicBook.IssueNumber);
        }

        public bool ValidateDuplicateArtist(int ComicBookId, int ArtistId, int RoleId)
        {
            return Context.ComicBookArtists
                     .Any(cba => cba.ComicBookId == ComicBookId &&
                                 cba.ArtistId == ArtistId &&
                                 cba.RoleId == RoleId);
        }
    }
}
