using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using OceanAnomaly.Components;
using System.Linq;

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
		public static T RecursiveFindComponentLocal<T>(this GameObject gameObject, GameObject prefab = null, bool createComponentLocally = false, bool createChildComponentLocally = false) where T : Component
		{
			// Checks locally for the component
			T component = gameObject.GetComponent<T>();
			if (component != null)
			{
				return component;
			}
			// Checks the children for the component
			component = gameObject.GetComponentInChildren<T>();
			if (component != null)
			{
				return component;
			}
			// Should we make a new GameObject that hopefully has that component?
			if (prefab != null)
			{
				return GameObject.Instantiate(prefab).RecursiveFindComponentLocal<T>();
			}
			// Should we add a new instance of the component to this GameObject?
			if (createComponentLocally)
			{
				return gameObject.AddComponent<T>();
			}
			if (createChildComponentLocally)
			{
				return new GameObject($"{gameObject.name} {typeof(T).Name}").AddComponent<T>();
			}
			// If none of these then we might as well return null, but returning a mutated null seems cool ;)
			return component;
		}
		/// <summary>
		/// Used to return a new Vector3 with this Vector2's X and Y but a Z of 0 by default.
		/// </summary>
		/// <param name="vector">The Vector2!</param>
		/// <param name="z">Any z value you wish to put in the Vector3.</param>
		/// <returns></returns>
		public static Vector3 ToVector3(this Vector2 vector, float z = 0f)
		{
			return new Vector3(vector.x, vector.y, z);
		}
		/// <summary>
		/// Used to return a new Vector2 with this Vector3's X and Y.
		/// </summary>
		/// <param name="vector">The Vector3!</param>
		/// <returns></returns>
		public static Vector2 ToVector2(this Vector3 vector)
		{
			return new Vector2(vector.x, vector.y);
		}
		/// <summary>
		/// Used to find the first child by a tag. Will be obviously null if you don't got a child with that tag or component.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="component"></param>
		/// <param name="tag"></param>
		/// <returns></returns>
		public static T FindChildByTag<T>(this T component, string tag) where T : Component
		{
			foreach (T foundComp in component.GetComponentsInChildren<T>())
			{
				if (foundComp.tag == tag)
				{
					return foundComp;
				}
			}
			return null;
		}
		/// <summary>
		/// Used to find ALL the childeren by a tag. Will be obviously null if you don't got a child with that tag or component.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="component"></param>
		/// <param name="tag"></param>
		/// <returns></returns>
		public static T[] FindChilderenByTag<T>(this T component, string tag) where T : Component
		{
			List<T> foundChilderen = new List<T>();
			foreach (T foundComponent in component.GetComponentsInChildren<T>())
			{
                if (foundComponent.tag == tag)
                {
                    foundChilderen.Add(foundComponent);
                }
            }
			return foundChilderen.ToArray();
		}
		/// <summary>
		/// Used to grab a component that exisists in a child, if it doesn't exist there then we will create it.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="monoBehaviour"></param>
		/// <param name="newChildName"></param>
		/// <returns></returns>
		public static T GetOrMakeComponentInChilderen<T>(this MonoBehaviour monoBehaviour, string newChildName = null) where T : Component
		{
			// Search for it in the children
			T foundComponent = monoBehaviour.gameObject.GetComponentInChildren<T>();
			// If we can't find it lets make it
			if (foundComponent == null)
			{
				// If a name isn't given lets set the name
				if (newChildName == null)
				{
					newChildName = $"{monoBehaviour.name} {typeof(T).Name}";
				}
				foundComponent = new GameObject(newChildName).AddComponent<T>();
				// Set the parent to the monoBehaviour that called this
				foundComponent.transform.parent = monoBehaviour.transform;
			}
			return foundComponent;
		}
		public static Vector3 RoundVector(this Vector3 vector, int decimals = 0)
		{
			return new Vector3((float) Math.Round(vector.x, decimals), (float) Math.Round(vector.y, decimals), (float) Math.Round(vector.z, decimals));
		}
		/// <summary>
		/// Calculates the Convex Hull of a set of finite points.
		/// Not my original code. Original can be found <a href="https://stackoverflow.com/a/46371357">here</a>.
		/// </summary>
		/// <param name="points"></param>
		/// <returns></returns>
		public static List<Vector2> GetConvexHull(List<Vector2> points)
		{
			if (points == null)
				return null;

			if (points.Count() <= 1)
				return points;

			int n = points.Count(), k = 0;
			List<Vector2> H = new List<Vector2>(new Vector2[2 * n]);

			points.Sort((a, b) =>
				 a.x == b.x ? a.y.CompareTo(b.y) : a.x.CompareTo(b.x));

			// Build lower hull
			for (int i = 0; i < n; ++i)
			{
				while (k >= 2 && Cross(H[k - 2], H[k - 1], points[i]) <= 0)
					k--;
				H[k++] = points[i];
			}

			// Build upper hull
			for (int i = n - 2, t = k + 1; i >= 0; i--)
			{
				while (k >= t && Cross(H[k - 2], H[k - 1], points[i]) <= 0)
					k--;
				H[k++] = points[i];
			}

			return H.Take(k - 1).ToList();
		}
		/// <summary>
		/// Calculates the cross between each of the points given.
		/// Not my original code. Original can be found <a href="https://stackoverflow.com/a/46371357">here</a>.
		/// </summary>
		/// <param name="O"></param>
		/// <param name="A"></param>
		/// <param name="B"></param>
		/// <returns></returns>
		public static double Cross(Vector2 O, Vector2 A, Vector2 B)
		{
			return (A.x - O.x) * (B.y - O.y) - (A.y - O.y) * (B.x - O.x);
		}
		public static Vector3 MidPoint(Vector3 pointA, Vector3 pointB)
		{
			return Vector3.Lerp(pointA, pointB, 0.5f);
		}
		/// <summary>
		/// Fun cool code to rotate a point around another point a certain amount of degrees.
		/// </summary>
		/// <param name="point"></param>
		/// <param name="pivot"></param>
		/// <param name="degrees"></param>
		/// <returns></returns>
		public static Vector2 RotatePoint(this Vector2 point, Vector2 pivot, double degrees)
		{
			double cosTheta = Math.Cos(degrees * Mathf.Deg2Rad);
			double sinTheta = Math.Sin(degrees * Mathf.Deg2Rad);

			double x = (cosTheta * (point.x - pivot.x) - sinTheta * (point.y - pivot.y) + pivot.x);
			double y = (sinTheta * (point.x - pivot.x) - cosTheta * (point.y - pivot.y) + pivot.y);

			return new Vector2((float) x, (float) y);
		}
		/// <summary>
		/// Adjusts the position of a vectors distance relative to another vector. Original code link <see href="https://stackoverflow.com/a/21001882">here</see>.
		/// </summary>
		/// <param name="pointA"></param>
		/// <param name="pointB"></param>
		/// <param name="distance"></param>
		/// <returns></returns>
		public static Vector3 AdjustDistance(this Vector3 pointA, Vector3 pointB, float distance)
		{
			return new Vector3(pointA.x + ((pointB.x - pointA.x) * distance),
				pointA.y + ((pointB.y - pointA.y) * distance),
				pointA.z + ((pointB.z - pointA.z) * distance));
		}
	}
}