#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SceneAssets.Excluded.Hide.SharpShadowLight.Scripts.Utilities {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  public class SharpShadowLightMenu : Editor {
    /// <summary>
    /// 
    /// </summary>
    const string _menu_path = "GameObject/SharpShadowLight";

    internal static Lights.SharpShadowLight _Light;
    static string _folder_path;

    /// <summary>
    /// 
    /// </summary>
    [MenuItem(_menu_path + "/Lights/ ☀ Radial No Material ", false, 21)]
    static void AddRadialNoMat() {
      _folder_path = EditorUtils.GetMainRelativepath();

      //Object prefab = AssetDatabase.LoadAssetAtPath(folderPath + "Prefabs/Lights/2DPointLight.prefab", typeof(GameObject));
      var hex = new GameObject(
          "Light2D"); //Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
      _Light = hex.AddComponent<Lights.SharpShadowLight>();
      _Light._Layer = 255;
      hex.transform.position = new Vector3(0, 0, 0);
    }

    /// <summary>
    /// 
    /// </summary>
    [MenuItem(_menu_path + "/Lights/ ☀ Radial Procedural Gradient ", false, 31)]
    static void AddRadialGradient() {
      _folder_path = EditorUtils.GetMainRelativepath();

      var mate = AssetDatabase.LoadAssetAtPath(
                     _folder_path + "Materials/LightMaterialGradient.mat",
                     typeof(Material)) as Material;
      var hex = new GameObject(
          "Light2D"); //Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
      _Light = hex.AddComponent<Lights.SharpShadowLight>();
      _Light._Layer = 255;
      hex.transform.position = new Vector3(0, 0, 0);
      _Light._Light_Material = mate;
    }

    /// <summary>
    /// 
    /// </summary>
    [MenuItem(_menu_path + "/Lights/ ☀ Pseudo Spot Light ", false, 41)]
    static void AddPseudo() {
      _folder_path = EditorUtils.GetMainRelativepath();

      var prefab = AssetDatabase.LoadAssetAtPath(
          _folder_path + "Prefabs/Lights/2DPseudoSpotLight.prefab",
          typeof(GameObject));
      var hex = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
      if (hex != null) {
        hex.transform.position = new Vector3(0, 0, 0);
        hex.name = "2DRadialGradientPoint";
        _Light = hex.GetComponent<Lights.SharpShadowLight>();
      }

      //light.layer = 255;
    }

    #region Casters Zone

    [MenuItem(_menu_path + "/Casters/Square", false, 66)]
    static void AddSquare() {
      _folder_path = EditorUtils.GetMainRelativepath();
      var prefab = AssetDatabase.LoadAssetAtPath(
          _folder_path + "Prefabs/Casters/square.prefab",
          typeof(GameObject));
      var hex = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
      if (hex != null) {
        hex.transform.position = new Vector3(5, 0, 0);
        //hex.layer = LayerMask.NameToLayer("shadows");
        hex.name = "Square";
      }
    }

    [MenuItem(_menu_path + "/Casters/Hexagon", false, 67)]
    static void AddHexagon() {
      _folder_path = EditorUtils.GetMainRelativepath();
      var prefab = AssetDatabase.LoadAssetAtPath(
          _folder_path + "Prefabs/Casters/hexagon.prefab",
          typeof(GameObject));
      var hex = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
      if (hex != null) {
        hex.transform.position = new Vector3(5, 0, 0);
        //hex.layer = LayerMask.NameToLayer("shadows");
        hex.name = "Hexagon";
      }
    }

    [MenuItem(_menu_path + "/Casters/Pacman", false, 68)]
    static void AddPacman() {
      _folder_path = EditorUtils.GetMainRelativepath();
      var prefab = AssetDatabase.LoadAssetAtPath(
          _folder_path + "Prefabs/Casters/pacman.prefab",
          typeof(GameObject));
      var hex = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
      if (hex != null) {
        hex.transform.position = new Vector3(5, 0, 0);
        //hex.layer = LayerMask.NameToLayer("shadows");
        hex.name = "Pacman";
      }
    }

    [MenuItem(_menu_path + "/Casters/Star", false, 69)]
    static void AddStar() {
      _folder_path = EditorUtils.GetMainRelativepath();
      var prefab = AssetDatabase.LoadAssetAtPath(
          _folder_path + "Prefabs/Casters/star.prefab",
          typeof(GameObject));
      var hex = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
      if (hex != null) {
        hex.transform.position = new Vector3(5, 0, 0);
        //hex.layer = LayerMask.NameToLayer("shadows");
        hex.name = "Star";
      }
    }

    #endregion
  }
}
#endif
