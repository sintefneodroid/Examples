using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = System.Object;

namespace SceneAssets.Experiments {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  [RequireComponent(typeof(ImageSynthesiser))]
  public class CaptureButtonOverlay : MonoBehaviour {
    [SerializeField] int width = 320;
    [SerializeField] int height = 200;
    [SerializeField] int _image_counter = 1;
    [SerializeField] ImageSynthesiser image_synthesiser;
    [SerializeField] string scene_name;

    void Awake() {
      this.image_synthesiser = this.GetComponent<ImageSynthesiser>();
      this.scene_name = SceneManager.GetActiveScene().name;
    }

    void OnGUI() {
      if (GUILayout.Button($"Capture ({this._image_counter})")) {
        this.image_synthesiser.Save(this.scene_name + "_" + this._image_counter++, this.width, this.height);
      }
    }
  }
}
