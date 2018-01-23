using ComicBookShared.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicBookShared.Data
{
    public class ArtistsRepository : BaseRepository<Artist>
    {
        public ArtistsRepository(Context context)
            : base(context)
        {
        }

        public override Artist Get(int id, bool includeRelatedEntities = true)
        {
            //throw new NotImplementedException();
            var artist = Context.Artists.AsQueryable();
            if (includeRelatedEntities)
            {
                artist = artist
                    .Include(s => s.ComicBooks.Select(a => a.ComicBook.Series))
                    .Include(s => s.ComicBooks.Select(a => a.Role));
            }

            return artist
                .Where(a => a.Id == id)
                .SingleOrDefault();
        }

        public override IList<Artist> GetList()
        {
            //throw new NotImplementedException();
            return Context.Artists
                .OrderBy(a => a.Name)
                .ToList();
        }

        public bool ArtistExists(int id, string name)
        {
            //throw new NotImplementedException();
            return Context.Artists
                .Any(a => a.Id != id && a.Name == name);
        }
    }
}
