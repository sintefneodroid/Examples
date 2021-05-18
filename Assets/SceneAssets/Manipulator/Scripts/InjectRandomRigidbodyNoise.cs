namespace SceneAssets.Manipulator.Scripts {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  public class InjectRandomRigidbodyNoise : UnityEngine.MonoBehaviour {
    [UnityEngine.SerializeField] float _magnitude = 0.1f;
    [UnityEngine.SerializeField] UnityEngine.ForceMode _forceMode = UnityEngine.ForceMode.Impulse;
    UnityEngine.Rigidbody _rigidbody;

    void Start() { this._rigidbody = this.GetComponent<UnityEngine.Rigidbody>(); }

    void Update() {
      this._rigidbody.AddForce(force : UnityEngine.Random.insideUnitSphere * this._magnitude,
                               mode : this._forceMode);
      this._rigidbody.AddTorque(torque : UnityEngine.Random.insideUnitSphere * this._magnitude,
                                mode : this._forceMode);
    }
  }
}