using System;
using UnityEngine;

namespace SceneAssets.Experiments.ScriptedManipulator.Utilities {
  [ExecuteInEditMode]
  public class QuickGlow : MonoBehaviour {
    [SerializeField] Material _add_material = null;
    [SerializeField] Material _blur_material = null;

    [Range(0, 4)] public int _Down_Res = 0;

    [Range(0, 3)] public float _Intensity = 0;

    [Range(0, 10)] public int _Iterations = 0;

    [Range(0, 10)] public float _Size = 0;
    static readonly Int32 _size = Shader.PropertyToID("_Size");
    static readonly Int32 _intensity = Shader.PropertyToID("_Intensity");
    static readonly Int32 _blend_tex = Shader.PropertyToID("_BlendTex");

    void OnValidate() {
      if (this._blur_material != null) {
        this._blur_material.SetFloat(_size, this._Size);
      }

      if (this._add_material != null) {
        this._add_material.SetFloat(_intensity, this._Intensity);
      }
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst) {
      var composite = RenderTexture.GetTemporary(src.width, src.height);
      Graphics.Blit(src, composite);

      var width = src.width >> this._Down_Res;
      var height = src.height >> this._Down_Res;

      var rt = RenderTexture.GetTemporary(width, height);
      Graphics.Blit(src, rt);

      for (var i = 0; i < this._Iterations; i++) {
        var rt2 = RenderTexture.GetTemporary(width, height);
        Graphics.Blit(rt, rt2, this._blur_material);
        RenderTexture.ReleaseTemporary(rt);
        rt = rt2;
      }

      this._add_material.SetTexture(_blend_tex, rt);
      Graphics.Blit(composite, dst, this._add_material);

      RenderTexture.ReleaseTemporary(rt);
      RenderTexture.ReleaseTemporary(composite);
    }
  }
}
