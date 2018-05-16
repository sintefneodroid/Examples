using System;
using UnityEngine;

namespace SceneAssets.Experiments.Rigidbodies {
  public class Traction : MonoBehaviour {
    [SerializeField] Rigidbody[] _tentacles;

    void Start() { this._tentacles = this.GetComponentsInChildren<Rigidbody>(); }

    void FixedUpdate() {
      if (Input.GetKeyDown(KeyCode.LeftArrow)) {
        foreach (var tentacle in this._tentacles) //tentacle.AddRelativeForce (Vector3.left);
        {
          if (Math.Abs(tentacle.transform.localPosition.x) > 2) {
            tentacle.transform.localPosition = tentacle.transform.localPosition - tentacle.transform.right;
          }
        }
      } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
        foreach (var tentacle in this._tentacles) //tentacle.AddRelativeForce (Vector3.right);
        {
          tentacle.transform.localPosition = tentacle.transform.localPosition + tentacle.transform.right;
        }
      }
    }
  }
}
