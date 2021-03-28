using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Extensions
{
    public static class CollectionExtensions
    {
        public static TValue TryReturnValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.ContainsKey(key))
                return dictionary[key];

            return default(TValue);
        }

        public static TValue TryReturnValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            try
            {
                if (dictionary.ContainsKey(key))
                    return dictionary[key];

                return defaultValue;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static void AddOrUpdateValue<TKey, TValue>(this IDictionary<TKey, TValue> map, TKey key, TValue value)
        {
            map[key] = value;
        }
    }
}
