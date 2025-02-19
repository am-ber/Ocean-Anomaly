﻿using OceanAnomaly.Attributes;
using OceanAnomaly.Tools;
using System.Collections;
using UnityEngine;

namespace OceanAnomaly.Controllers
{
	public class CrosshairController : MonoBehaviour
	{
		// Inits
		public GameObject crosshairRing;
		public float minRingSize = 1;
		public float maxRingSize = 2;
		public float reduceFactor = 0.2f;
		[SerializeField]
		private bool softDisabled = false;
		public bool SoftDisabled
		{
			get { return softDisabled; }
			set
			{
				softDisabled = value;
				crosshairRing.SetActive(!softDisabled);
			}
		}
		[ReadOnly]
		[SerializeField]
		private float scaleValue = 1;
		Vector3 oldMousePosition;
		Vector3 delta;
		private void OnEnable()
		{
			Cursor.visible = false;
		}
		// Unity update function
		void Update()
		{
			if (softDisabled)
			{
				return;
			}
			// Get mouse position in 2D only
			Vector2 worldPoint2d = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			// Set position to mouse position
			transform.position = worldPoint2d;
			crosshairRing.transform.position = worldPoint2d;

			// Track old position and scale objet to speed of mouse
			delta = Input.mousePosition - oldMousePosition;
			scaleValue = GlobalTools.Map(delta.magnitude > 10 ? 10 : delta.magnitude, 0, 10, minRingSize, maxRingSize);
			oldMousePosition = Input.mousePosition;
			crosshairRing.transform.localScale = new Vector3(Mathf.Lerp(scaleValue, minRingSize, reduceFactor), Mathf.Lerp(scaleValue, minRingSize, reduceFactor));
			scaleValue = GlobalTools.Map(scaleValue, minRingSize, maxRingSize, 1, 10);
		}
		private void OnDisable()
		{
			Cursor.visible = true;
		}
	}
}