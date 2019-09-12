using UnityEngine;

namespace SceneAssets.Manipulator.Scripts {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  public class InjectRandomRigidbodyNoise : MonoBehaviour {
    [SerializeField] float _magnitude = 0.1f;
    [SerializeField] ForceMode _forceMode = ForceMode.Impulse;
    Rigidbody _rigidbody;

    void Start() { this._rigidbody = this.GetComponent<Rigidbody>(); }

    void Update() {
      this._rigidbody.AddForce(Random.insideUnitSphere * this._magnitude, this._forceMode);
      this._rigidbody.AddTorque(Random.insideUnitSphere * this._magnitude, this._forceMode);
    }
  }
}
