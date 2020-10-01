using UnityEngine;

namespace SceneAssets.Experiments.ScriptedManipulator.Utilities {
  /// <summary>
  ///
  /// </summary>
  public class IgnoreCollision : MonoBehaviour {
    string _a_tag = "ignored_by_sub_collider_fish";

    void OnCollisionEnter(Collision collision) {
      if (collision.gameObject.CompareTag(tag : this._a_tag)) {
        Physics.IgnoreCollision(collider1 : this.GetComponent<Collider>(), collider2 : collision.collider);
      }
    }

    void OnCollisionExit(Collision collision) {
      if (collision.gameObject.CompareTag(tag : this._a_tag)) {
        Physics.IgnoreCollision(collider1 : this.GetComponent<Collider>(), collider2 : collision.collider);
      }
    }

    void OnCollisionStay(Collision collision) {
      if (collision.gameObject.CompareTag(tag : this._a_tag)) {
        Physics.IgnoreCollision(collider1 : this.GetComponent<Collider>(), collider2 : collision.collider);
      }
    }
  }
}
