using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// I basically took this whole class from (https://youtu.be/KPoeNZZ6H4s?si=U6-_A96gwLu1b12I)
/// because I never studied math this much. So I'm just gonna use this and hope it works...
/// </summary>
public class SecondOrderDynamics : MonoBehaviour
{
	[Header("Inputs")]
	public float f = 0f;
	public float z = 0f;
	public float r = 0f;
	public Vector3 x0 = Vector3.zero;
	[Header("")]
	private Vector3 xp;			// Previous input
	private Vector3 y, yd;		// State variables
	private float k1, k2, k3;	// Dynamics constants

	private void Start()
	{
		// Compute constants
		k1 = z / (Mathf.PI * f);
		k2 = 1 / ((2 * Mathf.PI * f) * (2 * Mathf.PI * f));
		k3 = r * z / (2 * Mathf.PI * f);
		// Initialize variables
		xp = x0;
		y = x0;
		yd = Vector3.zero;
	}
	/// <summary>
	/// 
	/// </summary>
	/// <param name="T">Time.deltaTime</param>
	/// <param name="x"></param>
	/// <param name="xd"></param>
	/// <returns></returns>
	public Vector3 CalculateNewY(float T, Vector3 x, Vector3? xd = null)
	{
		// Estimate velocity
		if (xd == null)
		{
			xd = (x - xp) / T;
			xp = x;
		}
		// Clamp k2 to guarantee stability without jitter
		float k2_stable = Mathf.Max(k2, T * T / 2 + T * k1 / 2, T * k1);
		// Integrate position by velocity
		y = y + T * yd;
		// Integrate velocity by acceleration
		yd = yd + T * (x + (k3 * xd.Value) - y - (k1 * yd)) / k2_stable;
		return y;
	}
}
