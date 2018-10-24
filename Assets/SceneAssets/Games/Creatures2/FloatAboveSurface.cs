using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatAboveSurface : MonoBehaviour
{
    	public float MinDistance = 1.1f;
	public float MaxDistance = 1.2f;
	public float MaxForce = 32.0f;

	Rigidbody rb;

	void Start(){
		rb = GetComponent<Rigidbody>();
	}

	float RaycastDownwards(){
		RaycastHit rch;
		if (Physics.Raycast ( transform.position, -transform.up, out rch, MaxDistance)){
			return rch.distance;
		}

		return 100;
	}

	void FixedUpdate ()
	{
		float distance = RaycastDownwards();

		float fractionalPosition = (MaxDistance - distance) / (MaxDistance - MinDistance);
		if (fractionalPosition < 0) fractionalPosition = 0;
		if (fractionalPosition > 1) fractionalPosition = 1;
		float force = fractionalPosition * MaxForce;

		rb.AddForceAtPosition(Vector3.up * force, transform.position);
	}
}
