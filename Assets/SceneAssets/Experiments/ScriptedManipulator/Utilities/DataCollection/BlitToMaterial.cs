using UnityEngine;

namespace SceneAssets.Experiments.ScriptedManipulator.Utilities.DataCollection {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  [ExecuteInEditMode]
  public class BlitToMaterial : MonoBehaviour {
    [SerializeField] Material _material = null;

    void OnRenderImage(RenderTexture source, RenderTexture destination) {
      Graphics.Blit(source : source, dest : destination, mat : this._material);
    }
  }
}
