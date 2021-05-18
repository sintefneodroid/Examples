﻿namespace SceneAssets.Games.Creatures2 {
  /// <summary>
  /// </summary>
  public class FloatAboveSurface : UnityEngine.MonoBehaviour {
    /// <summary>
    /// </summary>
    public float _MinDistance = 1.1f;

    /// <summary>
    /// </summary>
    public float _MaxDistance = 1.2f;

    /// <summary>
    /// </summary>
    public float _MaxForce = 32.0f;

    UnityEngine.Rigidbody _rb;

    void Start() { this._rb = this.GetComponent<UnityEngine.Rigidbody>(); }

    void FixedUpdate() {
      var distance = this.RaycastDownwards();

      var fractional_position = (this._MaxDistance - distance) / (this._MaxDistance - this._MinDistance);
      if (fractional_position < 0) {
        fractional_position = 0;
      }

      if (fractional_position > 1) {
        fractional_position = 1;
      }

      var force = fractional_position * this._MaxForce;

      this._rb.AddForceAtPosition(force : UnityEngine.Vector3.up * force, position : this.transform.position);
    }

    float RaycastDownwards() {
      if (UnityEngine.Physics.Raycast(origin : this.transform.position,
                                      direction : -this.transform.up,
                                      hitInfo : out var rch,
                                      maxDistance : this._MaxDistance)) {
        return rch.distance;
      }

      return 100;
    }
  }
}