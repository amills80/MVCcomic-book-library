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

        public Series GetSeriesById(int id)
        {
            return _context.Series
                    .Include(s => s.ComicBooks.Select(cb => cb.Series))
                    .Where(s => s.Id == id)
                    .SingleOrDefault();
        }

        public void AddSeries(Series series)
        {
            _context.Series.Add(series);
            _context.SaveChanges();
        }

        // TODO Check to see if this can be removed
        //public void SaveChanges()
        //{
        //    throw new NotImplementedException();
        //}

        public void DeleteSeries(int id)
        {
            var set = _context.Set<Series>();
            var entity = set.Find(id);
            set.Remove(entity);
            _context.SaveChanges();
        }
    }
}
