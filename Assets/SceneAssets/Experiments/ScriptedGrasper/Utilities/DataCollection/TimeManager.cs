using UnityEngine;

namespace SceneAssets.Experiments.ScriptedGrasper.Utilities.DataCollection {
  public class TimeManager : MonoBehaviour {
    [Range(0.0f, 10.0f)] [SerializeField] float _time_scale = 1f;
    float _interval_size = 0.02f;

    // Use this for initialization
    void Start() {
      Time.timeScale = this._time_scale;
      Time.fixedDeltaTime = this._interval_size * Time.timeScale;
    }

    // Update is called once per frame
    void Update() { }
  }
}
