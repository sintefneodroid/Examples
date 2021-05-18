namespace SceneAssets.Experiments.ScriptedManipulator.Utilities {
  [UnityEngine.ExecuteInEditMode]
  public class QuickGlow : UnityEngine.MonoBehaviour {
    static readonly int _size = UnityEngine.Shader.PropertyToID("_Size");
    static readonly int _intensity = UnityEngine.Shader.PropertyToID("_Intensity");
    static readonly int _blend_tex = UnityEngine.Shader.PropertyToID("_BlendTex");
    [UnityEngine.SerializeField] UnityEngine.Material _add_material = null;
    [UnityEngine.SerializeField] UnityEngine.Material _blur_material = null;

    [UnityEngine.RangeAttribute(0, 4)] public int _Down_Res = 0;

    [UnityEngine.RangeAttribute(0, 3)] public float _Intensity = 0;

    [UnityEngine.RangeAttribute(0, 10)] public int _Iterations = 0;

    [UnityEngine.RangeAttribute(0, 10)] public float _Size = 0;

    void OnRenderImage(UnityEngine.RenderTexture src, UnityEngine.RenderTexture dst) {
      var composite = UnityEngine.RenderTexture.GetTemporary(width : src.width, height : src.height);
      UnityEngine.Graphics.Blit(source : src, dest : composite);

      var width = src.width >> this._Down_Res;
      var height = src.height >> this._Down_Res;

      var rt = UnityEngine.RenderTexture.GetTemporary(width : width, height : height);
      UnityEngine.Graphics.Blit(source : src, dest : rt);

      for (var i = 0; i < this._Iterations; i++) {
        var rt2 = UnityEngine.RenderTexture.GetTemporary(width : width, height : height);
        UnityEngine.Graphics.Blit(source : rt, dest : rt2, mat : this._blur_material);
        UnityEngine.RenderTexture.ReleaseTemporary(temp : rt);
        rt = rt2;
      }

      this._add_material.SetTexture(nameID : _blend_tex, value : rt);
      UnityEngine.Graphics.Blit(source : composite, dest : dst, mat : this._add_material);

      UnityEngine.RenderTexture.ReleaseTemporary(temp : rt);
      UnityEngine.RenderTexture.ReleaseTemporary(temp : composite);
    }

    void OnValidate() {
      if (this._blur_material != null) {
        this._blur_material.SetFloat(nameID : _size, value : this._Size);
      }

      if (this._add_material != null) {
        this._add_material.SetFloat(nameID : _intensity, value : this._Intensity);
      }
    }
  }
}