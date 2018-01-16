using System;
using ComicBookShared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ComicBookShared.Data
{
    public class SeriesRepository : BaseRepository<Series>
    {
        public SeriesRepository(Context context)
            : base(context)
        {
        }

        public override Series Get(int id, bool includeRelatedEntities = true)
        {
            var series = Context.Series.AsQueryable();
            if (includeRelatedEntities)
            {
                series = series
                    .Include(s => s.ComicBooks);
            }

            return series
                .Where(cb => cb.Id == id)
                .SingleOrDefault();
        }

        public override IList<Series> GetList()
        {
            return Context.Series
                .OrderBy(s => s.Title)
                .ToList();
        }

        public bool SeriesHasTitle(int seriesId, string title)
        {
            return Context.Series
                .Any(s => s.Id != seriesId && s.Title == title);
        }

        //public IList<Series> GetSeriesList()
        //{
        //    return _context.Series
        //            .OrderBy(s => s.Title)
        //            .ToList();
        //}

        //public Series GetSeriesById(int id)
        //{
        //    return _context.Series
        //            .Include(s => s.ComicBooks.Select(cb => cb.Series))
        //            .Where(s => s.Id == id)
        //            .SingleOrDefault();
        //}

        //public void AddSeries(Series series)
        //{
        //    _context.Series.Add(series);
        //    _context.SaveChanges();
        //}

        //public void DeleteSeries(int id)
        //{
        //    var set = _context.Set<Series>();
        //    var entity = set.Find(id);
        //    set.Remove(entity);
        //    _context.SaveChanges();
        //}
    }
}
