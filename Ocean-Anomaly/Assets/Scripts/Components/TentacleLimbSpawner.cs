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
using OceanAnomaly.Animation;
using System.Threading;

namespace OceanAnomaly.Components
{
	public class TentacleLimbSpawner : LimbController
	{
		// These Fields need references in the inspector
		[Header("Non-optional Fields")]
		[SerializeField]
		private TentacleLimb limbPrefab;            // Mainly just the prefab for the BodyParts needs to be given
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
		private RigConstraintState limbIkRigState;
		[SerializeField]
		private AnimationDataScriptable dampedRigData;
		private RigConstraintState limbDampedRigState;
		[SerializeField]
		private AnimationDataScriptable manualAnimationData;
		private AnimationState manualAnimationState;
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
		private List<TentacleLimb> limbs;
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
			limbs = new List<TentacleLimb>();
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
			manualAnimationState = new AnimationState(gameObject, manualAnimationData);
			stateManager.ChangeState(limbDampedRigState);
		}
		private void Start()
		{
			InstantiateLimbs();
		}
		public void InstantiateLimbs()
		{
			// Limb Generation
			TentacleLimb previousPart = limbSource.AddComponent<TentacleLimb>();
			previousPart.endpointTag = limbEndpointTag;
			previousPart.parent = transform;
			for (int i = 0; i < limbLengthLimit; i++)
			{
				// subsequent limbs get made from the impervious source limb
				TentacleLimb newPart = Instantiate(limbPrefab, previousPart.EndPoint.position, previousPart.EndPoint.rotation);
				// Set the name and index for easy reference
				newPart.lockObject = limbDetachLock;
				newPart.name = $"Tentecale Limb {i}";
				// Apply additional body part settings
				newPart.OnDetatchingEntry.AddListener(OnLimbDetatchEntry);
				newPart.OnDetatchingExit.AddListener(OnLimbDetatchExit);
				// Add health listeners if health exists
				newPart.LimbHealth?.modifyHealthEvent.AddListener((value) =>
				{
					limbTotalHealth += value;
				});
				// Resolve leftAndRightSeparation
				float currentIndexMultiplier = (limbLengthLimit - i) / limbLengthLimit;
				float distanceMultiplied = currentIndexMultiplier * Vector3.Distance(newPart.transform.position, newPart.GetMidPoint());
				newPart.LeftRightSeparation = distanceMultiplied;
				// Set the graphics from the limbDrawer
				newPart.LimbGfx.sprite = limbDrawer.LimbMiddleGraphic;
				// Add each new limb to the list of limbs to keep track of them
				limbs.Add(newPart);
				// Sets the previous part
				newPart.SetPreviousBodyPart(previousPart);
				// Finish loop with setting the previous part to the newPart we just made
				previousPart = newPart;
			}
			// Set all repeated settings for a completed limb
			SetAllLimbSettings();
			// Lastely build the rig with the new bones
			rigBuilder.Build();
		}
		private void OnLimbDetatchEntry(TentacleLimb detachedLimb)
		{
			lock (limbDetachLock)
			{
				Debug.Log($"Starting Detaching Process for: {detachedLimb.name}");
				// When a limb breaks from the chain we need to turn off the rigBuilder to modify transforms of the bones
				rigBuilder.enabled = false;
				stateManager.ChangeState(manualAnimationState);
				// Clear out the list of DampedTransforms
				ClearDampedConstraints();
			}
		}
		private void OnLimbDetatchExit(TentacleLimb detachedLimb)
		{
			lock (limbDetachLock)
			{
				Debug.Log($"Exiting Detach Process for: {detachedLimb.name}");
				// Remove the detatchedLimb from our list
				limbs.Remove(detachedLimb);
				detachedLimb.transform.SetParent(null);
				// Set all repeated settings for a completed limb
				SetAllLimbSettings();
				// Enable the Builder to stop screwing with the damped contraint bs
				rigBuilder.enabled = true;
				// Enable the current rig after the 
				stateManager.ChangeState(stateManager.GetPreviousState());
				Debug.Log($"Destroying: {detachedLimb.name}");
				Destroy(detachedLimb.gameObject);
			}
		}
		private void SetAllLimbSettings()
		{
			// If we have no more limbs then lets break out of here
			if (CheckForLimbsDestroyed()) return;
			// Reset the limbTransforms in the list
			ResetLimbTransforms();
			// Recalculate limbTotalHealth because there's some inaccuracies
			limbTotalHealth = GetLimbTotalHealth();
			// Set the graphics start and end points
			SetStartAndEndGfx();
			// Reset our IK Constraint Root and Tip
			SetTargetIk();
			// Reset the damped constrain target
			SetDampedTarget();
		}
		private void SetStartAndEndGfx()
		{
			// Back out if we need to
			if (limbDrawer == null)
			{
				return;
			}
			limbs[0].LimbGfx.sprite = limbDrawer.LimbStartGraphic;
			limbs[limbs.Count - 1].LimbGfx.sprite = limbDrawer.LimbEndGraphic;
		}
		private void SetTargetIk()
		{
			// Set the IK target position to the root end point
			limbTargetIk.position = limbs[limbs.Count - 1].EndPoint.position;
			limbIkConstraint.data.tip = limbs[limbs.Count - 1].EndPoint;
			// Set the root and tip of the chain to the first and last limb found in limbs
			limbIkConstraint.data.root = limbs[0].transform;
			Debug.Log($"Set IK Target to: {limbs[limbs.Count - 1].EndPoint.name}");
			Debug.Log($"Set IK root to: {limbs[0].name}");
		}
		private void SetDampedTarget()
		{
			// Make a new set of damped transform constraints
			TentacleLimb previousLimb = null;
			foreach (TentacleLimb limb in limbs)
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
			Debug.Log($"Created {limbDampedConstraints.Count} Damped Constraints.");
		}
		private void ClearDampedConstraints()
		{
			// Clear out the list of object
			foreach (DampedTransform dampedTransform in limbDampedConstraints)
			{
				dampedTransform.enabled = false;
				Destroy(dampedTransform.gameObject);
			}
			Debug.Log($"Disabled and cleared {limbDampedConstraints.Count} Damped Constraints.");
			limbDampedConstraints.Clear();
		}
		/// <summary>
		/// Grabs the current limbs object.
		/// </summary>
		/// <returns></returns>
		public List<TentacleLimb> GetLimbs()
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
			foreach (TentacleLimb limb in limbs)
			{
				totalHealth += limb.LimbHealth.GetCurrentHealth();
			}
			return totalHealth;
		}
		private void ResetLimbTransforms()
		{
			foreach (TentacleLimb limb in limbs)
			{
				limb.SnapToPrevious();
			}
			Debug.Log($"Reset limb transforms for: {limbs.Count}");
		}
		public void ChangeAnimationState(AnimationState state)
		{
			stateManager.ChangeState(state);
		}
	}
}