using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Extensions
{
	static class Random
	{
		public static T GetRandom<T>(this IReadOnlyList<T> source) => source[UnityEngine.Random.Range(0, source.Count)];

		public static T GetRandom<T>(this IEnumerable<T> source) => source.ElementAt(UnityEngine.Random.Range(0, source.Count()));

		/// <summary>
		/// Selects up to <paramref name="maxCount"/> unique random elements from <paramref name="source"/> and returns them to <paramref name="destination"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">Source to select random elements from.</param>
		/// <param name="destination">Destination to output selected elements to.</param>
		/// <param name="maxCount">Maximum number of elements to select.</param>
		public static void SelectRandom<T>(this IEnumerable<T> source, IList<T> destination, uint maxCount)
		{
			destination.Clear();
			var getRandomQuery = source.Except(destination);
			while (destination.Count < maxCount && getRandomQuery.Any())
				destination.Add(getRandomQuery.GetRandom());
		}

		public static T GetWeightedRandom<T>(this IEnumerable<T> source, Func<T, int> getWeight)
		{
			var random = UnityEngine.Random.Range(0, source.Sum(getWeight));
			int iterated = 0;
			foreach (var t in source)
			{
				iterated += getWeight(t);
				if (iterated > random)
					return t;
			}
			return source.Last();
		}

		public static T GetWeightedRandom<T>(this IEnumerable<T> source, Func<T, float> getWeight)
		{
			var random = UnityEngine.Random.Range(0, source.Sum(getWeight));
			float iterated = 0f;
			foreach (var t in source)
			{
				iterated += getWeight(t);
				if (iterated > random)
					return t;
			}
			return source.Last();
		}

		/// <param name="chance">Chance to return <see langword="true"/>.</param>
		/// <returns>A random <see langword="bool"/> with <paramref name="chance"/>% chance of being <see langword="true"/>.</returns>
		public static bool TryRoll(float chance)
		{
			return chance >= 1f || UnityEngine.Random.value <= chance;
		}

		/// <summary>
		/// Like <see cref="TryRoll(float)"/> with an inherent 50%.
		/// </summary>
		/// <returns>A random <see langword="bool"/> with 50% chance of being <see langword="true"/>.</returns>
		public static bool TryRoll()
		{
			return UnityEngine.Random.Range(0, 2) == 1;
		}

		public static float GetRandomPolarity(float multiplier = 1f)
		{
			return TryRoll() ? multiplier : -multiplier;
		}

		public static Vector2 GetRandomDirection2DRad(float rangeRadians)
		{
			return new(Mathf.Cos(rangeRadians), Mathf.Sin(rangeRadians));
		}

		public static Vector2 GetRandomDirection2DDeg(float rangeDegrees)
		{
			return GetRandomDirection2DRad(rangeDegrees * Mathf.Deg2Rad);
		}

		/// <param name="minValues">Bottom-Left corner of the bounds.</param>
		/// <param name="maxValues">Top-Right corner of the bounds.</param>
		/// <returns>A random point within the bounds.</returns>
		public static Vector2 GetRandomPositionInBounds(Vector2 minValues, Vector2 maxValues)
		{
			return new(UnityEngine.Random.Range(minValues.x, maxValues.x), UnityEngine.Random.Range(minValues.y, maxValues.y));
		}
	}
}
