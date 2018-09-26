using UnityEngine;

namespace SceneAssets.Experiments.Rigidbodies {
  public class FestoBehaviour : MonoBehaviour {
    [SerializeField] Rigidbody[] _children;
    [SerializeField] bool _find_global_rigidbodies;
    [SerializeField] float _torque_scalar;

    void Awake() {
      this._children = this._find_global_rigidbodies
                           ? FindObjectsOfType<Rigidbody>()
                           : this.GetComponentsInChildren<Rigidbody>();
    }

    void Update() {
      if (Input.GetKeyDown(KeyCode.UpArrow)) {
        this._torque_scalar += 100;
      } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
        this._torque_scalar -= 100;
      }
    }

    void FixedUpdate() {
      foreach (var body in this._children) {
        if (body.gameObject != this) {
          body.AddRelativeTorque(Vector3.forward * this._torque_scalar);
        }
      }
    }
  }
}