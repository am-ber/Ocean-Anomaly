using System.Collections;
using UnityEngine;
using OceanAnomaly.Tools;

namespace OceanAnomaly.Controllers
{
	public class CrosshairController : MonoBehaviour
	{
		// Inits
		public GameObject crosshairRing;
		public float minRingSize = 1;
		public float maxRingSize = 1.25f;
		public float reduceFactor = 0.1f;
		public float distanceFromCamera = 10f;
		public float scaleValue;

		Vector3 oldMousePosition;
		Vector3 delta;

		// Unity update function
		void Update()
		{
			// Get positions and convert to 2d
			Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceFromCamera));
			transform.position = worldPoint;

			// Track old position and scale object to speed of mouse
			delta = Input.mousePosition - oldMousePosition;
			scaleValue = GlobalTools.Remap(delta.magnitude, 0, 10, minRingSize, maxRingSize);
			oldMousePosition = Input.mousePosition;
			crosshairRing.transform.localScale = new Vector3(Mathf.Lerp(scaleValue, minRingSize, reduceFactor), Mathf.Lerp(scaleValue, minRingSize, reduceFactor));
			scaleValue = GlobalTools.Remap(scaleValue, minRingSize, maxRingSize, 1, 10);
		}
	}
}