using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Common.AsmDefTools.Editor {
  public static class AsmDefMenu {
    const string _asm_def_extension = ".AsmDef";
    const string _asm_def_ignore_name = "AsmDef_Ignore.asset";
    const string _asm_def_ignore_path = "Assets/AsmDefTools";
    static readonly string _save_key = "HIDDEN" + Application.unityVersion;

    static AsmDefIgnore _ignore;

    static AsmDefIgnore Ignore {
      get {
        if (_ignore == null) {
          var asset_path = Path.Combine(_asm_def_ignore_path, _asm_def_ignore_name);
          _ignore = AssetDatabase.LoadAssetAtPath<AsmDefIgnore>(asset_path);
          if (_ignore == null) {
            Debug.Log("Creating AsmDefIgnore.asset for first use");
            _ignore = ScriptableObject.CreateInstance<AsmDefIgnore>();
            _ignore.name = _asm_def_ignore_name + _asm_def_extension;
            AssetDatabase.CreateAsset(_ignore, asset_path);
            AssetDatabase.SaveAssets();
          }
        }

        return _ignore;
      }
    }

    static bool Hidden {
      get { return EditorPrefs.GetInt(_save_key) == 1; }
      set { EditorPrefs.SetInt(_save_key, value ? 1 : 0); }
    }

    static string GetFileName(string path) {
      if (path.IsHidden()) {
        path = path.Remove(path.Length - 1, 1);
      }

      return Path.GetFileName(path);
    }

    static bool ToggleFile(string path, bool enable) {
      if (string.IsNullOrEmpty(path)) {
        Debug.LogError("no valid path given, ignored");
      }

      if (enable == !path.IsHidden()) {
        Debug.LogWarning("the path already has the requested state, ignored");
        return false;
      } else {
        File.Move(path, path.Toggle(enable));
      }

      return true;
    }

    #region Menu

    const string _menu_path = "Tools/";

    [MenuItem(_menu_path + "AsmDef/Toggle AsmDef")]
    static void ToggleAsmDef() {
      var files = Directory.GetFiles(
          Application.dataPath,
          "*" + _asm_def_extension + "*",
          SearchOption.AllDirectories);

      var active = new List<string>(files.Length);
      var inactive = new List<string>(files.Length);

      foreach (var file in files) {
        var file_name = GetFileName(file);
        var file_name_only = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(file_name));
        if (!Ignore.Contains(file_name_only)) {
          if (file.IsHidden()) {
            inactive.Add(file);
          } else {
            active.Add(file);
          }
        } else {
          Debug.LogFormat("{0} will be ignored since it's ignored, lol", file_name);
        }
      }

      Debug.LogFormat("Active AsmDef and meta files {0}, inactive files {1}", active.Count, inactive.Count);

      var toggle_list = Hidden ? inactive : active;
      foreach (var item in toggle_list) {
        if (!ToggleFile(item, Hidden)) {
          Debug.LogErrorFormat("Something went wrong, check");
        } else {
          Debug.LogFormat("converted {0}", item);
        }
      }

      Hidden = !Hidden;

      if (active.Count != 0 && inactive.Count != 0) {
        Debug.LogWarning("not all AsmDefs are toggled");
        var string_list = active.Count < inactive.Count ? active : inactive;
        foreach (var s in string_list) {
          Debug.LogWarningFormat("AsmDef that is different {0}", s);
        }
      }

      //AssetDatabase.Refresh(ImportAssetOptions.Default);
    }

    [MenuItem(_menu_path + "AsmDef/Toggle AsmDef", validate = true)]
    static bool ToggleAsmDefValidate() {
      Menu.SetChecked("AsmDef/Toggle AsmDef", Hidden);
      return true;
    }

    [MenuItem(_menu_path + "Assets/Keep AsmDef active")]
    static void KeepAsmDefActive() {
      var file_name = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(Selection.activeObject));
      var active = Ignore.Contains(file_name);

      if (!active) {
        Ignore.Add(file_name);
      } else {
        Ignore.Remove(file_name);
      }

      Menu.SetChecked("Assets/Keep AsmDef active", !active);
    }

    [MenuItem(_menu_path + "Assets/Keep AsmDef active", validate = true)]
    static bool KeepAsmDefActiveValidate() {
      if (Selection.activeObject == null) {
        return false;
      }

      var asset_path = AssetDatabase.GetAssetPath(Selection.activeObject);

      if (!asset_path.Contains(_asm_def_extension)) {
        return false;
      }

      Menu.SetChecked("Assets/Keep AsmDef active", Ignore.Contains(Selection.activeObject.name));
      return AssetDatabase.GetAssetPath(Selection.activeObject).Contains(_asm_def_extension);
    }

    #endregion
  }
}
