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
		public float projectileHealth;

		public void FirePorjectile(GameObject projectileObject, Spline pathToFollow)
		{
			SpriteRenderer renderer = projectileObject.GetComponent<SpriteRenderer>();
			if (renderer == null)
			{
				renderer = projectileObject.AddComponent<SpriteRenderer>();
			}
			renderer.sprite = sprite;

		}
	}
}
