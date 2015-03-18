using System;
using System.Collections.Generic;
using System.Linq;
using Random = System.Random;

namespace Helpers
{
    public struct Tuple<T1, T2>
    {
        public readonly T1 Item1;
        public readonly T2 Item2;
        public Tuple(T1 first, T2 second)
        {
            Item1 = first;
            Item2 = second;
        }
    }

    public static class EnumerableFuncs
    {
        private static Random rand = new Random();

        private static int? ResolveIndex<T>(this IEnumerable<T> Arr, int? Num)
        {
            return Num.HasValue ? Num >= 0 ? Num : Arr.Count() + Num : null;
        }

        public static IEnumerable<T> Slice<T>(this IEnumerable<T> Arr, int? Start, int? End)
        {
            int ResStart = ResolveIndex(Arr, Start) ?? 0;
            int ResEnd = ResolveIndex(Arr, End) ?? Arr.Count();
            return Arr.Skip(ResStart).Take(ResEnd - ResStart);
        }

        public static void AddIf<T>(this IList<T> Lst, T Element, Func<T, bool> Checker)
        {
            if (Checker(Element))
                Lst.Add(Element);
        }

        public static void AddIfNotIn<T>(this IList<T> Lst, T Element, IEnumerable<T> ExclusionList)
        {
            Lst.AddIf(Element, E => !ExclusionList.Contains(E));
        }

        public static void AddIf<T>(this HashSet<T> Lst, T Element, Func<T, bool> Checker)
        {
            if (Checker(Element))
                Lst.Add(Element);
        }

        public static void AddIfNotIn<T>(this HashSet<T> Lst, T Element, IEnumerable<T> ExclusionList)
        {
            Lst.AddIf(Element, E => !ExclusionList.Contains(E));
        }

        public static T BoundedGet<T>(this IEnumerable<T> Arr, int Index)
        {
            return Arr.ElementAt(Math.Max(0, Math.Min(Arr.Count() - 1, Index)));
        }

        public static T RandomChoice<T>(this IEnumerable<T> Arr)
        {
            return Arr.ElementAt(rand.Next(Arr.Count()));
        }

		public static T[] toShuffledArray<T>(this ICollection<T> collection) {
			T[] arr = collection.ToArray ();
			int i = arr.ToArray().Length;	
			while (i-- > 1) {
				int swapIndex = UnityEngine.Random.Range(0, i);
				T tmp = arr[i];
				arr[i] = arr[swapIndex];
				arr[swapIndex] = tmp;
			}
			i = arr.ToArray().Length;	
			while (i-- > 1) {
				int swapIndex = UnityEngine.Random.Range(0, i);
				T tmp = arr[i];
				arr[i] = arr[swapIndex];
				arr[swapIndex] = tmp;
			}
			return arr;
		}
    }

}