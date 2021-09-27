using System;
using System.Collections.Generic;

namespace Robot
{
    public static class Utils
    {
        public static void AddRange<T, S>(this IDictionary<T, S> source, IDictionary<T, S> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection), "Collection is null");
            }

            foreach (var item in collection)
            {
                if (!source.ContainsKey(item.Key))
                {
                    source.Add(item.Key, item.Value);
                }
                else
                {
                    // handle duplicate key issue here
                }
            }
        }
    }
}
