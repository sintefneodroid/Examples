using UnityEngine;

namespace SceneAssets.Manipulator {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  public class InjectRandomRigidbodyNoise : MonoBehaviour {
    [SerializeField] float _magnitude = 1;
    Rigidbody _rigidbody;

    void Start() { this._rigidbody = this.GetComponent<Rigidbody>(); }

    void Update() {
      this._rigidbody.AddForce(Random.insideUnitSphere * this._magnitude);
      this._rigidbody.AddTorque(Random.insideUnitSphere * this._magnitude);
    }
  }
}
