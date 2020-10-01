﻿#if UNITY_EDITOR && BIOIK_EXISTS
using System;
using UnityEditor;
using UnityEngine;
using Exclude.BioIK;

namespace SceneAssets.Manipulator.Scripts {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  [CustomEditor(inspectedType : typeof(IkSolverEnablerConfigurable))]
  [CanEditMultipleObjects]
  public class EnablerConfigurableEditor : Editor {
    /// <summary>
    /// </summary>
    internal static IkSolverEnablerConfigurable _IkSolverEnablerConfigurable;

    /// <summary>
    /// </summary>
    SerializedProperty _enablee;

    SerializedProperty _enablee_script;

    Type _stype;

    /// <summary>
    /// </summary>
    internal void OnEnable() {
      _IkSolverEnablerConfigurable = this.target as IkSolverEnablerConfigurable;

      this._enablee = this.serializedObject.FindProperty("_Enablee");
    }

    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public override void OnInspectorGUI() {
      if (_IkSolverEnablerConfigurable == null) {
        return;
      }

      EditorGUI.BeginChangeCheck();
      {
        EditorGUILayout.ObjectField(property : this._enablee, objType : typeof(BioIK), label : new GUIContent("Enablee"));
        /* var e = this._enablee.objectReferenceValue as MonoBehaviour;
         if (e != null) {
           
           var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Default; // BindingFlags is located in System.Reflection - modify these to your liking to get the methods you're interested in
           comps = e.GetComponents<MonoBehaviour>().ToList();
           
           /*foreach (var mb in comps)
           {
             methods.AddRange(mb.GetMethods(flags));  
           }*/

        /*
        var iterator = comps.GetEnumerator();
        while (iterator.MoveNext()) {
          EditorGUILayout.ObjectField("Enablee Script",
              iterator.Current as MonoBehaviour,
              typeof(MonoBehaviour),
              true);
        }*/

        //EditorGUIUtility.ShowObjectPicker<MonoBehaviour>(this._enablee.objectReferenceValue, true, "", 0);
      }

      _IkSolverEnablerConfigurable._disable_every_next_frame =
          EditorGUILayout.Toggle("_disable_every_next_frame",
                                 value : _IkSolverEnablerConfigurable._disable_every_next_frame);
      _IkSolverEnablerConfigurable._disabled_by_default =
          EditorGUILayout.Toggle("_disable_by_default", value : _IkSolverEnablerConfigurable._disabled_by_default);

      if (GUILayout.Button("Toggle")) {
        _IkSolverEnablerConfigurable.ActiveToggle();
      }

      if (GUILayout.Button("Activate")) {
        _IkSolverEnablerConfigurable.Activate();
      }

      if (EditorGUI.EndChangeCheck()) {
        EditorUtility.SetDirty(target : this.target);
        this.ApplyChanges();
      }
    }

    /// <summary>
    /// </summary>
    void ApplyChanges() {
      Undo.RecordObject(objectToUndo : _IkSolverEnablerConfigurable, name : "Apply changes");

      foreach (var o in this.targets) {
        var s = (IkSolverEnablerConfigurable)o;
        s._Enablee = (BioIK)this._enablee.objectReferenceValue;
      }
    }
  }
}
#endif
