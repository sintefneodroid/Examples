using UnityEngine;

namespace SceneAssets.Excluded.Hide.SharpShadowLight.Scripts.Utilities {
  /// <summary>
  /// 
  /// </summary>
  public static class SinCosTable {
    /// <summary>
    /// 
    /// </summary>
    static bool _instanced;

    /// <summary>
    /// 
    /// </summary>
    public static float[] _Sen_Array;

    /// <summary>
    /// 
    /// </summary>
    public static float[] _Cos_Array;

    /// <summary>
    /// 
    /// </summary>
    public static void Init() {
      if (_instanced) {
        return;
      }

      _Sen_Array = new float[360];
      _Cos_Array = new float[360];

      for (var i = 0; i < 360; i++) {
        _Sen_Array[i] = Mathf.Sin(i * Mathf.Deg2Rad);
        _Cos_Array[i] = Mathf.Cos(i * Mathf.Deg2Rad);
      }

      _instanced = true;
    }
  }
}
