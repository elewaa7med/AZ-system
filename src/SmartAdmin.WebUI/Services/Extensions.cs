using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SmartAdmin.WebUI.Services
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the minimum Double value 
        /// if the sequence is not empty; otherwise returns the specified default value. 
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The minimum value in the sequence or default value if sequence is empty.</returns>
        public static DateTime? MinOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, DateTime> selector)
        {
            if (source.Any())
                return source.Min(selector);
            return null;
        }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum Double value 
        /// if the sequence is not empty; otherwise returns the specified default value. 
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the sequence or default value if sequence is empty.</returns>
        public static DateTime? MaxOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, DateTime> selector)
        {
            if (source.Any())
                return source.Max(selector);

            return null;
        }
    }
}

