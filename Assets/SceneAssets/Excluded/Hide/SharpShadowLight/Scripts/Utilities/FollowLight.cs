using UnityEngine;

namespace SceneAssets.Excluded.Hide.SharpShadowLight.Scripts.Utilities {
  public class FollowLight : MonoBehaviour {
    public GameObject _To_Follow;

    // Update is called once per frame
    void Update() { this.gameObject.transform.position = this._To_Follow.transform.position; }
  }
}
