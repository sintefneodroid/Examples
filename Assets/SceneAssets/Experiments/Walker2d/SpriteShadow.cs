namespace SceneAssets.Experiments.Walker2d {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  [UnityEngine.RequireComponent(requiredComponent : typeof(UnityEngine.SpriteRenderer))]
  [UnityEngine.ExecuteAlways]
  public class SpriteShadow : UnityEngine.MonoBehaviour {
    [UnityEngine.SerializeField] bool receive_shadows = true;

    [UnityEngine.SerializeField]
    UnityEngine.Rendering.ShadowCastingMode cast_shadows = UnityEngine.Rendering.ShadowCastingMode.On;

    UnityEngine.SpriteRenderer _renderer;

    void Awake() {
      this._renderer = this.GetComponent<UnityEngine.SpriteRenderer>();

      this._renderer.shadowCastingMode = this.cast_shadows;

      this._renderer.receiveShadows = this.receive_shadows;
    }
  }
}