﻿using System;
using System.Runtime.Caching;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicBookShared.Data
{
    static class EntityCache
    {
        private static MemoryCache _cache = MemoryCache.Default;

        public static void Add(string key, object item, int expireInMinutes = 60)
        {
            _cache.Add(key, item, DateTimeOffset.Now.AddMinutes(60)); 
        }

        public static T Get<T>(string key)
        {
            if (_cache.Contains(key))
            {
                return (T)_cache.Get(key);
            }
            else
            {
                return default(T);
            }
        }

        public static void Remove(string key)
        {
            if (_cache.Contains(key))
            {
                _cache.Remove(key);
            }
        }
    }
}
