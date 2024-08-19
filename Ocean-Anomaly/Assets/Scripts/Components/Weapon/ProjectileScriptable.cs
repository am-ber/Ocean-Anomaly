using Dreamteck.Splines;
using OceanAnomaly.Controllers;
using UnityEngine;

namespace OceanAnomaly.Components.Weapon
{
	[CreateAssetMenu(fileName = "ProjectileType", menuName = "ScriptableObjects/WeaponFuntion/Projectile")]
	public class ProjectileScriptable : BaseWeponFunctionScriptable
	{
		public Sprite sprite;
		public float travelSpeed;
		public Health projectileHealth;
		[TagSelector]
		public string projectileTag = "Untagged";
		public LayerMask projectileLayer;
		public GameObject projectilePrefab;
		private ProjectileBehavior behavior;
		private void CheckForComponents()
		{
			// Set the projectile target to a ProjectileBehavior script that will eventually exist on the object
			behavior = projectilePrefab.GetComponent<ProjectileBehavior>();
			if (behavior == null)
			{
				behavior = projectilePrefab.AddComponent<ProjectileBehavior>();
			}
		}
		/// <summary>
		/// Will fire the projectile given along the <seealso cref="Spline"/> path to follow.
		/// </summary>
		/// <param name="projectileObject"></param>
		/// <param name="pathToFollow"></param>
		public void FirePorjectile(float direction, float accuracy, Vector3? targetPosition = null, Transform targetTransform = null)
		{
			CheckForComponents();
			if (targetPosition != null)
			{
				behavior.setTarget(targetPosition.Value, accuracy);
			}
			if (targetTransform != null)
			{
				behavior.setTarget(targetTransform, accuracy);
			}
		}
	}
}
