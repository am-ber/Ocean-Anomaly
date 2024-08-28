using System.Collections;
using System;
using UnityEngine;

namespace OceanAnomaly.Tools
{
	public static class GlobalTools
	{
		/// <summary>
		/// Linearly maps a Value from a number scale [fromMin:fromMax] to a different number scale [toMin:toMax]
		/// </summary>
		/// <param name="value"></param>
		/// <param name="fromMin"></param>
		/// <param name="toMin"></param>
		/// <param name="fromMax"></param>
		/// <param name="toMax"></param>
		/// <returns></returns>
		public static float Map(float from, float fromMin, float fromMax, float toMin, float toMax)
		{
			var fromAbs = from - fromMin;
			var fromMaxAbs = fromMax - fromMin;

			var normal = fromAbs / fromMaxAbs;

			var toMaxAbs = toMax - toMin;
			var toAbs = toMaxAbs * normal;

			var to = toAbs + toMin;

			return to;
		}
		/// <summary>
		/// Starts a Coroutine with a desired delay.
		/// </summary>
		/// <param name="mb"></param>
		/// <param name="delay"></param>
		/// <param name="action"></param>
		public static void StartCoroutine(this MonoBehaviour mb, float delay, Action action, bool repeat = false)
		{
			mb.StartCoroutine(WaitForSeconds(delay, action, repeat));
		}
		/// <summary>
		/// Needed for the coroutine delay function.
		/// </summary>
		/// <param name="delay"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		private static IEnumerator WaitForSeconds(float delay, Action action, bool repeat = false)
		{
			while (repeat)
			{
				yield return new WaitForSeconds(delay);
				action();
			}
		}
		/// <summary>
		/// An extension for LayerMasks to allow the option to convert to a layer quickly.
		/// </summary>
		/// <param name="mask"></param>
		/// <returns></returns>
		public static int MaskToLayer(this LayerMask mask)
		{
			int layerNumber = 0;
			int layer = mask.value;
			while (layer > 0)
			{
				layer = layer >> 1;
				layerNumber++;
			}
			return layerNumber - 1;
		}
		public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
		{
			return Quaternion.Euler(angles) * (point - pivot) + pivot;
		}
		public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
		{
			return rotation * (point - pivot) + pivot;
		}
		/// <summary>
		/// A quick a dirty way to randomly pick something from a list of objects.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ts"></param>
		/// <returns></returns>
		public static T RandomPick<T>(params T[] ts)
		{
			return ts[UnityEngine.Random.Range(0, ts.Length)];
		}
		/// <summary>
		/// Finds the Centroid of a given list of Vector3's.
		/// </summary>
		/// <param name="points"></param>
		/// <returns></returns>
		public static Vector3 FindCentroid(params Vector3[] points)
		{
			// If we get junk, lets just leave
			if (points == null || points.Length <= 0)
				return Vector3.zero;
			// Take a bounds and encapsulate everything to get the center
			Bounds bounds = new Bounds(points[0], Vector3.zero);
			for (int i = 1; i < points.Length; i++)
			{
				bounds.Encapsulate(points[i]);
			}
			return bounds.center;
		}
		/// <summary>
		/// Finds the largest distance between the Centroid and all points given.
		/// </summary>
		/// <param name="points"></param>
		/// <param name="centroid"></param>
		/// <returns></returns>
		public static float FindLargestDistanceFromCentroid(Vector3[] points, Vector3 centroid)
		{
			if (points == null || points.Length <= 0 || centroid == null)
				return 0.0f;
			// Find the largest distance between the centroid and all other points
			float distance = 0.0f;
			for (int i = 0; i < points.Length; i++)
			{
				float tempDistance = Vector3.Distance(points[i], centroid);
				if (tempDistance > distance)
					distance = tempDistance;
			}
			return distance;
		}
		public static bool InRange(this float value, float min, float max)
		{
			return (value > min) && (value < max);
		}
		/// <summary>
		/// Used to grab a random point within a certain radius. You can also give it a center point if you want.
		/// </summary>
		/// <param name="radiusMax"></param>
		/// <param name="center"></param>
		/// <returns></returns>
		public static Vector3 GetRandomPointInRadius2D(float radiusMax, Vector3? center = null, float radiusMin = 0)
		{
			// Clean our arguments a bit
			if (center == null)
			{
				center = new Vector3();
			}
			if (radiusMax <= 0.0f)
			{
				return center.Value;
			}
			if (radiusMin >= radiusMax)
			{
				radiusMin = 0;
			}

			Vector3 randomPoint = UnityEngine.Random.insideUnitCircle.normalized * UnityEngine.Random.Range(radiusMin, radiusMax);

			return randomPoint + center.Value;
		}
		/// <summary>
		/// Recursively find a game object locally. If it doesn't exist it will make it if you give it a prefab object and repeat.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="gameObject"></param>
		/// <param name="prefab"></param>
		/// <returns></returns>
		public static T RecursiveFindComponentLocal<T>(this GameObject gameObject, GameObject prefab = null) where T : Component
		{
			T component = gameObject.GetComponent<T>();
			if (component != null)
			{
				return component;
			}
			component = gameObject.GetComponentInChildren<T>();
			if (component != null)
			{
				return component;
			}
			if (prefab != null)
			{
				return GameObject.Instantiate(prefab).RecursiveFindComponentLocal<T>();
			}
			return component;
		}
		/// <summary>
		/// Used to return a new Vector3 with a Z of 0.
		/// </summary>
		/// <param name="vector"></param>
		/// <returns></returns>
		public static Vector3 ToVector3(this Vector2 vector)
		{
			return new Vector3(vector.x, vector.y);
		}
	}
}