using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatAboveSurface : MonoBehaviour {
  public float MinDistance = 1.1f;
  public float MaxDistance = 1.2f;
  public float MaxForce = 32.0f;

  Rigidbody rb;

  void Start() { this.rb = this.GetComponent<Rigidbody>(); }

  float RaycastDownwards() {
    RaycastHit rch;
    if (Physics.Raycast(this.transform.position, -this.transform.up, out rch, this.MaxDistance)) {
      return rch.distance;
    }

    return 100;
  }

  void FixedUpdate() {
    float distance = this.RaycastDownwards();

    float fractionalPosition = (this.MaxDistance - distance) / (this.MaxDistance - this.MinDistance);
    if (fractionalPosition < 0) fractionalPosition = 0;
    if (fractionalPosition > 1) fractionalPosition = 1;
    float force = fractionalPosition * this.MaxForce;

    this.rb.AddForceAtPosition(Vector3.up * force, this.transform.position);
  }
}