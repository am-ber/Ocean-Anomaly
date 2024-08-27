using Dreamteck.Splines;
using OceanAnomaly.Controllers;
using OceanAnomaly.Tools;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace OceanAnomaly.Components.Weapon
{
	[CreateAssetMenu(fileName = "ProjectileType", menuName = "ScriptableObjects/WeaponFuntion/Projectile")]
	public class ProjectileScriptable : BaseWeponFunctionScriptable
	{
		public Sprite sprite;
		public float travelSpeed;
		public float maxSpeed;
		public uint damage;
		[TagSelector]
		public string projectileTag = "Untagged";
		public string impactSoundName;
		public LayerMask projectileLayer;
		public GameObject projectilePrefab;
		public GameObject impactPrefab;
		private ProjectileBehavior behavior;
		private void CheckForComponents(GameObject givenObject)
		{
			// Set the projectile target to a ProjectileBehavior script that will eventually exist on the object
			behavior = givenObject.GetComponent<ProjectileBehavior>();
			if (behavior == null)
			{
				behavior = givenObject.AddComponent<ProjectileBehavior>();
			}
			// Apply layer and tags
			givenObject.layer = GlobalTools.MaskToLayer(projectileLayer);
			givenObject.tag = projectileTag;
		}
		public void FirePorjectile(GameObject givenObject, float direction, float accuracy, Vector3? targetPosition = null, Transform targetTransform = null)
		{
			CheckForComponents(givenObject);
			// Apply Projectile Settings
			behavior.maxSpeed = maxSpeed;
			behavior.travelSpeed = travelSpeed;
			behavior.accuracy = accuracy;
			behavior.damage = damage;
			behavior.rotationSpeed = 2;
			behavior.impactSoundName = impactSoundName;
			behavior.impactPrefab = impactPrefab;
			// Resolve which target to set for
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
