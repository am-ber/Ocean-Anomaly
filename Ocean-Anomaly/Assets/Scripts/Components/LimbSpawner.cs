using OceanAnomaly.Attributes;
using OceanAnomaly.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.U2D.IK;

namespace OceanAnomaly.Components
{
	public class LimbSpawner : MonoBehaviour
	{
		// These Fields need references in the inspector
		[Header("Non-optional Fields")]
		[SerializeField]
		private Limb limbPrefab;            // Mainly just the prefab for the BodyParts needs to be given
		[SerializeField]
		private uint limbLengthLimit = 10;  // This already has a default value

		// These fields are optional as new GameObjects will be Instantiated correctly
		// Only fill these out if you know what you're doing!
		[Header("Optional Fields")]
		[SerializeField]
		private RigBuilder rigBuilder;
		[SerializeField]
		private Rig limbRig;
		[SerializeField]
		private ChainIKConstraint limbIkConstraint;
		[SerializeField]
		private Transform limbTargetIk;
		// This is ReadOnly, or basically only used for debugging purposes
		[ReadOnly]
		[SerializeField]
		private List<Limb> limbs;
		[ReadOnly]
		public bool limbDestroyed = false;
		[ReadOnly]
		[SerializeField]
		private float limbTotalHealth = 0f;
		private void Awake()
		{
			Initialize();
		}
		private void Initialize()
		{
			// Attempt to find the rigbuilder
			if (rigBuilder == null)
			{
				rigBuilder = GetComponent<RigBuilder>();
				// If we still cannot find the RigBuilder than lets just add the Component locally
				if (rigBuilder == null)
				{
					rigBuilder = gameObject.AddComponent<RigBuilder>();
				}
			}
			// Check for the Rig component in our childeren
			if (limbRig == null)
			{
				limbRig = GetComponentInChildren<Rig>();
				// If we didn't find the limbRig amongst our children somewhere then lets just make a new child
				if (limbRig == null)
				{
					limbRig = new GameObject($"{gameObject.name} Rig").AddComponent<Rig>();
					limbRig.transform.parent = transform;
				}
			}
			// Add the rig layer to the builder
			rigBuilder.layers.Add(new RigLayer(limbRig, true));
			// Check for the IK Constraint Component
			if (limbIkConstraint == null)
			{
				limbIkConstraint = limbRig.GetComponentInChildren<ChainIKConstraint>();
				// If we didn't find the constraint in our children, lets just make a new child below the limbRig
				if (limbIkConstraint == null)
				{
					limbIkConstraint = new GameObject($"{gameObject.name} IK Constraint").AddComponent<ChainIKConstraint>();
					limbIkConstraint.transform.parent = limbRig.transform;
					// If we created the Component, then we need to set these to default documentation recommendations
					limbIkConstraint.data.chainRotationWeight = 1;
					limbIkConstraint.data.tipRotationWeight = 1;
					limbIkConstraint.data.maxIterations = 15;
					limbIkConstraint.data.tolerance = 0.0001f;
				}
			}
			// If we didn't find any children under the IK Constraint, lets change that
			if (limbTargetIk == null)
			{
				limbTargetIk = new GameObject($"{gameObject.name} IK Target").transform;
				limbTargetIk.transform.parent = limbIkConstraint.transform;
			}
			// Set the target in the constraint to the limbTargetIk game object
			limbIkConstraint.data.target = limbTargetIk;
		}
		private void Start()
		{
			InstantiateLimbs();
		}
		public void InstantiateLimbs()
		{
			// Limb Generation
			Limb previousPart = null;
			for (int i = 0; i < limbLengthLimit; i++)
			{
				// Set the previous part to the one we just created
				if (previousPart == null)
				{
					// aka our first iteration
					previousPart = Instantiate(limbPrefab, transform);
				} else
				{
					// all subsequent iterations
					Limb newPart = Instantiate(limbPrefab, previousPart.EndPointOffset.position, previousPart.EndPointOffset.rotation);
					newPart.SetPreviousBodyPart(previousPart);
					previousPart = newPart;
				}
				// Apply additional body part settings
				previousPart.OnDetatching.AddListener(OnLimbDetatch);
				// Add health listeners if health exists
				previousPart.LimbHealth?.modifyHealthEvent.AddListener((value) =>
				{
					limbTotalHealth += value;
				});
				// Add each new limb to the list of limbs to keep track of them
				limbs.Add(previousPart);
			}
			// Calculate initial limbTotalHealth
			limbTotalHealth = GetLimbTotalHealth();
			// Set the TargetIK, Root, and Tip
			SetTargetIk();
			// Lastely build the rig with the new bones
			rigBuilder.Build();
		}
		public void OnLimbDetatch(Limb detatchedLimb)
		{
			// When a limb breaks from the chain we need to turn off the rigBuilder to modify transforms of the bones
			rigBuilder.enabled = false;
			// Remove the detatchedLimb from our list
			limbs.Remove(detatchedLimb);
			// Recalculate limbTotalHealth because there's some inaccuracies
			limbTotalHealth = GetLimbTotalHealth();
			// Reset the limbTransforms in the list
			ResetLimbTransforms();
			// Reset our IK Constraint Root and Tip
			SetTargetIk();
			rigBuilder.enabled = true;
		}
		private void SetTargetIk()
		{
			// If we have no more limbs then lets break out of here
			if (limbs.Count <= 0)
			{
				limbDestroyed = true;
				return;
			}
			// Set the IK target position to the root end point
			limbTargetIk.position = limbs[limbs.Count - 1].EndPointOffset.position;
			limbIkConstraint.data.tip = limbs[limbs.Count - 1].EndPointOffset;
			// Set the root and tip of the chain to the first and last limb found in limbs
			limbIkConstraint.data.root = limbs[0].transform;
		}
		public float GetLimbTotalHealth()
		{
			float totalHealth = 0;
			foreach (Limb limb in limbs)
			{
				totalHealth += limb.LimbHealth.GetCurrentHealth();
			}
			return totalHealth;
		}
		private void ResetLimbTransforms()
		{
			foreach (Limb limb in limbs)
			{
				limb.SnapToPrevious();
			}
		}
	}
}