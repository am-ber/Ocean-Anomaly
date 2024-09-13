using OceanAnomaly.Tools;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace OceanAnomaly.Attributes
{
	public class FieldOfView2D : MonoBehaviour
	{
		public float viewDistance = 20f;
		public float viewAngle = 15f;
		[SerializeField]
		private LayerMask targetLayers;
		[SerializeField]
		private LayerMask obstacleLayers;
		[SerializeField]
		private float updateFrequency = 0.1f; // In seconds
		public bool PauseUpdating = false;
		[ReadOnly]
		[SerializeField]
		private List<Collider2D> visibleTargets;
		[SerializeField]
		private float viewRotationOffset = -90f;
		public Transform viewTransform;
		public UnityEvent<Collider2D[]> OnTargetsFound;
		private void Awake()
		{
			Initialize();
		}
		private void OnValidate()
		{
			Initialize();
		}
		private void Initialize()
		{
			// If the list is null, then lets just make a new list
			if (visibleTargets == null)
			{
				visibleTargets = new List<Collider2D>();
			}
			// If we didn't give it a transform for offset, lets just set it to this MonoBehaviours
			if (viewTransform == null)
			{
				viewTransform = transform;
			}
		}
		private async void Update()
		{
			// If we aren't updating, then lets back out
            if (PauseUpdating)
            {
				return;
            }
			// Manually check for targets
			CheckForTargets();
            /* TODO: When updating to Unity 2023 or Unity 6 or higher, this should be changed to 
			 * await Awaitable.WaitForSecondsAsync(updateFrequency);
			 */
            await Task.Delay((int)(1000 * updateFrequency));
		}
		/// <summary>
		/// Grabs the current targets in the viewDistance and viewAngle with a Physics Overlap.
		/// </summary>
		/// <returns></returns>
		public Collider2D[] GetTargetsInField()
		{
			return visibleTargets.ToArray();
		}
		/// <summary>
		/// Manually checks for targets. You don't need to do this if the Updating isn't paused.
		/// But a manual call will be sure to grab current targets this frame instead of waiting
		/// for the next update.
		/// </summary>
		public void CheckForTargets()
		{
			Collider2D[] targetsAroundEnemy = Physics2D.OverlapCircleAll(viewTransform.position, viewDistance, targetLayers);
			if (targetsAroundEnemy.Length > 0)
			{
				// Create a list for all the targets in view
				List<Collider2D> targetsInView = new List<Collider2D>();
				foreach (Collider2D target in targetsAroundEnemy)
				{
					// Check if the target is in view
					if (TransformInView(target.transform))
					{
						targetsInView.Add(target);
					}
				}
				// Check for them NOT being the same targets as our last call
				if (!SameAsCurrentTargets(targetsInView))
				{
					Debug.Log($"Found targets on layers: {GlobalTools.MaskToLayer(targetLayers)}");
					// Clear the list and populate it with the newOnes found in the view
					visibleTargets.Clear();
					visibleTargets.AddRange(targetsInView);
				}
				// Finally call any listeners we may have
				OnTargetsFound?.Invoke(GetTargetsInField());
			} else
			{
				visibleTargets.Clear();
			}
		}
		/// <summary>
		/// Internally Checks if the new targets given are the same as the previous ones.
		/// </summary>
		/// <param name="newTargets"></param>
		/// <returns></returns>
		private bool SameAsCurrentTargets(List<Collider2D> newTargets)
		{
			foreach (Collider2D newTarget in newTargets)
			{
				// Check if the previous targets name is the same as our newTarget
				// We need to check for the transforms because the Collider2D data will
				// always be different each frame as positions change.
				if (!visibleTargets.Exists(previousTarget => previousTarget.transform == newTarget.transform))
				{
					return false;
				}
			}
			return true;
		}
		/// <summary>
		/// Checks if the given transform is in view of the forward from the viewTransform.
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		public bool TransformInView(Transform target)
		{
			// Check if we are in the view angles
			Vector3 directionToPoint = (target.position - viewTransform.position).normalized;
			float pointAngle = Mathf.Atan2(directionToPoint.y, directionToPoint.x) * Mathf.Rad2Deg;
			if (!(pointAngle < (viewAngle / 2)))
			{
				return false;
			}
			// Check if our view is obstructed by anything in the obstacle layer
			float dstToTarget = Vector3.Distance(transform.position, target.position);
			RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPoint, dstToTarget, obstacleLayers);
			if (hit.collider == null)
			{
				return true;
			}
			return false;
		}
		public Vector3 DirectionFromAngle(float angleInDegrees)
		{
			angleInDegrees -= viewTransform.eulerAngles.z;
			return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0);
		}
	}
}