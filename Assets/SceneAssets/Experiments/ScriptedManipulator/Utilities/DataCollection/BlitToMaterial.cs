namespace SceneAssets.Experiments.ScriptedManipulator.Utilities.DataCollection {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  [UnityEngine.ExecuteInEditMode]
  public class BlitToMaterial : UnityEngine.MonoBehaviour {
    [UnityEngine.SerializeField] UnityEngine.Material _material = null;

    void OnRenderImage(UnityEngine.RenderTexture source, UnityEngine.RenderTexture destination) {
      UnityEngine.Graphics.Blit(source : source, dest : destination, mat : this._material);
    }
  }
}