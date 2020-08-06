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
    static readonly int _size = Shader.PropertyToID("_Size");
    static readonly int _intensity = Shader.PropertyToID("_Intensity");
    static readonly int _blend_tex = Shader.PropertyToID("_BlendTex");

    void OnValidate() {
      if (this._blur_material != null) {
        this._blur_material.SetFloat(nameID : _size, value : this._Size);
      }

      if (this._add_material != null) {
        this._add_material.SetFloat(nameID : _intensity, value : this._Intensity);
      }
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst) {
      var composite = RenderTexture.GetTemporary(width : src.width, height : src.height);
      Graphics.Blit(source : src, dest : composite);

      var width = src.width >> this._Down_Res;
      var height = src.height >> this._Down_Res;

      var rt = RenderTexture.GetTemporary(width : width, height : height);
      Graphics.Blit(source : src, dest : rt);

      for (var i = 0; i < this._Iterations; i++) {
        var rt2 = RenderTexture.GetTemporary(width : width, height : height);
        Graphics.Blit(source : rt, dest : rt2, mat : this._blur_material);
        RenderTexture.ReleaseTemporary(temp : rt);
        rt = rt2;
      }

      this._add_material.SetTexture(nameID : _blend_tex, value : rt);
      Graphics.Blit(source : composite, dest : dst, mat : this._add_material);

      RenderTexture.ReleaseTemporary(temp : rt);
      RenderTexture.ReleaseTemporary(temp : composite);
    }
  }
}
