namespace SceneAssets.Experiments.ScriptedManipulator.Utilities.DataCollection.Experimental {
  using ImageConversion = UnityEngine.ImageConversion;

  public class ImageRecorder : UnityEngine.MonoBehaviour {
    [UnityEngine.SerializeField] UnityEngine.Camera _camera;
    readonly string _file_path = @"training_data/shadow/";

    int _i;

    void Start() {
      if (!this._camera) {
        this._camera = this.GetComponent<UnityEngine.Camera>();
      }
    }

    void Update() {
      this.SaveRenderTextureToImage(id : this._i, cam : this._camera, file_name_dd : this._file_path);

      this._i++;
    }

    public void SaveRenderTextureToImage(int id, UnityEngine.Camera cam, string file_name_dd) {
      var texture2_d = RenderTextureImage(camera : cam);
      var data = ImageConversion.EncodeToPNG(texture2_d);
      var file_name = file_name_dd + id + ".png";
      System.IO.File.WriteAllBytes(path : file_name, bytes : data);
    }

    public static UnityEngine.Texture2D RenderTextureImage(UnityEngine.Camera camera) {
      // From unity documentation, https://docs.unity3d.com/ScriptReference/Camera.Render.html
      var current_render_texture = UnityEngine.RenderTexture.active;
      UnityEngine.RenderTexture.active = camera.targetTexture;
      camera.Render();
      var image = new UnityEngine.Texture2D(width : camera.targetTexture.width,
                                            height : camera.targetTexture.height);
      image.ReadPixels(source : new UnityEngine.Rect(0,
                                                     0,
                                                     width : camera.targetTexture.width,
                                                     height : camera.targetTexture.height),
                       destX : 0,
                       destY : 0);
      image.Apply();
      UnityEngine.RenderTexture.active = current_render_texture;
      return image;
    }
  }
}