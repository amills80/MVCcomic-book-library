using ComicBookShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ComicBookShared.Data
{
    public class Repository
    {
        private Context _context = null;

        public Repository(Context context)
        {
            _context = context;
        }

        public IList<ComicBook> GetComicBooks()
        {
            return _context.ComicBooks
                    .Include(cb => cb.Series)
                    .OrderBy(cb => cb.Series.Title)
                    .ThenBy(cb => cb.IssueNumber)
                    .ToList();
        }

        public ComicBook GetComicBook(int id, bool includeRelatedEntities = true)
        {
            var comicBooks = _context.ComicBooks.AsQueryable();

            if (includeRelatedEntities)
            {
                comicBooks = comicBooks
                    .Include(cb => cb.Series)
                    .Include(cb => cb.Artists.Select(a => a.Artist))
                    .Include(cb => cb.Artists.Select(a => a.Role));
            }

            return comicBooks
                .Where(cb => cb.Id == id)
                .SingleOrDefault();
        }

        public void AddComicBook(ComicBook comicBook)
        {
            _context.ComicBooks.Add(comicBook);
            _context.SaveChanges();
        }

        public void UpdateComicBook(ComicBook comicBook)
        {
            _context.Entry(comicBook).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void DeleteComicBook(ComicBook comicBook)
        {
            _context.Entry(comicBook).State = EntityState.Deleted;
            _context.SaveChanges();
        }

        public bool ValidateDuplicateBook(ComicBook comicBook)
        {
            return _context.ComicBooks.Any(cb => cb.Id != comicBook.Id &&
                                            cb.SeriesId == comicBook.SeriesId &&
                                            cb.IssueNumber == comicBook.IssueNumber);
        }

        public void AddComicBookArtist(ComicBookArtist comicBookArtist)
        {
            _context.ComicBookArtists.Add(comicBookArtist);
            _context.SaveChanges();
        }

        public void DeleteComicBookArtist(int id)
        {
            var comicBookArtist = new ComicBookArtist() { Id = id };
            _context.Entry(comicBookArtist).State = EntityState.Deleted;
            _context.SaveChanges();
        }

        public bool ValidateDuplicateArtist(int ComicBookId, int ArtistId, int RoleId)
        {
           return _context.ComicBookArtists
                    .Any(cba => cba.ComicBookId == ComicBookId &&
                                cba.ArtistId == ArtistId &&
                                cba.RoleId == RoleId);
        }

        public IList<Series> GetSeriesList()
        {
            return _context.Series
                .OrderBy(s => s.Title)
                .ToList();
        }

        public IList<Artist> GetArtists()
        {
            return _context.Artists
                    .OrderBy(a => a.Name)
                    .ToList();
        }

        public IList<Role> GetRoles()
        {
            return _context.Roles
                .OrderBy(r => r.Name).ToList();
        }


   

        /*Code from edit : GetComicBook()
            return _context.ComicBooks
                .Where(cb => cb.Id == id)
                .SingleOrDefault(); 

        Code from delete : GetComicBook()
            return _context.ComicBooks
                .Include(cb => cb.Series)
                .Where(cb => cb.Id == id)
                .SingleOrDefault();

        From CBA Ctrl index()
            Context.ComicBooks
                .Include(cb => cb.Series)
                //.OrderBy(cb => cb.Series.Title)
                .Where(cb => cb.Id == comicBookId)
                .SingleOrDefault();
         */
    }
}
