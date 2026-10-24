using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace HomeWorkOtusProffesional.Classes
{
    public static class CollectExt
    {
        public static T GetMax<T>(this IEnumerable collection, Func<T, float> convertToNumber) where T : class
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (convertToNumber == null)
                throw new ArgumentNullException(nameof(convertToNumber));

            T maxItem = null;
            float maxValue = float.MinValue;
            bool found = false;

            foreach (object item in collection)
            {
                if (item is T typedItem)
                {
                    float value = convertToNumber(typedItem);
                    if (!found || value > maxValue)
                    {
                        maxValue = value;
                        maxItem = typedItem;
                        found = true;
                    }
                }
            }

            return maxItem;
        }
    }
}
