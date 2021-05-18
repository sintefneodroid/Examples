namespace SceneAssets.Experiments.ScriptedManipulator.Scripts {
  //[ExecuteInEditMode]
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  public class ObstacleSpawner : UnityEngine.MonoBehaviour {
    [UnityEngine.SerializeField] UnityEngine.GameObject _cube = null;
    [UnityEngine.SerializeField] UnityEngine.Material _material_cube = null;

    [UnityEngine.SerializeField] UnityEngine.Material _material_sphere = null;

    /// <summary>
    /// </summary>
    [UnityEngine.RangeAttribute(1, 20)]
    [UnityEngine.SerializeField]
    int _number_of_cubes = 1;

    /// <summary>
    /// </summary>
    [UnityEngine.RangeAttribute(1, 20)]
    [UnityEngine.SerializeField]
    int _number_of_spheres = 1;

    [UnityEngine.SerializeField] UnityEngine.GameObject[] _obstacles = null;

    [UnityEngine.HeaderAttribute("Spawn random number of objects?")]
    [UnityEngine.SerializeField]
    bool _random_obj_num = false;

    [UnityEngine.SpaceAttribute]
    [UnityEngine.HeaderAttribute("Random scaling of objects (0 = uniform scale)")]
    [UnityEngine.RangeAttribute(0.000f, 0.300f)]
    [UnityEngine.SerializeField]
    float _scaling_factor = 0f;

    [UnityEngine.HeaderAttribute("Cube")]
    [UnityEngine.SerializeField]
    bool _spawn_cubes = true;

    /// <summary>
    /// </summary>
    [UnityEngine.HeaderAttribute("Sphere")]
    [UnityEngine.SerializeField]
    bool _spawn_spheres = true;

    [UnityEngine.SerializeField] UnityEngine.GameObject _sphere = null;

    /// <summary>
    /// </summary>
    [UnityEngine.SerializeField]
    float _sphere_size = 0.2f;

    /// <summary>
    /// </summary>
    [UnityEngine.HeaderAttribute("Show obstacle spawn box?")]
    [UnityEngine.SerializeField]
    bool _visualize_grid = true;

    /// <summary>
    /// </summary>
    [UnityEngine.SpaceAttribute]
    [UnityEngine.HeaderAttribute("Bounderies")]
    [UnityEngine.RangeAttribute(0.10f, 5.00f)]
    [UnityEngine.SerializeField]
    float _x_size = 1.4f;

    [UnityEngine.RangeAttribute(0.00f, 3.00f)]
    [UnityEngine.SerializeField]
    float _y_center_point = 1.4f;

    [UnityEngine.RangeAttribute(0.10f, 5.00f)]
    [UnityEngine.SerializeField]
    float _y_size = 1.2f;

    [UnityEngine.RangeAttribute(0.10f, 5.00f)]
    [UnityEngine.SerializeField]
    float _z_size = 1.4f;

    void Awake() { this.Setup(); }

    void Update() {
      if (this._visualize_grid) {
        droid.Runtime.Utilities.Grasping.GraspingUtilities.DrawRect(x_size : this._x_size,
                                                                    y_size : this._y_size,
                                                                    z_size : this._z_size,
                                                                    pos : this.transform.position,
                                                                    color : UnityEngine.Color.red);
      }
    }

    void OnDestroy() { this.TearDown(); }

    void TearDown() {
      if (this._cube) {
        DestroyImmediate(obj : this._cube);
      }

      if (this._sphere) {
        DestroyImmediate(obj : this._sphere);
      }

      if (this._obstacles != null && this._obstacles.Length > 0) {
        this.RemoveObstacles();
      }
    }

    /// <summary>
    /// </summary>
    public void Setup() {
      this.TearDown();
      this._y_center_point = this.transform.position.y;
      this._obstacles = new UnityEngine.GameObject[this._number_of_cubes + this._number_of_spheres];
      this._cube = UnityEngine.GameObject.CreatePrimitive(type : UnityEngine.PrimitiveType.Cube);
      this._cube.SetActive(false);
      //_cube.AddComponent<Obstruction>();
      this._cube.GetComponent<UnityEngine.MeshRenderer>().material = this._material_cube;
      this._sphere = UnityEngine.GameObject.CreatePrimitive(type : UnityEngine.PrimitiveType.Sphere);
      this._sphere.SetActive(false);
      //_sphere.AddComponent<Obstruction>();
      this._sphere.GetComponent<UnityEngine.MeshRenderer>().material = this._material_sphere;
      if (this._scaling_factor > 0.3f || this._scaling_factor < -0.3f) {
        this._scaling_factor = 0.3f;
      }

      if (!this._spawn_cubes && !this._spawn_spheres) {
        this._spawn_cubes = true;
      }

      if (this._random_obj_num) {
        this.SpawnObstacles(cube_num : UnityEngine.Random.Range(1, maxExclusive : this._number_of_cubes),
                            sphere_num : UnityEngine.Random.Range(1, maxExclusive : this._number_of_spheres));
      } else {
        this.SpawnObstacles(cube_num : this._number_of_cubes, sphere_num : this._number_of_spheres);
      }
    }

    public void SpawnObstacles(float cube_num = 1, float sphere_num = 1) {
      this.RemoveObstacles();
      var temp_list = new System.Collections.Generic.List<UnityEngine.GameObject>();
      if (this._spawn_cubes) {
        for (var i = 0; i < cube_num; i++) {
          var temp = UnityEngine.Random.Range(minInclusive : -this._scaling_factor, maxInclusive : this._scaling_factor);
          //spawn_pos = new Vector3(Random.Range(x_min, x_max), Random.Range(y_min, y_max), Random.Range(z_min, z_max));
          var spawn_pos =
              new UnityEngine.Vector3(x : UnityEngine.Random.Range(minInclusive : -this._x_size / 2,
                                                                   maxInclusive : this._x_size / 2),
                                      y : UnityEngine.Random.Range(minInclusive : -this._y_size / 2
                                                                                  + this._y_center_point,
                                                                   maxInclusive : this._y_size / 2
                                                                                  + this._y_center_point),
                                      z : UnityEngine.Random.Range(minInclusive : -this._z_size / 2,
                                                                   maxInclusive : this._z_size / 2));
          var cube_clone = Instantiate(original : this._cube,
                                       position : spawn_pos,
                                       rotation : UnityEngine.Quaternion.identity,
                                       parent : this.transform);
          cube_clone.transform.localScale = new UnityEngine.Vector3(x : this._sphere_size + temp,
                                                                    y : this._sphere_size + temp,
                                                                    z : this._sphere_size + temp);
          cube_clone.SetActive(true);
          cube_clone.tag = "Obstruction";

          temp_list.Add(item : cube_clone);
          /*if (Vector3.Distance(cube_clone.transform.position, GameObject.Find("EscapePos").transform.position) < 0.5f) {
                    Destroy(cube_clone);
                  }*/
        }
      }

      if (this._spawn_spheres) {
        for (var i = 0; i < sphere_num; i++) {
          var temp = UnityEngine.Random.Range(minInclusive : -this._scaling_factor, maxInclusive : this._scaling_factor);
          //spawn_pos = new Vector3(Random.Range(x_min, x_max), Random.Range(y_min, y_max), Random.Range(z_min, z_max));
          var spawn_pos =
              new UnityEngine.Vector3(x : UnityEngine.Random.Range(minInclusive : -this._x_size / 2,
                                                                   maxInclusive : this._x_size / 2),
                                      y : UnityEngine.Random.Range(minInclusive : -this._y_size / 2
                                                                                  + this._y_center_point,
                                                                   maxInclusive : this._y_size / 2
                                                                                  + this._y_center_point),
                                      z : UnityEngine.Random.Range(minInclusive : -this._z_size / 2,
                                                                   maxInclusive : this._z_size / 2));
          var sphere_clone = Instantiate(original : this._sphere,
                                         position : spawn_pos,
                                         rotation : UnityEngine.Quaternion.identity,
                                         parent : this.transform);
          sphere_clone.transform.localScale = new UnityEngine.Vector3(x : this._sphere_size + temp,
                                                                      y : this._sphere_size + temp,
                                                                      z : this._sphere_size + temp);
          sphere_clone.SetActive(true);
          sphere_clone.tag = "Obstruction";

          temp_list.Add(item : sphere_clone);
          /*if (Vector3.Distance(sphere_clone.transform.position, GameObject.Find("EscapePos").transform.position) < 0.2f) {
                    Destroy(sphere_clone);
                  }*/
        }
      }

      temp_list.CopyTo(array : this._obstacles);
    }

    void RemoveObstacles() {
      foreach (var obstacle in this._obstacles) {
        DestroyImmediate(obj : obstacle);
      }
    }
  }
}