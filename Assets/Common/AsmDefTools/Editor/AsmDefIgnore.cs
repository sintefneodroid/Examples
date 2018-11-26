/*
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Common.AsmDefTools.Editor {
  public class AsmDefIgnore : ScriptableObject {
    [SerializeField] List<string> _ignored_asm_definitions = new List<string>();

    public bool Add(string file_name) {
      if (this._ignored_asm_definitions.Contains(file_name)) {
        Debug.LogErrorFormat("Filename already exists {0}, not adding", file_name);
        return false;
      }

      Debug.Log("Adding " + file_name);
      EditorUtility.SetDirty(this);
      this._ignored_asm_definitions.Add(file_name);
      return true;
    }

    public bool Remove(string file_name) {
      if (!this._ignored_asm_definitions.Contains(file_name)) {
        Debug.LogErrorFormat("Filename does not exist {0}, not removing", file_name);
      }

      Debug.Log("Removing " + file_name);
      EditorUtility.SetDirty(this);
      var removed = this._ignored_asm_definitions.Remove(file_name);
      return removed;
    }

    public bool Contains(string file_name) { return this._ignored_asm_definitions.Contains(file_name); }
  }
}
*/
