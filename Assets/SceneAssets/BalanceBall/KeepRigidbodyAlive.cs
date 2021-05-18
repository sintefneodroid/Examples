namespace SceneAssets.BalanceBall {
  /// <summary>
  /// </summary>
  [UnityEngine.RequireComponent(requiredComponent : typeof(UnityEngine.Rigidbody))]
  public class KeepRigidbodyAlive : UnityEngine.MonoBehaviour {
    UnityEngine.Rigidbody _rb;
    void Awake() { this._rb = this.GetComponent<UnityEngine.Rigidbody>(); }

    void FixedUpdate() {
      if (this._rb.IsSleeping()) {
        this._rb.WakeUp();
      }
    }
  }
}