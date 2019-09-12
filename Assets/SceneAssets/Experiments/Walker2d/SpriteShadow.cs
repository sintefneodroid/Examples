using UnityEngine;
using UnityEngine.Rendering;

namespace SceneAssets.Experiments.Walker2d {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  [RequireComponent(typeof(SpriteRenderer)), ExecuteAlways]
  public class SpriteShadow : MonoBehaviour {
    [SerializeField] bool receive_shadows = true;
    [SerializeField] ShadowCastingMode cast_shadows = ShadowCastingMode.On;

    SpriteRenderer _renderer;

    void Awake() {
      this._renderer = this.GetComponent<SpriteRenderer>();

      this._renderer.shadowCastingMode = this.cast_shadows;

      this._renderer.receiveShadows = this.receive_shadows;
    }
  }
}
