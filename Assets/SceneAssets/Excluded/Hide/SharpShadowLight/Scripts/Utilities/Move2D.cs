using UnityEngine;

namespace SceneAssets.Excluded.Hide.SharpShadowLight.Scripts.Utilities {
  /// <summary>
  /// 
  /// </summary>
  public class Move2D : MonoBehaviour {

    /// <summary>
    /// 
    /// </summary>
    void Update() {
        var pos = this.transform.position;
        pos.x += Input.GetAxis("Horizontal") * 30f * Time.deltaTime;
        pos.y += Input.GetAxis("Vertical") * 30f * Time.deltaTime;

        this.transform.position = pos;
    }
  }
}
