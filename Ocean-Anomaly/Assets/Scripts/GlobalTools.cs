using System.Collections;
using System;
using UnityEngine;

namespace OceanAnomaly.Tools
{
	public static class GlobalTools
	{
		/// <summary>
		/// Linearly maps a Value from a number scale (from1-from2) to a different number scale (to1-to2)
		/// </summary>
		/// <param name="value"></param>
		/// <param name="from1"></param>
		/// <param name="to1"></param>
		/// <param name="from2"></param>
		/// <param name="to2"></param>
		/// <returns></returns>
		public static float Remap(float value, float from1, float from2, float to1, float to2)
		{
			return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
		}
		/// <summary>
		/// Starts a Coroutine with a desired delay.
		/// </summary>
		/// <param name="mb"></param>
		/// <param name="delay"></param>
		/// <param name="action"></param>
		public static void StartCoroutine(this MonoBehaviour mb, float delay, Action action)
		{
			mb.StartCoroutine(WaitForSeconds(delay, action));
		}
		/// <summary>
		/// Needed for the coroutine delay function.
		/// </summary>
		/// <param name="delay"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		private static IEnumerator WaitForSeconds(float delay, Action action)
		{
			while (true)
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
	}
}