using OceanAnomaly.Attributes;
using OceanAnomaly.Tools;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using DG.Tweening;
using System.Threading.Tasks;
using OceanAnomaly.Controllers;
using OceanAnomaly.StateManagement;
using AnimationState = OceanAnomaly.StateManagement.AnimationState;

namespace OceanAnomaly.Components
{
	public enum LimbAnimationState
	{
		IKConstraint,
		DampedConstraint,
		Manual
	}
	public class TentacleLimbSpawner : LimbController
	{
		// These Fields need references in the inspector
		[Header("Non-optional Fields")]
		[SerializeField]
		private Limb limbPrefab;            // Mainly just the prefab for the BodyParts needs to be given
		[SerializeField]
		private uint limbLengthLimit = 10;  // This already has a default value
		[TagSelector]
		[SerializeField]
		private string limbEndpointTag = "";
		[Header("Animation State Settings")]
		[SerializeField]
		private StateManager stateManager;
		[SerializeField]
		private AnimationDataScriptable ikRigAnimationData;
		[SerializeField]
		private RigConstraintState limbIkRigState;
		[SerializeField]
		private AnimationDataScriptable dampedRigData;
		[SerializeField]
		private RigConstraintState limbDampedRigState;
		// These fields are optional as new GameObjects will be Instantiated correctly
		// Only fill these out if you know what you're doing!
		[Header("Optional Fields")]
		[SerializeField]
		private RigBuilder rigBuilder;
		[Header("IK Constraint Settings")]
		[SerializeField]
		private Rig limbIkRig;
		[Range(0f, 1f)]
		[SerializeField]
		private float startWeightIkConstraint = 1f;
		[SerializeField]
		private ChainIKConstraint limbIkConstraint;
		[SerializeField]
		private Transform limbTargetIk;
		[Header("Damped Transform Settings")]
		[SerializeField]
		private Rig limbDampedRig;
		[Range(0f, 1f)]
		[SerializeField]
		private float startWeightDampedConstraint = 1f;
		[Range(0f, 1f)]
		[SerializeField]
		private float dampRotation = 0.2f;
		[Range(0f, 1f)]
		[SerializeField]
		private float dampPosition = 0f;
		[SerializeField]
		private Transform limbSource;
		[ReadOnly]
		[SerializeField]
		private List<DampedTransform> limbDampedConstraints;
		// This is ReadOnly, or basically only used for debugging purposes
		[Header("General Debugging")]
		[ReadOnly]
		[SerializeField]
		private List<Limb> limbs;
		[ReadOnly]
		public bool limbDestroyed = false;
		private object limbDetachLock = new object();
		private object rigAnimationLock = new object();
		private void Awake()
		{
			Initialize();
		}
		private void Initialize()
		{
			limbs = new List<Limb>();
			limbDampedConstraints = new List<DampedTransform>();
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
			if (limbIkRig == null)
			{
				limbIkRig = new GameObject($"{gameObject.name} IK Rig").AddComponent<Rig>();
				limbIkRig.transform.SetParent(transform);
			}
			// Check for the Rig component in our childeren
			if (limbDampedRig == null)
			{
				limbDampedRig = new GameObject($"{gameObject.name} Damped Rig").AddComponent<Rig>();
				limbDampedRig.transform.SetParent(transform, false);
			}
			// Set the default weights of the Rigs
			limbIkRig.weight = startWeightIkConstraint;
			limbDampedRig.weight = startWeightDampedConstraint;
			// Add the rig layer to the builder
			rigBuilder.layers.Add(new RigLayer(limbIkRig, true));
			rigBuilder.layers.Add(new RigLayer(limbDampedRig, true));
			// Check for the IK Constraint Component
			if (limbIkConstraint == null)
			{
				limbIkConstraint = limbIkRig.GetOrMakeComponentInChilderen<ChainIKConstraint>();
				// Set the constraint to default documentation recommendations
				limbIkConstraint.data.chainRotationWeight = 1;
				limbIkConstraint.data.tipRotationWeight = 1;
				limbIkConstraint.data.maxIterations = 15;
				limbIkConstraint.data.tolerance = 0.0001f;
			}
			// We need the target IK in a specific place, so we can just make it if somebody didn't set it before
			if (limbTargetIk == null)
			{
				limbTargetIk = new GameObject($"{gameObject.name} IK Target").transform;
				limbTargetIk.transform.SetParent(limbIkConstraint.transform, false);
			}
			// Lets make a parent object for all the limbs we create
			if (limbSource == null)
			{
				limbSource = new GameObject($"Limb Source").transform;
				limbSource.transform.SetParent(transform, false);
			}
			// Set the target in the constraint to the limbTargetIk game object
			limbIkConstraint.data.target = limbTargetIk;
			// Set animation states
			stateManager = new StateManager();
			limbIkRigState = new RigConstraintState(gameObject, ikRigAnimationData, limbIkRig);
			limbDampedRigState = new RigConstraintState(gameObject, dampedRigData, limbDampedRig);
			stateManager.ChangeState(limbDampedRigState);
		}
		private void Start()
		{
			InstantiateLimbs();
		}
		public void InstantiateLimbs()
		{
			// Limb Generation
			Limb previousPart = limbSource.AddComponent<Limb>();
			previousPart.endpointTag = limbEndpointTag;
			previousPart.parent = transform;
			for (int i = 0; i < limbLengthLimit; i++)
			{
				// subsequent limbs get made from the impervious source limb
				Limb newPart = Instantiate(limbPrefab, previousPart.EndPoint.position, previousPart.EndPoint.rotation);
				newPart.SetPreviousBodyPart(previousPart);
				previousPart = newPart;
				// Set the name and index for easy reference
				previousPart.lockObject = limbDetachLock;
				previousPart.LimbIndex = i;
				previousPart.name = $"Tentecale Limb {i}";
				// Apply additional body part settings
				previousPart.OnDetatchingExit.AddListener(OnLimbDetatchExit);
				// Add health listeners if health exists
				previousPart.LimbHealth?.modifyHealthEvent.AddListener((value) =>
				{
					limbTotalHealth += value;
				});
				// Resolve leftAndRightSeparation
				float currentIndexMultiplier = (limbLengthLimit - i) / limbLengthLimit;
				float distanceMultiplied = currentIndexMultiplier * Vector3.Distance(newPart.transform.position, newPart.GetMidPoint());
				print($"Limb {i} : {distanceMultiplied}");
				newPart.LeftRightSeparation = distanceMultiplied;
				// Add each new limb to the list of limbs to keep track of them
				limbs.Add(previousPart);
			}
			// Calculate initial limbTotalHealth
			limbTotalHealth = GetLimbTotalHealth();
			// Set the TargetIK, Root, and Tip
			SetTargetIk();
			// Set the damped constraint target
			SetDampedTarget();
			// Lastely build the rig with the new bones
			rigBuilder.Build();
		}
		private void OnLimbDetatchEntry(Limb detachedLimb)
		{
			// When a limb breaks from the chain we need to turn off the rigBuilder to modify transforms of the bones
			rigBuilder.enabled = false;
		}
		private void OnLimbDetatchExit(Limb detatchedLimb)
		{
			lock (limbDetachLock)
			{
				// Remove the detatchedLimb from our list
				limbs.Remove(detatchedLimb);
				// Recalculate limbTotalHealth because there's some inaccuracies
				limbTotalHealth = GetLimbTotalHealth();
				// Reset the limbTransforms in the list
				ResetLimbTransforms();
				// Reset our IK Constraint Root and Tip
				SetTargetIk();
				// Reset the damped constrain target
				SetDampedTarget();
				// OnEnable() of the builder should call Build() for the rigBuilder
				rigBuilder.enabled = true;
			}
		}
		private void SetTargetIk()
		{
			// If we have no more limbs then lets break out of here
			if (CheckForLimbsDestroyed()) return;
			// Set the IK target position to the root end point
			limbTargetIk.position = limbs[limbs.Count - 1].EndPoint.position;
			limbIkConstraint.data.tip = limbs[limbs.Count - 1].EndPoint;
			// Set the root and tip of the chain to the first and last limb found in limbs
			limbIkConstraint.data.root = limbs[0].transform;
		}
		private void SetDampedTarget()
		{
			// If we have no more limbs then lets break out of here
			if (CheckForLimbsDestroyed()) return;
			// Clear out the list of object
			foreach (DampedTransform dampedTransform in limbDampedConstraints)
			{
				Destroy(dampedTransform.gameObject);
			}
			limbDampedConstraints.Clear();
			// Make a new set of damped transform constraints
			Limb previousLimb = null;
			foreach (Limb limb in limbs)
			{
				// Create the damped constraint and set initial settings
				DampedTransform dampedTransform = new GameObject($"Damped Constraint for {limb.name}").AddComponent<DampedTransform>();
				// We set all the damped transforms as children to the damped rig
				// They don't need to be subsiquint children to eachother
				dampedTransform.transform.parent = limbDampedRig.transform;
				dampedTransform.data.dampRotation = dampRotation;
				dampedTransform.data.dampPosition = dampPosition;
				dampedTransform.data.maintainAim = true;
				dampedTransform.data.constrainedObject = limb.transform;
				// Establish the source and constrained object
				if (previousLimb == null)
				{
					previousLimb = limb;
					dampedTransform.data.sourceObject = limbSource;
				} else
				{
					dampedTransform.data.sourceObject = previousLimb.transform;
				}
				previousLimb = limb;
				// Finally add this new DamepedTransform constraint to the list
				limbDampedConstraints.Add(dampedTransform);
			}
		}
		/// <summary>
		/// Grabs the current limbs object.
		/// </summary>
		/// <returns></returns>
		public List<Limb> GetLimbs()
		{
			return limbs;
		}
		public bool CheckForLimbsDestroyed()
		{
			// If we have no more limbs then lets break out of here
			if (limbs.Count <= 0)
			{
				limbDestroyed = true;
				return true;
			}
			return false;
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
			print("Reseting Limb Transforms");
			foreach (Limb limb in limbs)
			{
				limb.SnapToPrevious();
			}
		}
		public void ChangeAnimationState(AnimationState state)
		{
			stateManager.ChangeState(state);
		}
	}
}