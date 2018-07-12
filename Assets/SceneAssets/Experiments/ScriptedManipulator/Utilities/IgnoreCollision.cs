using UnityEngine;

namespace SceneAssets.Experiments.ScriptedGrasper.Utilities {
  public class IgnoreCollision : MonoBehaviour {
    // Use this for initialization
    void Start() { }

    // Update is called once per frame
    void Update() { }

    void OnCollisionEnter(Collision collision) {
      if (collision.gameObject.tag == "ignored_by_sub_collider_fish") {
        Physics.IgnoreCollision(this.GetComponent<Collider>(), collision.collider);
      }
    }

    void OnCollisionExit(Collision collision) {
      if (collision.gameObject.tag == "ignored_by_sub_collider_fish") {
        Physics.IgnoreCollision(this.GetComponent<Collider>(), collision.collider);
      }
    }

    void OnCollisionStay(Collision collision) {
      if (collision.gameObject.tag == "ignored_by_sub_collider_fish") {
        Physics.IgnoreCollision(this.GetComponent<Collider>(), collision.collider);
      }
    }
  }
}
