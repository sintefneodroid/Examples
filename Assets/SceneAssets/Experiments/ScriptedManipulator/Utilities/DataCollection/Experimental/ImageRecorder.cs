using System.IO;
using UnityEngine;

namespace SceneAssets.Experiments.ScriptedManipulator.Utilities.DataCollection.Experimental {
  public class ImageRecorder : MonoBehaviour {
    readonly string _file_path = @"training_data/shadow/";

    [SerializeField] Camera _camera;

    int _i;

    void Start() {
      if (!this._camera) {
        this._camera = this.GetComponent<Camera>();
      }
    }

    void Update() {
      this.SaveRenderTextureToImage(this._i, this._camera, this._file_path);

      this._i++;
    }

    public void SaveRenderTextureToImage(int id, Camera cam, string file_name_dd) {
      var texture2_d = RenderTextureImage(cam);
      var data = texture2_d.EncodeToPNG();
      var file_name = file_name_dd + id + ".png";
      File.WriteAllBytes(file_name, data);
    }

    public static Texture2D RenderTextureImage(Camera camera) {
      // From unity documentation, https://docs.unity3d.com/ScriptReference/Camera.Render.html
      var current_render_texture = RenderTexture.active;
      RenderTexture.active = camera.targetTexture;
      camera.Render();
      var image = new Texture2D(camera.targetTexture.width, camera.targetTexture.height);
      image.ReadPixels(new Rect(0,
                                0,
                                camera.targetTexture.width,
                                camera.targetTexture.height),
                       0,
                       0);
      image.Apply();
      RenderTexture.active = current_render_texture;
      return image;
    }
  }
}
