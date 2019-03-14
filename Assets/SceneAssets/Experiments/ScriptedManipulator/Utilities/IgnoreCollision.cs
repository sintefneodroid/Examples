using UnityEngine;

namespace SceneAssets.Experiments.ScriptedManipulator.Utilities {
  /// <summary>
  ///
  /// </summary>
  public class IgnoreCollision : MonoBehaviour {
    string a_tag = "ignored_by_sub_collider_fish";

    void OnCollisionEnter(Collision collision) {
      if (collision.gameObject.CompareTag(a_tag)) {
        Physics.IgnoreCollision(this.GetComponent<Collider>(), collision.collider);
      }
    }

    void OnCollisionExit(Collision collision) {
      if (collision.gameObject.CompareTag(a_tag)) {
        Physics.IgnoreCollision(this.GetComponent<Collider>(), collision.collider);
      }
    }

    void OnCollisionStay(Collision collision) {
      if (collision.gameObject.CompareTag(a_tag)) {
        Physics.IgnoreCollision(this.GetComponent<Collider>(), collision.collider);
      }
    }
  }
}
