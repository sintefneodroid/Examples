using UnityEngine;

namespace SceneAssets.Satellite.Scripts {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  [RequireComponent(typeof(Rigidbody))]
  public class InitialForce : MonoBehaviour {
    [SerializeField] Vector3 _force;
    [SerializeField] bool _on_awake = true;

    [SerializeField] Rigidbody _rb;
    [SerializeField] bool _relative;
    [SerializeField] bool _torque;

    void ApplyInitialForce() {
      if (this._torque) {
        if (this._relative) {
          this._rb.AddRelativeTorque(this._force, ForceMode.Impulse);
        } else {
          this._rb.AddTorque(this._force, ForceMode.Impulse);
        }
      } else {
        if (this._relative) {
          this._rb.AddRelativeForce(this._force, ForceMode.Impulse);
        } else {
          this._rb.AddForce(this._force, ForceMode.Impulse);
        }
      }
    }

    void Awake() {
      this._rb = this.GetComponent<Rigidbody>();

      if (this._on_awake) {
        this.ApplyInitialForce();
      }
    }

    void Start() {
      if (!this._on_awake) {
        this.ApplyInitialForce();
      }
    }
  }
}