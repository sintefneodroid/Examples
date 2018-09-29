using UnityEngine;

namespace SceneAssets.Experiments.Rigidbodies {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  public class HoverStabilise : MonoBehaviour {
    Rigidbody _rb;
    [SerializeField] float _return_to_start_speed = 0.6f;
    [SerializeField] float _stability = 0.3f;
    [SerializeField] float _stability_speed = 2.0f;

    [SerializeField] Vector3 _start_position;

    void Awake() {
      this._rb = this.GetComponent<Rigidbody>();
      this._start_position = this.transform.position;
    }

    void FixedUpdate() {
      var predicted_up = Quaternion.AngleAxis(
                             this._rb.angularVelocity.magnitude
                             * Mathf.Rad2Deg
                             * this._stability
                             / this._stability_speed,
                             this._rb.angularVelocity)
                         * this.transform.up;
      var torque_vector = Vector3.Cross(predicted_up, Vector3.up);
      this._rb.AddTorque(torque_vector * this._stability_speed * this._stability_speed);

      this._rb.AddForce(-Physics.gravity);

      this._rb.AddForce(
          (this._start_position - this.transform.position)
          * this._return_to_start_speed); //TODO: Make PID controller
    }
  }
}
