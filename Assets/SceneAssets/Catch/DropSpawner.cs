using System.Collections.Generic;
using Neodroid.Runtime.Utilities.Structs;
using UnityEngine;

namespace SceneAssets.Catch {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  public class DropSpawner : MonoBehaviour {
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    List<Transform> _spawn_poses;

    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    List<GameObject> _spawn_objects;

    [SerializeField] List<GameObject> _spawned_objects;

    [SerializeField] bool _use_predefined_spawn_locations;

    [SerializeField]
    Space3 _spawn_position_range =
        new Space3 {_Max_Values = new Vector3(5, 0), _Min_Values = new Vector3(-5, 0)};

    [SerializeField]
    Space4 _spawn_rotation_range = new Space4 {_Max_Values = Vector4.one, _Min_Values = -Vector4.one};

    [SerializeField] int _max_num_concurrent_objects = 4;
    [SerializeField] ValueSpace _spawn_delay_range = new ValueSpace {_Max_Value = 1f, _Min_Value = 0.1f};
    [SerializeField] float _next_spawn_time;

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
              pose = new Pose(
                  this._spawn_position_range.RandomVector3(),
                  this._spawn_rotation_range.RandomQuaternion());
            }

            Instantiate(this._spawn_objects[object_selection], pose.position, pose.rotation);
            var delay = this._spawn_delay_range.RandomValue();
            this._next_spawn_time = Time.time + delay;
          }
        }
      }
    }

    // Update is called once per frame
    void Update() { this.SpawnObjects(); }
  }
}