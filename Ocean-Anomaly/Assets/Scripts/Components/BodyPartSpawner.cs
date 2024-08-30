using OceanAnomaly.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.U2D.IK;

namespace OceanAnomaly.Components
{
	public struct LimbData
	{
		public Transform LimbPoint;
		public List<BodyPart> Limbs;
		public Transform LimbTarget;
		public ChainIKConstraint LimbIk;
		public LimbData(Transform limbPoint, List<BodyPart> limbs, Transform limbTarget, ChainIKConstraint limbIk)
		{
			LimbPoint = limbPoint;
			Limbs = limbs;
			LimbTarget = limbTarget;
			LimbIk = limbIk;
		}
	}
	public class BodyPartSpawner : MonoBehaviour
	{
		[SerializeField]
		private BodyPart LimbPrefab;
		[SerializeField]
		private List<Transform> LimbPoints;
		[SerializeField]
		private Rig limbRig;
		[SerializeField]
		private BoneRenderer boneRenderer;
		[SerializeField]
		private Animator animatorComponent;
		[SerializeField]
		private uint limbLengthLimit = 10;
		private List<LimbData> limbCollections;
		private void Awake()
		{
			if (animatorComponent == null)
			{
				animatorComponent = GetComponent<Animator>();
			}
			if (limbRig == null)
			{
				limbRig = gameObject.RecursiveFindComponentLocal<Rig>();
			}
			if (boneRenderer == null)
			{
				boneRenderer = GetComponent<BoneRenderer>();
				boneRenderer.transforms = new Transform[limbLengthLimit * LimbPoints.Count];
			}
			SetRigComponents(false);
			// Create the list of limbPoint, limb collections
			limbCollections = new List<LimbData>();
			foreach (Transform limbPoint in LimbPoints)
			{
				// Create a new IK Rig and targetIK GameObject
				GameObject limbIkRig = new GameObject($"{limbPoint.name} IK Rig");
				GameObject limbTargetIK = new GameObject($"{limbPoint.name} IK Target");
				limbIkRig.transform.parent = limbRig.transform;
				limbTargetIK.transform.parent = limbIkRig.transform;
				// Add the ChainIKConstraint component to it
				ChainIKConstraint limbIk = limbIkRig.AddComponent<ChainIKConstraint>();
				limbIk.enabled = false;
				// Add this struct to the list
				limbCollections.Add(new LimbData(limbPoint, new List<BodyPart>(), limbTargetIK.transform, limbIk));
			}
		}
		private void Start()
		{
			CreateLimbs();
		}
		public void CreateLimbs()
		{
			int currentIndex = 0;
			// Iterate through the list that has limbPoints and a list of limbs so we can add all the limbs we Instantiate
			foreach (LimbData limbData in limbCollections)
			{
				if (limbData.LimbPoint != null)
				{
					// Limb Generation
					BodyPart previousPart = null;
					for (int i = 0; i < limbLengthLimit; i++)
					{
						// Set the previous part to the one we just created
						if (previousPart == null)
						{
							previousPart = Instantiate(LimbPrefab, limbData.LimbPoint);
						} else
						{
							BodyPart newPart = Instantiate(LimbPrefab, previousPart.EndPointOffset.position, previousPart.EndPointOffset.rotation);
							newPart.SetPreviousBodyPart(previousPart);
							previousPart = newPart;
						}
						// Apply additional body part settings
						previousPart.OnDetatching.AddListener(OnLimbDetatch);
						if (boneRenderer != null)
						{
							boneRenderer.transforms[i + (currentIndex * limbLengthLimit)] = previousPart.transform;
						}
						limbData.Limbs.Add(previousPart);
					}
					// Set the limb target at the root end point
					limbData.LimbTarget.position = previousPart.EndPointOffset.position;
					// This is the code where we add the rigs effector at the end of this chain of body parts
					if (limbRig != null)
					{
						limbRig.AddEffector(limbData.LimbTarget, new RigEffectorData.Style());
						// Set the root and tip of the chain to the first and last limb found in limbs
						limbData.LimbIk.data.root = limbData.Limbs[0].transform;
						limbData.LimbIk.data.tip = limbData.Limbs[limbData.Limbs.Count - 1].transform;
						limbData.LimbIk.data.target = limbData.LimbIk.transform;
						limbData.LimbIk.data.chainRotationWeight = 1;
						limbData.LimbIk.data.tipRotationWeight = 1;
						limbData.LimbIk.data.maxIterations = 15;
						limbData.LimbIk.data.tolerance = 0.0001f;
						limbData.LimbIk.enabled = true;
					}
					if (boneRenderer != null)
					{
						boneRenderer.drawBones = true;
					}
				}
				currentIndex++;
			}
			SetRigComponents(true);
		}
		public void OnLimbDetatch()
		{
			SetRigComponents(false);
			SetRigComponents(true);
		}
		public void SetRigComponents(bool enabled)
		{
			if (animatorComponent != null)
			{
				animatorComponent.enabled = enabled;
			}
			if (limbRig != null)
			{
				limbRig.enabled = enabled;
			}
			if (boneRenderer != null)
			{
				boneRenderer.enabled = enabled;
			}
		}
	}
}