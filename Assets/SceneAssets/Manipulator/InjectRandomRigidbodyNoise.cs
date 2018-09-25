using UnityEngine;

namespace SceneAssets.Manipulator {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  public class InjectRandomRigidbodyNoise : MonoBehaviour {
    Rigidbody _rigidbody;
    [SerializeField] float _magnitude = 1;

    void Start() { this._rigidbody = this.GetComponent<Rigidbody>(); }

    void Update() {
      this._rigidbody.AddForce(Random.insideUnitSphere * this._magnitude);
      this._rigidbody.AddTorque(Random.insideUnitSphere * this._magnitude);
    }
  }
}