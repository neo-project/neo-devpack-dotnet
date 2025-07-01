// Copyright (C) 2015-2025 The Neo Project.
//
// LinqExtensions.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.SmartContract.Framework.Linq
{
    public static class LinqExtensions
    {
        private static void AssertSourceNotNull<T>(this IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException("source is null");
        }

        private static void AssertNotNull(object obj, string name)
        {
            if (obj == null) throw new ArgumentNullException($"{name} is null");
        }

        /// <summary>
        ///  Determines whether a sequence is null or contains any elements.
        /// </summary>
        /// <typeparam name="T">Type of the enumerable value</typeparam>
        /// <param name="source">Enumerable source</param>
        /// <returns>True if <see cref="source"/> is null or empty</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

        /// <summary>
        /// Applies an accumulator function over a sequence. The specified seed value is used as the initial accumulator value.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TAccumulate"></typeparam>
        /// <param name="source">An collections to aggregate over.</param>
        /// <param name="seed">The initial accumulator value.</param>
        /// <param name="func">An accumulator function to be invoked on each element.</param>
        /// <returns>The final accumulator value.</returns>
        /// <exception cref="ArgumentNullException">source or func is null.</exception>
        public static TAccumulate Aggregate<TSource, TAccumulate>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
        {
            AssertSourceNotNull(source);
            AssertNotNull(func, nameof(func));
            foreach (var item in source)
            {
                seed = func(seed, item);
            }
            return seed;
        }

        /// <summary>
        ///  Determines whether all elements of a sequence satisfy a condition.
        /// </summary>
        /// <typeparam name="T">An collections that contains the elements to apply the predicate to.</typeparam>
        /// <param name="source"> An collections that contains the elements to apply the predicate to.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns> true if every element of the source sequence passes the test in the specified predicate, or if the sequence is empty; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">source or predicate is null.</exception>
        public static bool All<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            AssertSourceNotNull(source);
            AssertNotNull(predicate, nameof(predicate));
            foreach (var i in source)
            {
                if (!predicate(i)) return false;
            }
            return true;
        }

        /// <summary>
        /// Determines whether a sequence contains any elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"> An collections that contains the elements to apply the predicate to.</param>
        /// <returns>true if the source sequence contains any elements; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">source is null.</exception>
        public static bool Any<T>(this IEnumerable<T> source)
        {
            AssertSourceNotNull(source);
            foreach (var i in source)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        ///  Determines whether any element of a sequence satisfies a condition.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"> An collections that contains the elements to apply the predicate to.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>true if the source sequence is not empty and at least one of its elements passes the test in the specified predicate; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">source or predicate is null.</exception>
        public static bool Any<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            AssertSourceNotNull(source);
            AssertNotNull(predicate, nameof(predicate));
            foreach (var i in source)
            {
                if (predicate(i)) return true;
            }
            return false;
        }

        /// <summary>
        ///  Computes the average of a sequence of System.Int32 values.
        /// </summary>
        /// <param name="source">A sequence of values to calculate the average of.</param>
        /// <returns>The average of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null.</exception>
        public static int Average(this IEnumerable<int> source)
        {
            AssertSourceNotNull(source);
            int count = 0;
            int sum = 0;
            foreach (var item in source)
            {
                count++;
                sum += item;
            }
            if (Helper.NumEqual(count, 0)) throw new Exception("source is empty");
            return sum / count;
        }

        /// <summary>
        ///  Computes the average of a sequence of System.Int32 values that are obtained by invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">A sequence of values to calculate the average of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The average of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null.</exception>
        public static int Average<T>(this IEnumerable<T> source, Func<T, int> selector)
        {
            AssertSourceNotNull(source);
            AssertNotNull(selector, nameof(selector));
            int count = 0;
            int sum = 0;
            foreach (var item in source)
            {
                count++;
                sum += selector(item);
            }
            if (Helper.NumEqual(count, 0)) throw new Exception("source is empty");
            return sum / count;
        }

        /// <summary>
        ///  Computes the average of a sequence of System.Int64 values.
        /// </summary>
        /// <param name="source">A sequence of values to calculate the average of.</param>
        /// <returns>The average of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null.</exception>
        public static long Average(this IEnumerable<long> source)
        {
            AssertSourceNotNull(source);
            int count = 0;
            long sum = 0;
            foreach (var item in source)
            {
                count++;
                sum += item;
            }
            if (Helper.NumEqual(count, 0)) throw new Exception("source is empty");
            return sum / count;
        }

        /// <summary>
        ///  Computes the average of a sequence of System.Int64 values that are obtained by invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">A sequence of values to calculate the average of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The average of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null.</exception>
        public static long Average<T>(this IEnumerable<T> source, Func<T, long> selector)
        {
            AssertSourceNotNull(source);
            AssertNotNull(selector, nameof(selector));
            int count = 0;
            long sum = 0;
            foreach (var item in source)
            {
                count++;
                sum += selector(item);
            }
            if (Helper.NumEqual(count, 0)) throw new Exception("source is empty");
            return sum / count;
        }

        /// <summary>
        ///  Computes the average of a sequence of BigInteger values.
        /// </summary>
        /// <param name="source">A sequence of values to calculate the average of.</param>
        /// <returns>The average of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null.</exception>
        public static BigInteger Average(this IEnumerable<BigInteger> source)
        {
            AssertSourceNotNull(source);
            int count = 0;
            BigInteger sum = 0;
            foreach (var item in source)
            {
                count++;
                sum += item;
            }
            if (Helper.NumEqual(count, 0)) throw new Exception("source is empty");
            return sum / count;
        }

        /// <summary>
        ///  Computes the average of a sequence of BigInteger values that are obtained by invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">A sequence of values to calculate the average of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The average of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null.</exception>
        public static BigInteger Average<T>(this IEnumerable<T> source, Func<T, BigInteger> selector)
        {
            AssertSourceNotNull(source);
            AssertNotNull(selector, nameof(selector));
            int count = 0;
            BigInteger sum = 0;
            foreach (var item in source)
            {
                count++;
                sum += selector(item);
            }
            if (Helper.NumEqual(count, 0)) throw new Exception("source is empty");
            return sum / count;
        }

        /// <summary>
        /// Determines whether a sequence contains a specified element by using the default equality comparer.(Reference comparer for class, Value comparer for struct)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">A sequence in which to locate a value.</param>
        /// <param name="value"> The value to locate in the sequence.</param>
        /// <returns>true if the source sequence contains an element that has the specified value; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">source is null.</exception>
        public static bool Contains<T>(this IEnumerable<T> source, T value)
        {
            return Any(source, s => s!.Equals(value));
        }

        /// <summary>
        /// Returns the number of elements in a sequence.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">A sequence that contains elements to be counted.</param>
        /// <returns>The number of elements in the input sequence.</returns>
        /// <exception cref="ArgumentNullException">source is null.</exception>
        public static int Count<T>(this IEnumerable<T> source)
        {
            AssertSourceNotNull(source);
            int count = 0;
            foreach (var item in source)
            {
                count++;
            }
            return count;
        }

        /// <summary>
        /// Returns a number that represents how many elements in the specified sequence satisfy a condition.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">A sequence that contains elements to be counted.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>A number that represents how many elements in the sequence satisfy the condition in the predicate function.</returns>
        /// <exception cref="ArgumentNullException">source or predicate is null.</exception>
        public static int Count<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            AssertSourceNotNull(source);
            AssertNotNull(predicate, nameof(predicate));
            int count = 0;
            foreach (var item in source)
            {
                if (predicate(item)) count++;
            }
            return count;
        }

        /// <summary>
        /// Returns the first element of the sequence that satisfies a condition, or a default value if the sequence contains no elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The collections to return the first element of.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="defaultValue">The default value to return if the sequence is empty.</param>
        /// <returns>defaultValue if source is empty or if no element passes the test specified by predicate; otherwise, the first element in source that passes the test specified by predicate.</returns>
        /// <exception cref="ArgumentNullException">source or predicate is null.</exception>
        public static T FirstOrDefault<T>(this IEnumerable<T> source, Predicate<T> predicate, T defaultValue)
        {
            AssertSourceNotNull(source);
            AssertNotNull(predicate, nameof(predicate));
            foreach (var item in source)
            {
                if (predicate(item)) return item;
            }
            return defaultValue;
        }

        /// <summary>
        /// Projects each element of a sequence into a new form.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"> A sequence of values to invoke a transform function on.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An collection whose elements are the result of invoking the transform function on each element of source.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null.</exception>
        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            AssertSourceNotNull(source);
            AssertNotNull(selector, nameof(selector));
            var list = new List<TResult>();
            foreach (var item in source)
            {
                list.Add(selector(item));
            }
            return list;
        }

        /// <summary>
        ///  Bypasses a specified number of elements in a sequence and then returns the remaining elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">An collections to return elements from.</param>
        /// <param name="count">The number of elements to skip before returning the remaining elements.</param>
        /// <returns> An collection that contains the elements that occur after the specified index in the input sequence.</returns>
        /// <exception cref="ArgumentNullException">source is null.</exception>
        public static IEnumerable<T> Skip<T>(this IEnumerable<T> source, int count)
        {
            AssertSourceNotNull(source);
            var list = new List<T>();
            foreach (var item in source)
            {
                if (count > 0)
                {
                    count--;
                }
                else
                {
                    list.Add(item);
                }
            }
            return list;
        }

        /// <summary>
        ///  Computes the sum of a sequence of System.Int32 values.
        /// </summary>
        /// <param name="source">A sequence of System.Int32 values to calculate the sum of.</param>
        /// <returns>The sum of the values in the sequence.</returns>
        /// <exception cref="ArgumentNullException">source is null.</exception>
        public static int Sum(this IEnumerable<int> source)
        {
            AssertSourceNotNull(source);
            int sum = 0;
            foreach (var item in source)
            {
                sum += item;
            }
            return sum;
        }

        /// <summary>
        /// Computes the sum of the sequence of System.Int32 values that are obtained by invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <param name="source">A sequence of values that are used to calculate a sum.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The sum of the projected values.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null.</exception>
        public static int Sum<T>(this IEnumerable<T> source, Func<T, int> selector)
        {
            AssertSourceNotNull(source);
            AssertNotNull(selector, nameof(selector));
            int sum = 0;
            foreach (var item in source)
            {
                sum += selector(item);
            }
            return sum;
        }

        /// <summary>
        ///  Computes the sum of a sequence of System.Int64 values.
        /// </summary>
        /// <param name="source">A sequence of System.Int64 values to calculate the sum of.</param>
        /// <returns>The sum of the values in the sequence.</returns>
        /// <exception cref="ArgumentNullException">source is null.</exception>
        public static long Sum(this IEnumerable<long> source)
        {
            AssertSourceNotNull(source);
            long sum = 0;
            foreach (var item in source)
            {
                sum += item;
            }
            return sum;
        }

        /// <summary>
        /// Computes the sum of the sequence of System.Int64 values that are obtained by invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <param name="source">A sequence of values that are used to calculate a sum.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The sum of the projected values.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null.</exception>
        public static long Sum<T>(this IEnumerable<T> source, Func<T, long> selector)
        {
            AssertSourceNotNull(source);
            AssertNotNull(selector, nameof(selector));
            long sum = 0;
            foreach (var item in source)
            {
                sum += selector(item);
            }
            return sum;
        }

        /// <summary>
        ///  Computes the sum of a sequence of BigInteger values.
        /// </summary>
        /// <param name="source">A sequence of BigInteger values to calculate the sum of.</param>
        /// <returns>The sum of the values in the sequence.</returns>
        /// <exception cref="ArgumentNullException">source is null.</exception>
        public static BigInteger Sum(this IEnumerable<BigInteger> source)
        {
            AssertSourceNotNull(source);
            BigInteger sum = 0;
            foreach (var item in source)
            {
                sum += item;
            }
            return sum;
        }

        /// <summary>
        /// Computes the sum of the sequence of BigInteger values that are obtained by invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <param name="source">A sequence of values that are used to calculate a sum.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The sum of the projected values.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null.</exception>
        public static BigInteger Sum<T>(this IEnumerable<T> source, Func<T, BigInteger> selector)
        {
            AssertSourceNotNull(source);
            AssertNotNull(selector, nameof(selector));
            BigInteger sum = 0;
            foreach (var item in source)
            {
                sum += selector(item);
            }
            return sum;
        }

        /// <summary>
        /// Returns a specified number of contiguous elements from the start of a sequence.
        /// </summary>
        /// <param name="source">The sequence to return elements from.</param>
        /// <param name="count">The number of elements to return.</param>
        /// <returns>An collection that contains the specified number of elements from the start of the input sequence.</returns>
        /// <exception cref="ArgumentNullException">source is null.</exception>
        public static IEnumerable<T> Take<T>(this IEnumerable<T> source, int count)
        {
            AssertSourceNotNull(source);
            var list = new List<T>();
            foreach (var i in source)
            {
                if (count <= 0) break;
                list.Add(i);
                count--;
            }
            return list;
        }

        /// <summary>
        ///  Creates a Map<Tkey,TValue> from a collection according to specified key selector and element selector functions.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source">A collection to create a Map from.</param>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <param name="elementSelector"> A transform function to produce a result element value from each element.</param>
        /// <returns> A Map<TKey,TValue> that contains values of type TElement selected from the input sequence.</returns>
        /// <exception cref="ArgumentNullException">source or keySelector or elementSelector is null.</exception>
        public static Map<TKey, TValue> ToMap<TSource, TKey, TValue>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> elementSelector)
        {
            AssertSourceNotNull(source);
            AssertNotNull(keySelector, nameof(keySelector));
            AssertNotNull(elementSelector, nameof(elementSelector));
            var map = new Map<TKey, TValue>();
            foreach (var item in source)
            {
                map[keySelector(item)] = elementSelector(item);
            }
            return map;
        }


        /// <summary>
        ///  Filters a sequence of values based on a predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">An collections to filter.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>An collection that contains elements from the input sequence that satisfy the condition.</returns>
        ///<exception cref="ArgumentNullException">source or predicate is null.</exception>
        public static IEnumerable<T> Where<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            AssertSourceNotNull(source);
            AssertNotNull(predicate, nameof(predicate));
            var list = new List<T>();
            foreach (var i in source)
            {
                if (predicate(i)) { list.Add(i); }
            }
            return list;
        }
    }
}
