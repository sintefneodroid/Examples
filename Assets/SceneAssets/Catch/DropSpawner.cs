namespace SceneAssets.Catch {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  public class DropSpawner : UnityEngine.MonoBehaviour {
    [UnityEngine.SerializeField] int _max_num_concurrent_objects = 4;
    [UnityEngine.SerializeField] float _next_spawn_time = 0f;

    [UnityEngine.SerializeField]
    droid.Runtime.Structs.Space.Sample.SampleSpace1 _spawn_delay_range =
        new droid.Runtime.Structs.Space.Sample.SampleSpace1 {
                                                                Space =
                                                                    new droid.Runtime.Structs.Space.Space1 {
                                                                        Max = 1f, Min = 0.1f
                                                                    }
                                                            };

    /// <summary>
    /// </summary>
    [UnityEngine.SerializeField]
    System.Collections.Generic.List<UnityEngine.GameObject> _spawn_objects = null;

    /// <summary>
    /// </summary>
    [UnityEngine.SerializeField]
    System.Collections.Generic.List<UnityEngine.Transform> _spawn_poses = null;

    [UnityEngine.SerializeField]
    droid.Runtime.Structs.Space.Sample.SampleSpace3 _spawn_position_range =
        new droid.Runtime.Structs.Space.Sample.SampleSpace3 {
                                                                Space =
                                                                    new droid.Runtime.Structs.Space.Space3 {
                                                                        Max = new UnityEngine.Vector3(5, 0),
                                                                        Min = new UnityEngine.Vector3(-5, 0)
                                                                    }
                                                            };

    [UnityEngine.SerializeField]
    droid.Runtime.Structs.Space.Sample.SampleSpace4 _spawn_rotation_range =
        new droid.Runtime.Structs.Space.Sample.SampleSpace4 {
                                                                Space =
                                                                    new droid.Runtime.Structs.Space.Space4 {
                                                                        Max = UnityEngine.Vector4.one,
                                                                        Min = -UnityEngine.Vector4.one
                                                                    }
                                                            };

    [UnityEngine.SerializeField]
    System.Collections.Generic.List<UnityEngine.GameObject> _spawned_objects = null;

    [UnityEngine.SerializeField] bool _use_predefined_spawn_locations = false;

    void Start() { this._next_spawn_time = 0; }

    // Update is called once per frame
    void Update() { this.SpawnObjects(); }

    void SpawnObjects() {
      if (this._spawn_objects.Count > 0) {
        if (this._next_spawn_time < UnityEngine.Time.time) {
          if (this._spawned_objects.Count < this._max_num_concurrent_objects) {
            var object_selection = UnityEngine.Random.Range(0, maxExclusive : this._spawn_objects.Count);
            UnityEngine.Pose pose;
            if (this._use_predefined_spawn_locations) {
              var position_selection = UnityEngine.Random.Range(0, maxExclusive : this._spawn_poses.Count);
              var trans = this._spawn_poses[index : position_selection];
              pose = new UnityEngine.Pose(position : trans.position, rotation : trans.rotation);
            } else {
              pose = new UnityEngine.Pose(position : this._spawn_position_range.Sample(),
                                          rotation : this._spawn_rotation_range.Sample());
            }

            Instantiate(original : this._spawn_objects[index : object_selection],
                        position : pose.position,
                        rotation : pose.rotation);
            var delay = this._spawn_delay_range.Sample();
            this._next_spawn_time = UnityEngine.Time.time + delay;
          }
        }
      }
    }
  }
}