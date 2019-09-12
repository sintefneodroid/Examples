using System.Collections.Generic;
using droid.Runtime.Structs.Space;
using droid.Runtime.Structs.Space.Sample;
using UnityEngine;

namespace SceneAssets.Catch {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  public class DropSpawner : MonoBehaviour {
    [SerializeField] int _max_num_concurrent_objects = 4;
    [SerializeField] float _next_spawn_time = 0f;

    [SerializeField]
    SampleSpace1 _spawn_delay_range = new SampleSpace1 {Space = new Space1 {Max = 1f, Min = 0.1f}};

    /// <summary>
    /// </summary>
    [SerializeField]
    List<GameObject> _spawn_objects = null;

    /// <summary>
    /// </summary>
    [SerializeField]
    List<Transform> _spawn_poses = null;

    [SerializeField]
    SampleSpace3 _spawn_position_range =
        new SampleSpace3() {Space = new Space3 {Max = new Vector3(5, 0), Min = new Vector3(-5, 0)}};

    [SerializeField]
    SampleSpace4 _spawn_rotation_range =
        new SampleSpace4 {Space = new Space4 {Max = Vector4.one, Min = -Vector4.one}};

    [SerializeField] List<GameObject> _spawned_objects = null;

    [SerializeField] bool _use_predefined_spawn_locations = false;

    void Start() { this._next_spawn_time = 0; }

    void SpawnObjects() {
      if (this._spawn_objects.Count > 0) {
        if (this._next_spawn_time < Time.time) {
          if (this._spawned_objects.Count < this._max_num_concurrent_objects) {
            var object_selection = Random.Range(0, this._spawn_objects.Count);
            Pose pose;
            if (this._use_predefined_spawn_locations) {
              var position_selection = Random.Range(0, this._spawn_poses.Count);
              var trans = this._spawn_poses[position_selection];
              pose = new Pose(trans.position, trans.rotation);
            } else {
              pose = new Pose(this._spawn_position_range.Sample(), this._spawn_rotation_range.Sample());
            }

            Instantiate(this._spawn_objects[object_selection], pose.position, pose.rotation);
            var delay = this._spawn_delay_range.Sample();
            this._next_spawn_time = Time.time + delay;
          }
        }
      }
    }

    // Update is called once per frame
    void Update() { this.SpawnObjects(); }
  }
}
