using System;
using ComicBookShared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicBookShared.Data
{
    public class SeriesRepository : BaseRepository<ComicBook>
    {

        public SeriesRepository(Context context)
            : base(context)
        {
        }

        public override ComicBook Get(int id, bool includeRelatedEntities = true)
        {
            throw new NotImplementedException();
        }

        public override IList<ComicBook> GetList()
        {
            throw new NotImplementedException();
        }
    }
}
