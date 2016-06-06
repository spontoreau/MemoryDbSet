using System;
using Newtonsoft.Json;

namespace MemoryDbSet
{
    public static class ObjectExtension
    {
        public static T Clone<T>(this T obj, Action<T> callback = null) where T : class
        {
            var clone = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));

            if(callback != null)
                callback(clone);

            return clone;
        }
    }
}

