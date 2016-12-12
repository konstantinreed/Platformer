﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameLibrary
{
	/// <summary>
	/// Математические функции для чисел типа float (аналогично классу Math)
	/// </summary>
	public static class Mathf
	{
		public static Random RandomGenerator = new Random();

		public const float ZeroTolerance = 1e-6f; // Value a 8x higher than 1.19209290E-07F

		/// <summary>
		/// Возвращает число Пи
		/// </summary>
		public const float Pi = 3.141592653f;

		/// <summary>
		/// Возвращает Пи * 2
		/// </summary>
		public const float TwoPi = 2 * 3.141592653f;

		/// <summary>
		/// Возвращает Пи / 2
		/// </summary>
		public const float HalfPi = 3.141592653f / 2;

		/// <summary>
		/// Для перевода градусов в радианы умножайте на это число
		/// </summary>
		public const float DegToRad = Pi / 180;

		/// <summary>
		/// Для перевода радиан в градусы умножайте на это число
		/// </summary>
		public const float RadToDeg = 180 / Pi;

		public static float Max(float x, float y)
		{
			return (x > y) ? x : y;
		}

		public static float Min(float x, float y)
		{
			return (x < y) ? x : y;
		}

		public static float Abs(float x)
		{
			return Math.Abs(x);
		}

		public static float Abs(Vector2 x)
		{
			return x.Length();
		}

		public static float Abs(Vector3 x)
		{
			return x.Length();
		}

		public static int Sign(float x)
		{
			return Math.Sign(x);
		}

		public static float Cos(float radians)
		{
			return (float)Math.Cos(radians);
		}

		public static float Sin(float radians)
		{
			return (float)Math.Sin(radians);
		}

		public static float Atan2(Vector2 v)
		{
			return (float)Math.Atan2(v.Y, v.X);
		}

		public static int Wrap(int x, int lowerBound, int upperBound)
		{
			int range = upperBound - lowerBound + 1;
			x = ((x - lowerBound) % range);
			return x < 0 ? upperBound + 1 + x : lowerBound + x;
		}

		public static float Wrap(float x, float lowerBound, float upperBound)
		{
			if (x < lowerBound) {
				return upperBound - (lowerBound - x) % (upperBound - lowerBound);
			}
			return lowerBound + (x - lowerBound) % (upperBound - lowerBound);
		}

		public static float Wrap360(float angle)
		{
			if ((angle >= 360.0f) || (angle < 0.0f)) {
				angle -= (float)Math.Floor(angle * (1.0f / 360.0f)) * 360.0f;
			}
			return angle;
		}

		public static float WrapRadians(float radians)
		{
			radians = (radians + Mathf.Pi) % Mathf.TwoPi;
			return (radians < 0f ? radians + Mathf.TwoPi : radians) - Mathf.Pi;
		}

		public static float Sqr(float x)
		{
			return x * x;
		}

		public static float Sqrt(float x)
		{
			return (float)Math.Sqrt(x);
		}

		public static float Dist2(Vector2 a, Vector2 b)
		{
			return Sqr(a.X - b.X) + Sqr(a.Y - b.Y);
		}

		public static float Log(float x)
		{
			return (float)Math.Log(x);
		}

		public static float Pow(float x, float y)
		{
			return (float)Math.Pow(x, y);
		}

		public static float Lerp(float amount, float value1, float value2)
		{
			return value1 + (value2 - value1) * amount;
		}

		public static Vector2 Lerp(float amount, Vector2 value1, Vector2 value2)
		{
			return value1 + (value2 - value1) * amount;
		}

		public static Vector3 Lerp(float amount, Vector3 value1, Vector3 value2)
		{
			return value1 + (value2 - value1) * amount;
		}

		public static float RandomFloat(this Random rng, float min, float max)
		{
			return rng.RandomFloat() * (max - min) + min;
		}

		public static float RandomFloat(float min, float max)
		{
			return RandomGenerator.RandomFloat(min, max);
		}

		public static bool RandomBool(this Random rng)
		{
			return rng.RandomInt(2) == 0;
		}

		public static bool RandomBool()
		{
			return RandomGenerator.RandomBool();
		}

		public static int RandomInt(this Random rng, int min, int max)
		{
			return rng.RandomInt(max - min + 1) + min;
		}

		public static int RandomInt(int min, int max)
		{
			return RandomGenerator.RandomInt(min, max);
		}

		public static T RandomOf<T>(this Random rng, params T[] objects)
		{
			return objects[rng.RandomInt(objects.Length)];
		}

		public static T RandomOf<T>(params T[] objects)
		{
			return RandomGenerator.RandomOf(objects);
		}

		/// <summary>
		/// Перечисляет элементы коллекции в случайном порядке
		/// </summary>
		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng = null)
		{
			if (rng == null)
				rng = RandomGenerator;
			var elements = source.ToArray();
			for (int i = elements.Length; i > 0; i--) {
				int j = rng.Next(i);
				yield return elements[j];
				elements[j] = elements[i - 1];
			}
		}

		public static int RandomInt(this Random rng, int maxValue)
		{
			return rng.Next(maxValue);
		}

		public static int RandomInt(int maxValue)
		{
			return RandomGenerator.RandomInt(maxValue);
		}

		public static float RandomFloat(this Random rng)
		{
			return (float)rng.NextDouble();
		}

		public static float RandomFloat()
		{
			return RandomGenerator.RandomFloat();
		}

		public static float NormalRandom(this Random rng, float median, float dispersion)
		{
			if (dispersion == 0.0f) {
				return median;
			}
			return median + dispersion *
				Sqrt(-2.0f * Log(rng.RandomFloat())) *
				Sin(2.0f * Pi * rng.RandomFloat());
		}

		public static float NormalRandom(float median, float dispersion)
		{
			return RandomGenerator.NormalRandom(median, dispersion);
		}

		public static float UniformRandom(this Random rng, float median, float dispersion)
		{
			return median + (rng.RandomFloat() - 0.5f) * dispersion;
		}

		public static float UniformRandom(float median, float dispersion)
		{
			return RandomGenerator.UniformRandom(median, dispersion);
		}

		public static bool InRange(float x, float upper, float lower)
		{
			return lower <= x && x <= upper;
		}

		public static float Clamp(float value, float min, float max)
		{
			return (value < min) ? min : (value > max ? max : value);
		}

		public static int Clamp(int value, int min, int max)
		{
			return (value < min) ? min : (value > max ? max : value);
		}

		public static Vector2 HermiteSpline(float t, Vector2 p0, Vector2 m0, Vector2 p1, Vector2 m1)
		{
			return new Vector2(
				HermiteSpline(t, p0.X, m0.X, p1.X, m1.X),
				HermiteSpline(t, p0.Y, m0.Y, p1.Y, m1.Y));
		}

		public static Vector3 HermiteSpline(float t, Vector3 p0, Vector3 m0, Vector3 p1, Vector3 m1)
		{
			return new Vector3(
				HermiteSpline(t, p0.X, m0.X, p1.X, m1.X),
				HermiteSpline(t, p0.Y, m0.Y, p1.Y, m1.Y),
				HermiteSpline(t, p0.Z, m0.Z, p1.Z, m1.Z));
		}

		public static float HermiteSpline(float t, float p0, float m0, float p1, float m1)
		{
			float t2 = t * t;
			float t3 = t2 * t;
			return (2.0f * t3 - 3.0f * t2 + 1.0f) * p0 + (t3 - 2.0f * t2 + t) * m0 +
				(-2.0f * t3 + 3.0f * t2) * p1 + (t3 - t2) * m1;
		}

		public static Vector2 CatmullRomSpline(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
		{
			return new Vector2(
				CatmullRomSpline(t, p0.X, p1.X, p2.X, p3.X),
				CatmullRomSpline(t, p0.Y, p1.Y, p2.Y, p3.Y)
			);
		}

		public static Vector3 CatmullRomSpline(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
		{
			return new Vector3(
				CatmullRomSpline(t, p0.X, p1.X, p2.X, p3.X),
				CatmullRomSpline(t, p0.Y, p1.Y, p2.Y, p3.Y),
				CatmullRomSpline(t, p0.Z, p1.Z, p2.Z, p3.Z)
			);
		}

		public static float CatmullRomSpline(float t, float p0, float p1, float p2, float p3)
		{
			float t2 = t * t;
			float t3 = t2 * t;
			return p1 + 0.5f * (
				(p2 - p0) * t +
				(2.0f * p0 - 5.0f * p1 + 4.0f * p2 - p3) * t2 +
				(3.0f * p1 - p0 - 3.0f * p2 + p3) * t3);
		}

		public static Vector3 BezierSpline(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
		{
			return new Vector3 {
				X = BezierSpline(t, p0.X, p1.X, p2.X, p3.X),
				Y = BezierSpline(t, p0.Y, p1.Y, p2.Y, p3.Y),
				Z = BezierSpline(t, p0.Z, p1.Z, p2.Z, p3.Z),
			};
		}

		public static Vector2 BezierSpline(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
		{
			return new Vector2 {
				X = BezierSpline(t, p0.X, p1.X, p2.X, p3.X),
				Y = BezierSpline(t, p0.Y, p1.Y, p2.Y, p3.Y)
			};
		}

		public static float BezierSpline(float t, float p0, float p1, float p2, float p3)
		{
			var oneMinusT = 1 - t;
			var oneMinusT2 = oneMinusT * oneMinusT;
			var oneMinusT3 = oneMinusT2 * oneMinusT;
			var t2 = t * t;
			var t3 = t2 * t;
			return oneMinusT3 * p0 + 3 * t * oneMinusT2 * p1 + 3 * t2 * oneMinusT * p2 + t3 * p3;
		}

		public static Vector3 BezierTangent(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
		{
			return new Vector3(
				BezierTangent(t, p0.X, p1.X, p2.X, p3.X),
				BezierTangent(t, p0.Y, p1.Y, p2.Y, p3.Y),
				BezierTangent(t, p0.Z, p1.Z, p2.Z, p3.Z));
		}

		public static Vector2 BezierTangent(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
		{
			return new Vector2(
				BezierTangent(t, p0.X, p1.X, p2.X, p3.X),
				BezierTangent(t, p0.Y, p1.Y, p2.Y, p3.Y));
		}

		public static float BezierTangent(float t, float p0, float p1, float p2, float p3)
		{
			var oneMinusT = 1 - t;
			var oneMinusT2 = oneMinusT * oneMinusT;
			var t2 = t * t;
			return
				-3 * oneMinusT2 * p0 +
				3 * oneMinusT2 * p1 -
				6 * t * oneMinusT * p1 -
				3 * t2 * p2 +
				6 * t * oneMinusT * p2 +
				3 * t2 * p3;
		}
	}
}
