using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Common.AsmDefTools.Editor {
  public class AsmDefIgnore : ScriptableObject {
    [SerializeField] List<string> _ignored_asm_defs = new List<string>();

    public bool Add(string file_name) {
      if (this._ignored_asm_defs.Contains(file_name)) {
        Debug.LogErrorFormat("Filename already exists {0}, not adding", file_name);
        return false;
      }

      Debug.Log("Adding " + file_name);
      EditorUtility.SetDirty(this);
      this._ignored_asm_defs.Add(file_name);
      return true;
    }

    public bool Remove(string file_name) {
      if (!this._ignored_asm_defs.Contains(file_name)) {
        Debug.LogErrorFormat("Filename does not exist {0}, not removing", file_name);
      }

      Debug.Log("Removing " + file_name);
      EditorUtility.SetDirty(this);
      var removed = this._ignored_asm_defs.Remove(file_name);
      return removed;
    }

    public bool Contains(string file_name) { return this._ignored_asm_defs.Contains(file_name); }
  }
}