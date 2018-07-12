#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace SceneAssets.Excluded.Hide.SharpShadowLight.Scripts.Utilities {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  [CustomEditor(typeof(Lights.SharpShadowLight)), CanEditMultipleObjects]
  public class SharpShadowLightEditor : Editor {
    /// <summary>
    /// 
    /// </summary>
    internal static Lights.SharpShadowLight _Light;

    /// <summary>
    /// 
    /// </summary>
    //GUIStyle _title_style, _sub_title_style, _bg_style;

    /// <summary>
    /// 
    /// </summary>
    SerializedProperty _lmaterial, _radius, _segments, _layer;

    /// <summary>
    /// 
    /// </summary>
    const Double _tolerance = Double.Epsilon;

    /// <summary>
    /// 
    /// </summary>
    /*
    internal void InitStyles() {
      this._title_style = new GUIStyle(GUI.skin.label) {
          fontSize = 15,
          fontStyle = FontStyle.Bold,
          alignment = TextAnchor.MiddleLeft,
          margin = new RectOffset(4, 4, 10, 0)
      };

      this._sub_title_style = new GUIStyle(GUI.skin.label) {
          fontSize = 13,
          fontStyle = FontStyle.Bold,
          alignment = TextAnchor.MiddleLeft,
          margin = new RectOffset(4, 4, 10, 0)
      };

      this._bg_style = new GUIStyle(GUI.skin.button) {
          margin = new RectOffset(4, 4, 0, 4),
          padding = new RectOffset(1, 1, 1, 2),
          fixedHeight = 30f
      };
    }*/

    /// <summary>
    /// 
    /// </summary>
    internal void OnEnable() {
      _Light = this.target as Lights.SharpShadowLight;

      this._lmaterial = this.serializedObject.FindProperty("_Light_Material");
      this._radius = this.serializedObject.FindProperty("_Light_Radius");
      this._segments = this.serializedObject.FindProperty("_Light_Segments");
      this._layer = this.serializedObject.FindProperty("_Layer");
    }

    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public override void OnInspectorGUI() {
      if (_Light == null) {
        return;
      }

      //this.InitStyles();

      EditorGUI.BeginChangeCheck();
      {
        var fradius = Mathf.Abs(this._radius.floatValue);

        if (Math.Abs(this._radius.floatValue - fradius) > _tolerance) {
          this._radius.floatValue = fradius;
        }

        EditorGUILayout.PropertyField(this._radius, new GUIContent("Radius", "Size of light radius"));
        EditorGUILayout.IntSlider(
            this._segments,
            3,
            20,
            new GUIContent(
                "Segments",
                "Quantity of line segments is used for build mesh render of 2DynaLight. 3 at least"));
        EditorGUILayout.PropertyField(
            this._lmaterial,
            new GUIContent("Light Material", "Material Object used for render into light mesh"));

        EditorGUILayout.PropertyField(this._layer, new GUIContent("Layer", "Layer"));
      }

      if (EditorGUI.EndChangeCheck()) {
        EditorUtility.SetDirty(this.target);
        this.ApplyChanges();
      }
    }

    /// <summary>
    /// 
    /// </summary>
    void ApplyChanges() {
      Undo.RecordObject(_Light, "Apply changes");

      foreach (var o in this.targets) {
        var s = (Lights.SharpShadowLight)o;
        s._Light_Material = (Material)this._lmaterial.objectReferenceValue;
        s._Light_Radius = this._radius.floatValue;
        s._Light_Segments = this._segments.intValue;
        s._Layer = this._layer.intValue;
      }
    }
  }
}
#endif
