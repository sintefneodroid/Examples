namespace SceneAssets.Experiments.ScriptedManipulator.Scripts {
  public class GraspableObject : UnityEngine.MonoBehaviour,
                                 droid.Runtime.Utilities.Orientation.IMotionTracker {
    //[SerializeField] bool _draw_grasp = true;
    [UnityEngine.SerializeField] UnityEngine.Vector3 _last_recorded_move;
    [UnityEngine.SerializeField] UnityEngine.Quaternion _last_recorded_rotation;
    [UnityEngine.SerializeField] UnityEngine.Vector3 _previous_position;
    [UnityEngine.SerializeField] UnityEngine.Quaternion _previous_rotation;

    void Start() {
      this.UpdatePreviousTranform();
      this.UpdateLastRecordedTranform();
      //SetVisiblity(false);
    }

    void Update() { this.UpdatePreviousTranform(); }

    //[Space (height : 1)] [Header (header : "Scalars")]
    //[SerializeField]  int _floor_distance_scalar = 1;

    //[SerializeField]  int _gripper_distance_scalar = 1;

    public bool IsInMotion() {
      return this.transform.position != this._previous_position
             || this.transform.rotation != this._previous_rotation;
    }

    public bool IsInMotion(float sensitivity) {
      var distance_moved =
          UnityEngine.Vector3.Distance(a : this.transform.position, b : this._last_recorded_move);
      var angle_rotated =
          UnityEngine.Quaternion.Angle(a : this.transform.rotation, b : this._last_recorded_rotation);
      if (distance_moved > sensitivity || angle_rotated > sensitivity) {
        this.UpdateLastRecordedTranform();
        return true;
      }

      return false;
    }

    void UpdatePreviousTranform() {
      this._previous_position = this.transform.position;
      this._previous_rotation = this.transform.rotation;
    }

    void UpdateLastRecordedTranform() {
      this._last_recorded_move = this.transform.position;
      this._last_recorded_rotation = this.transform.rotation;
    }

    SceneAssets.Experiments.ScriptedManipulator.Scripts.Grasps.Grasp[] GetGrasps() {
      var grasps = this.gameObject
                       .GetComponentsInChildren<
                           SceneAssets.Experiments.ScriptedManipulator.Scripts.Grasps.Grasp>();
      return grasps;
    }

    void SetVisiblity(bool visible) {
      foreach (var grab in this.GetGrasps()) {
        grab.gameObject.SetActive(value : visible);
      }
    }

    void ChangeIndicatorColor(SceneAssets.Experiments.ScriptedManipulator.Scripts.Grasps.Grasp grasp,
                              UnityEngine.Color color) {
      foreach (var child in grasp.GetComponentsInChildren<UnityEngine.MeshRenderer>()) {
        child.material.SetColor("_Color1", value : color);
      }
    }

    //Main function
    //return grip vector/transform with highest score
    public System.Tuple<SceneAssets.Experiments.ScriptedManipulator.Scripts.Grasps.Grasp, float>
        GetOptimalGrasp(ScriptedGrasping grasping) {
      var grasps = this.GetGrasps();
      var unobstructed_grasps =
          new System.Collections.Generic.List<SceneAssets.Experiments.ScriptedManipulator.Scripts.Grasps.Grasp
          >();

      foreach (var grasp in grasps) {
        if (!grasp.IsObstructed()) {
          unobstructed_grasps.Add(item : grasp);
          this.ChangeIndicatorColor(grasp : grasp, color : UnityEngine.Color.yellow);
        } else {
          this.ChangeIndicatorColor(grasp : grasp, color : UnityEngine.Color.red);
        }
      }

      SceneAssets.Experiments.ScriptedManipulator.Scripts.Grasps.Grasp optimal_grasp = null;
      var shortest_distance = float.MaxValue;
      foreach (var grasp in unobstructed_grasps) {
        var distance =
            UnityEngine.Vector3.Distance(a : grasp.transform.position, b : grasping.transform.position);
        if (distance <= shortest_distance) {
          shortest_distance = distance;
          optimal_grasp = grasp;
        }
      }

      if (optimal_grasp != null) {
        this.ChangeIndicatorColor(grasp : optimal_grasp, color : UnityEngine.Color.green);
        return new
            System.Tuple<SceneAssets.Experiments.ScriptedManipulator.Scripts.Grasps.Grasp, float
            >(item1 : optimal_grasp, item2 : shortest_distance);
      }

      return null;
    }

    /*
  //Distance from floor - Score
  Dictionary<Transform,float> DistanceToFloorScore(Dictionary<Transform,float> grab_dict) {
    RaycastHit floorHit;
    float furthest_distance = -1;
    int index = 0;
    int furthest_index = 0;

    //Giving point to the grab vector furthest away from the floor
    foreach (var pair in grab_dict) {

      Vector3 point = pair.Key.position;

      if (Physics.Raycast(this.transform.position, (point - this.transform.position).normalized, Vector3.Distance(this.transform.position, point))) {
        grab_dict[pair.Key] -= 100;

      } else if (Physics.Raycast(point, Vector3.down, out floorHit, LayerMask.NameToLayer("Floor"))) {

        float current_distance = floorHit.distance;
        if (current_distance > furthest_distance) {
          furthest_distance = current_distance;
          grabs[index] += floor_dist_score;
          if (index != 0) {
            grabs[furthest_index] -= floor_dist_score;
          }
          furthest_index = index;
        }

      }
      index++;
    }
    return grabs;
  }

  //Distance from obstacles (range and if blocked) - Score
  Dictionary<Transform, float> DistanceToObstacleScore(Dictionary<Transform, float> score_list) {

    int index = 0;
    RaycastHit ray_hit;
    foreach (Transform grab_vector in GetGrabs()) {
      Vector3 point = grab_vector.Find("Point").position;

      if (Physics.Raycast(grab_vector.position, -grab_vector.forward, out ray_hit, proximity_radius)) {

        bool hand_col = (ray_hit.transform.tag == "Hand") ? true : false;

        if (Vector3.Distance(ray_hit.point, grab_vector.position) > 0.3f && !hand_col) {
          score_list[index] += obs_far;
          ChangeIndicatorColor(grab_vector, Color.yellow);

        } else if (Vector3.Distance(ray_hit.point, grab_vector.position) <= 0.3f && !hand_col) {
          score_list[index] += obs_close;
          ChangeIndicatorColor(grab_vector, Color.red);
        }

      } else if (Physics.Raycast(transform.position, (point - transform.position).normalized, Vector3.Distance(transform.position, point))) {

        score_list[index] += obs_very_close;
        ChangeIndicatorColor(grab_vector, Color.black);

      } else {
        score_list[index] += no_block;
        ChangeIndicatorColor(grab_vector, Color.white);
      }

      index++;
    }
    return score_list;
  }
  */
    //Distance from hand - Score
    System.Collections.Generic.Dictionary<UnityEngine.GameObject, float> DistanceToHandScore(
        UnityEngine.GameObject hand,
        System.Collections.Generic.Dictionary<UnityEngine.GameObject, float> grab_dict) {
      foreach (var pair in grab_dict) {
        var distance =
            UnityEngine.Vector3.Distance(a : pair.Key.transform.position, b : hand.transform.position);
        grab_dict[key : pair.Key] += distance;
      }

      return grab_dict;
    }

    /*
//Orientation (normal to floor) - Score
//----OBSOLETE----//
List<int> OrientationScore(List<int> score_list) {

 int index = 0;
 RaycastHit ray_hit;

 foreach (Transform grab_vector in GetGrabs()) {
   if (Physics.Raycast(grab_vector.position, grab_vector.right, out ray_hit, proximity_radius)) {
     if (ray_hit.transform.name == "Floor") {
       score_list[index] += good_ori;
     }
   } else if (Physics.Raycast(grab_vector.position, -grab_vector.right, out ray_hit, proximity_radius)) {
     if (ray_hit.transform.name == "Floor") {
       score_list[index] += good_ori;
     }
   } else {
     score_list[index] += bad_ori;
   }
   index++;
 }
 return score_list;
}*/
  }
}