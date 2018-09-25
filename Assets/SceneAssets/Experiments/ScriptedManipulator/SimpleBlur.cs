using UnityEngine;

namespace SceneAssets.Experiments.ScriptedManipulator {
  [ExecuteInEditMode]
  public class SimpleBlur : MonoBehaviour {
    [SerializeField] Material _background;

    void OnRenderImage(RenderTexture src, RenderTexture dst) { Graphics.Blit(src, dst, this._background); }
  }
}