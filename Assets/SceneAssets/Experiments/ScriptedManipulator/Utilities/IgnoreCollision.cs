namespace SceneAssets.Experiments.ScriptedManipulator.Utilities {
  /// <summary>
  /// </summary>
  public class IgnoreCollision : UnityEngine.MonoBehaviour {
    string _a_tag = "ignored_by_sub_collider_fish";

    void OnCollisionEnter(UnityEngine.Collision collision) {
      if (collision.gameObject.CompareTag(tag : this._a_tag)) {
        UnityEngine.Physics.IgnoreCollision(collider1 : this.GetComponent<UnityEngine.Collider>(),
                                            collider2 : collision.collider);
      }
    }

    void OnCollisionExit(UnityEngine.Collision collision) {
      if (collision.gameObject.CompareTag(tag : this._a_tag)) {
        UnityEngine.Physics.IgnoreCollision(collider1 : this.GetComponent<UnityEngine.Collider>(),
                                            collider2 : collision.collider);
      }
    }

    void OnCollisionStay(UnityEngine.Collision collision) {
      if (collision.gameObject.CompareTag(tag : this._a_tag)) {
        UnityEngine.Physics.IgnoreCollision(collider1 : this.GetComponent<UnityEngine.Collider>(),
                                            collider2 : collision.collider);
      }
    }
  }
}