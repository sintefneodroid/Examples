using UnityEngine;

namespace SceneAssets.BalanceBall {
  /// <summary>
  ///
  /// </summary>
  [RequireComponent(requiredComponent : typeof(Rigidbody))]
  public class KeepRigidbodyAlive : MonoBehaviour {
    Rigidbody _rb;
    void Awake() { this._rb = this.GetComponent<Rigidbody>(); }

    void FixedUpdate() {
      if (this._rb.IsSleeping()) {
        this._rb.WakeUp();
      }
    }
  }
}
