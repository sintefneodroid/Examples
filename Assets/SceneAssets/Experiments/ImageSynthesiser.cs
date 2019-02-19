using System;
using System.Collections;
using System.IO;
using droid.Runtime.Utilities.NeodroidCamera.Synthesis;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace SceneAssets.Experiments {
  /// <inheritdoc />
  ///  <summary>
  ///  </summary>
  [RequireComponent(typeof(Camera))]
  public class ImageSynthesiser : MonoBehaviour {


    /// <summary>
    ///
    /// </summary>
    [SerializeField]
    Shader segmentationShader;

    /// <summary>
    ///
    /// </summary>
    [SerializeField]
    Shader opticalFlowShader;

    /// <summary>
    ///
    /// </summary>
    [SerializeField]
    float opticalFlowSensitivity = 1.0f;

    [SerializeField] Material _optical_flow_material;
    [SerializeField] Camera _camera;

    void Awake() {
      // default fallbacks, if shaders are unspecified
      if (!this.segmentationShader) {
        this.segmentationShader = Shader.Find("Neodroid/Segmentation/SimpleSegmentation");
      }

      if (!this.opticalFlowShader) {
        this.opticalFlowShader = Shader.Find("Neodroid/OpticalFlow");
      }
    }

    void Start() {
      this._camera = this.GetComponent<Camera>();

      SynthesisUtils.Setup(this._camera);
      SynthesisUtils.OnCameraChangeFull(this._camera,
                                        this.segmentationShader,
                                        this.opticalFlowShader,
                                        this._optical_flow_material,
                                        this.opticalFlowSensitivity);
      OnSceneChange();
    }

    void LateUpdate() {
      #if UNITY_EDITOR
      if (this.DetectPotentialSceneChangeInEditor()) {
        OnSceneChange();
      }
      #endif // UNITY_EDITOR

      SynthesisUtils.OnCameraChangeFull(this._camera,
                                        this.segmentationShader,
                                        this.opticalFlowShader,
                                        this._optical_flow_material,
                                        this.opticalFlowSensitivity);
    }



    /// <summary>
    ///
    /// </summary>
    public static void OnSceneChange() {
      var renderers = FindObjectsOfType<Renderer>();
      var mpb = new MaterialPropertyBlock();
      foreach (var r in renderers) {
        GameObject game_object;
        var id = (game_object = r.gameObject).GetInstanceID();
        var layer = game_object.layer;
        var tag_ = game_object.tag;

        mpb.SetColor("_ObjectColor", ColorEncoding.EncodeIdAsColor(id));
        mpb.SetColor("_CategoryColor", ColorEncoding.EncodeLayerAsColor(layer));
        r.SetPropertyBlock(mpb);
      }
    }

    public void Save(string filename, int width = -1, int height = -1, string path = "") {
      if (width <= 0 || height <= 0) {
        width = Screen.width;
        height = Screen.height;
      }

      var filename_extension = Path.GetExtension(filename);
      if (filename_extension == "") {
        filename_extension = ".png";
      }

      var filename_without_extension = Path.GetFileNameWithoutExtension(filename);

      var path_without_extension = Path.Combine(path, filename_without_extension);

      // execute as coroutine to wait for the EndOfFrame before starting capture
      this.StartCoroutine(this.WaitForEndOfFrameAndSave(path_without_extension,
                                                        filename_extension,
                                                        width,
                                                        height));
    }

    IEnumerator WaitForEndOfFrameAndSave(string filename_without_extension,
                                         string filename_extension,
                                         int width,
                                         int height) {
      yield return new WaitForEndOfFrame();
      this.Save(filename_without_extension, filename_extension, width, height);
    }

    void Save(string filename_without_extension, string filename_extension, int width, int height) {
      foreach (var pass in SynthesisUtils.capture_passes) {
        this.Save(pass._Camera,
                  filename_without_extension + pass._Name + filename_extension,
                  width,
                  height,
                  pass._SupportsAntialiasing,
                  pass._NeedsRescale);
      }
    }

    void Save(Camera cam,
              string filename,
              int width,
              int height,
              bool supports_antialiasing,
              bool needs_rescale) {
      const Int32 depth = 24;
      const RenderTextureFormat format = RenderTextureFormat.Default;
      const RenderTextureReadWrite read_write = RenderTextureReadWrite.Default;
      var anti_aliasing = (supports_antialiasing) ? Mathf.Max(1, QualitySettings.antiAliasing) : 1;

      var final_rt = RenderTexture.GetTemporary(width, height, depth, format, read_write, anti_aliasing);
      var render_rt = (!needs_rescale)
                          ? final_rt
                          : RenderTexture.GetTemporary(this._camera.pixelWidth,
                                                       this._camera.pixelHeight,
                                                       depth,
                                                       format,
                                                       read_write,
                                                       anti_aliasing);
      var tex = new Texture2D(width, height, TextureFormat.RGB24, false);

      var prev_active_rt = RenderTexture.active;
      var prev_camera_rt = cam.targetTexture;

      // render to offscreen texture (readonly from CPU side)
      RenderTexture.active = render_rt;
      cam.targetTexture = render_rt;

      cam.Render();

      if (needs_rescale) {
        // blit to rescale (see issue with Motion Vectors in @KNOWN ISSUES)
        RenderTexture.active = final_rt;
        Graphics.Blit(render_rt, final_rt);
        RenderTexture.ReleaseTemporary(render_rt);
      }

      // read offsreen texture contents into the CPU readable texture
      tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
      tex.Apply();

      // encode texture into PNG
      var bytes = tex.EncodeToPNG();
      File.WriteAllBytes(filename, bytes);

      // restore state and cleanup
      cam.targetTexture = prev_camera_rt;
      RenderTexture.active = prev_active_rt;

      Destroy(tex);
      RenderTexture.ReleaseTemporary(final_rt);
    }

    #if UNITY_EDITOR
    GameObject _last_selected_go;
    int _last_selected_go_layer = -1;
    string _last_selected_go_tag = "unknown";

    bool DetectPotentialSceneChangeInEditor() {
      var change = false;
      // there is no callback in Unity Editor to automatically detect changes in scene objects
      // as a workaround lets track selected objects and check, if properties that are
      // interesting for us (layer or tag) did not change since the last frame
      if (Selection.transforms.Length > 1) {
        // multiple objects are selected, all bets are off!
        // we have to assume these objects are being edited
        change = true;
        this._last_selected_go = null;
      } else if (Selection.activeGameObject) {
        var go = Selection.activeGameObject;
        // check if layer or tag of a selected object have changed since the last frame
        var potential_change_happened =
            this._last_selected_go_layer != go.layer || this._last_selected_go_tag != go.tag;
        if (go == this._last_selected_go && potential_change_happened) {
          change = true;
        }

        this._last_selected_go = go;
        this._last_selected_go_layer = go.layer;
        this._last_selected_go_tag = go.tag;
      }

      return change;
    }
    #endif // UNITY_EDITOR
  }
}
