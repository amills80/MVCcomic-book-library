using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicBookShared.Data
{
    public class ComicBooksRepository
    {
        private Context context = null;

        public ComicBooksRepository(Context context)
        {
            _context = context;
        }
    }
}
