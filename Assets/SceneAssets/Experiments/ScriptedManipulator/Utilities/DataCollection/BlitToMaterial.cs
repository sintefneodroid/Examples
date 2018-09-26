using UnityEngine;

namespace SceneAssets.Experiments.ScriptedManipulator.Utilities.DataCollection {
  [ExecuteInEditMode]
  public class BlitToMaterial : MonoBehaviour {
    [SerializeField] Material _material;

    void OnRenderImage(RenderTexture source, RenderTexture destination) {
      Graphics.Blit(source, destination, this._material);
    }
  }
}