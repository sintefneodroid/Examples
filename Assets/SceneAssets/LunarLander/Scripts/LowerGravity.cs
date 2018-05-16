using UnityEngine;

namespace SceneAssets.LunarLander.Scripts {
  public class LowerGravity : MonoBehaviour {
    // Use this for initialization
    void Start() { Physics.gravity = Vector3.down * 3.33f; }

    // Update is called once per frame
    void Update() { }
  }
}
