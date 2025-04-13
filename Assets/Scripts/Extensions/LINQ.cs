using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Extensions
{
	static class LINQ
	{
		public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> collection, Func<TSource, TKey> selector) where TKey : IComparable<TKey>
		{
			TSource min = collection.FirstOrDefault();
			if (min != null && !min.Equals(default(TSource)))
			{
				TKey minVal = selector(min);
				foreach (var item in collection)
				{
					TKey val = selector(item);
					if (val.CompareTo(minVal) < 0)
					{
						min = item;
						minVal = val;
					}
				}
			}
			return min;
		}

		public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> collection, Func<TSource, TKey> selector) where TKey : IComparable<TKey>
		{
			TSource max = collection.FirstOrDefault();
			if (max != null && !max.Equals(default(TSource)))
			{
				TKey minVal = selector(max);
				foreach (var item in collection)
				{
					TKey val = selector(item);
					if (val.CompareTo(minVal) > 0)
					{
						max = item;
						minVal = val;
					}
				}
			}
			return max;
		}

		/// <summary>
		/// Similar to LINQ's TakeWhile, but the <paramref name="predicate"/> is inverted and the failed element is also returned.
		/// </summary>
		public static IEnumerable<T> TakeUntil<T>(this IEnumerable<T> source, Func<T, bool> predicate)
		{
			foreach (T item in source)
			{
				yield return item;
				if (predicate(item))
					yield break;
			}
		}

		public static IEnumerable<T> Except<T>(this IEnumerable<T> source, T except) where T : class
		{
			foreach (T item in source)
			{
				if (!item.Equals(except))
					yield return item;
			}
		}

		public static Vector2 Average(this IEnumerable<Vector2> source)
		{
			uint count = 0;
			Vector2 sum = Vector2.zero;
			foreach (var item in source)
			{
				count++;
				sum += item;
			}
			if (count > 1)
				return sum / count;
			else
				return sum;
		}

		public static Vector3 Average(this IEnumerable<Vector3> source)
		{
			uint count = 0;
			Vector3 sum = Vector3.zero;
			foreach (var item in source)
			{
				count++;
				sum += item;
			}
			if (count == 0)
				return Vector3.zero;
			return sum / count;
		}

		public static IEnumerable<KeyValuePair<TKey, TValue>> ByKeys<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source, IEnumerable<TKey> keys) => source.Where(kvp => keys.Contains(kvp.Key));

		public static IEnumerable<T> NotNull<T>(this IEnumerable<T> source) where T : class => source.Where(o => o != null);

		public static IEnumerable<T> FindAllInstances<T>(FindObjectsInactive findObjectsInactive = FindObjectsInactive.Include, FindObjectsSortMode sortMode = FindObjectsSortMode.None, IncludePrefabs includePrefabs = IncludePrefabs.Both)
		{
			return FindAllScriptableObjects<T>(findObjectsInactive, sortMode).Concat(FindAllComponents<T>(findObjectsInactive, sortMode, includePrefabs));
		}

		public static IEnumerable<T> FindAllComponents<T>(FindObjectsInactive findObjectsInactive = FindObjectsInactive.Include, FindObjectsSortMode sortMode = FindObjectsSortMode.None, IncludePrefabs includePrefabs = IncludePrefabs.Both)
		{
			return UnityEngine.Object.FindObjectsByType<Component>(findObjectsInactive, sortMode).IncludePrefabsQuery(includePrefabs).OfType<T>();
		}

		public static IEnumerable<T> FindAllScriptableObjects<T>(FindObjectsInactive findObjectsInactive = FindObjectsInactive.Include, FindObjectsSortMode sortMode = FindObjectsSortMode.None)
		{
			return UnityEngine.Object.FindObjectsByType<ScriptableObject>(findObjectsInactive, sortMode).OfType<T>();
		}

		public static IEnumerable<UnityEngine.GameObject> IncludePrefabsQuery(this IEnumerable<UnityEngine.GameObject> source, IncludePrefabs includePrefabs)
		{
			return includePrefabs switch
			{
				IncludePrefabs.OnlyPrefabs => source.Where(go => go.IsPrefab()),
				IncludePrefabs.ExcludePrefabs => source.Where(go => !go.IsPrefab()),
				_ => source
			};
		}

		public static IEnumerable<Component> IncludePrefabsQuery(this IEnumerable<Component> source, IncludePrefabs includePrefabs)
		{
			return includePrefabs switch
			{
				IncludePrefabs.OnlyPrefabs => source.Where(c => c.gameObject.IsPrefab()),
				IncludePrefabs.ExcludePrefabs => source.Where(c => !c.gameObject.IsPrefab()),
				_ => source
			};
		}

        public enum IncludePrefabs : byte
		{
			Both = 0,
			OnlyPrefabs,
			ExcludePrefabs
		}
	}
}
