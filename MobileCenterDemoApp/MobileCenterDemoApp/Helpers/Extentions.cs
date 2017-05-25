using System;
using System.Linq;
using System.Collections.Generic;

namespace MobileCenterDemoApp.Helpers
{
    public static class Extentions
    {
        public static IEnumerable<T> RemoveLastElements<T>(this IEnumerable<T> collection, int removeCount = 1)
        {
            if(removeCount < 0)
            {
                throw new ArgumentException("Remove count must be more than 0");
            }

            T[] array = collection.ToArray();

            if(removeCount > array.Length)
            {
                throw new ArgumentException("Remove count must be less than collection count");   
            }

            return collection.Reverse().Skip(removeCount).Reverse();
        }
    }
}
