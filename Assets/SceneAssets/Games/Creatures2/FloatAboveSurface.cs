using UnityEngine;
using UnityEngine.Serialization;

namespace SceneAssets.Games.Creatures2 {
  public class FloatAboveSurface : MonoBehaviour {
    [FormerlySerializedAs("MinDistance")] public float _MinDistance = 1.1f;
    [FormerlySerializedAs("MaxDistance")] public float _MaxDistance = 1.2f;
    [FormerlySerializedAs("MaxForce")] public float _MaxForce = 32.0f;

    Rigidbody _rb;

    void Start() { this._rb = this.GetComponent<Rigidbody>(); }

    float RaycastDownwards() {
      RaycastHit rch;
      if (Physics.Raycast(this.transform.position, -this.transform.up, out rch, this._MaxDistance)) {
        return rch.distance;
      }

      return 100;
    }

    void FixedUpdate() {
      var distance = this.RaycastDownwards();

      var fractional_position = (this._MaxDistance - distance) / (this._MaxDistance - this._MinDistance);
      if (fractional_position < 0) fractional_position = 0;
      if (fractional_position > 1) fractional_position = 1;
      var force = fractional_position * this._MaxForce;

      this._rb.AddForceAtPosition(Vector3.up * force, this.transform.position);
    }
  }
}