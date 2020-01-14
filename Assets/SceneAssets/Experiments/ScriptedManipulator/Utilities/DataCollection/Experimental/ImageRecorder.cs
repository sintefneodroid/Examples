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
      this.SaveRenderTextureToImage(id : this._i, cam : this._camera, file_name_dd : this._file_path);

      this._i++;
    }

    public void SaveRenderTextureToImage(int id, Camera cam, string file_name_dd) {
      var texture2_d = RenderTextureImage(camera : cam);
      var data = texture2_d.EncodeToPNG();
      var file_name = file_name_dd + id + ".png";
      File.WriteAllBytes(path : file_name, bytes : data);
    }

    public static Texture2D RenderTextureImage(Camera camera) {
      // From unity documentation, https://docs.unity3d.com/ScriptReference/Camera.Render.html
      var current_render_texture = RenderTexture.active;
      RenderTexture.active = camera.targetTexture;
      camera.Render();
      var image = new Texture2D(width : camera.targetTexture.width, height : camera.targetTexture.height);
      image.ReadPixels(new Rect(0,
                                0,
                                width : camera.targetTexture.width,
                                height : camera.targetTexture.height),
                       0,
                       0);
      image.Apply();
      RenderTexture.active = current_render_texture;
      return image;
    }
  }
}
