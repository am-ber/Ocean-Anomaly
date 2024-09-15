using OceanAnomaly.Components;
using OceanAnomaly.Tools;
using Shapes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OceanAnomaly.Controllers
{
	public class MainBodyPartController : MonoBehaviour
	{
		[SerializeField]
		private List<LimbController> limbControllers;
		private void Start()
		{
			// Grab all children with the LimbController
			limbControllers = new List<LimbController>(GetComponentsInChildren<LimbController>());
		}
	}
}
