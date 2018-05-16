#if UNITY_EDITOR

using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;

namespace SceneAssets.Excluded.Hide.SharpShadowLight.Scripts.Utilities {
  /// <summary>
  /// 
  /// </summary>
  public class EditorUtils : Editor {
    public static EditorUtils _EditorUtils;

    internal static string _Relativepath;

    public static string GetMainRelativepath() {
      if (_EditorUtils == null) {
        _EditorUtils = (EditorUtils)CreateInstance("EditorUtils");
      }

      if (_Relativepath != null) {
        return _Relativepath;
      }

      var ms = MonoScript.FromScriptableObject(_EditorUtils);
      var m_script_file_path = AssetDatabase.GetAssetPath(ms);

      var name = "Scripts/2DynaLight/Editor/" + Path.GetFileName(m_script_file_path);

      var rex = new Regex(name);
      var result = rex.Replace(m_script_file_path, "", 1);

      _Relativepath = result;
      return _Relativepath;
    }
  }
}
#endif
