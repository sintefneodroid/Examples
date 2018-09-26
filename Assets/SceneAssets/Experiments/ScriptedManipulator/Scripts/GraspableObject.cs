using System;
using System.Collections.Generic;
using Neodroid.Runtime.Utilities.Misc.Orientation;
using SceneAssets.Experiments.ScriptedManipulator.Grasps;
using UnityEngine;

namespace SceneAssets.Experiments.ScriptedManipulator.Scripts {
  public class GraspableObject : MonoBehaviour,
                                 IMotionTracker {
    //[SerializeField] bool _draw_grasp = true;
    [SerializeField] Vector3 _last_recorded_move;
    [SerializeField] Quaternion _last_recorded_rotation;
    [SerializeField] Vector3 _previous_position;
    [SerializeField] Quaternion _previous_rotation;

    //[Space (height : 1)] [Header (header : "Scalars")]
    //[SerializeField]  int _floor_distance_scalar = 1;

    //[SerializeField]  int _gripper_distance_scalar = 1;

    public bool IsInMotion() {
      return this.transform.position != this._previous_position
             || this.transform.rotation != this._previous_rotation;
    }

    public bool IsInMotion(float sensitivity) {
      var distance_moved = Vector3.Distance(this.transform.position, this._last_recorded_move);
      var angle_rotated = Quaternion.Angle(this.transform.rotation, this._last_recorded_rotation);
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

    void Start() {
      this.UpdatePreviousTranform();
      this.UpdateLastRecordedTranform();
      //SetVisiblity(false);
    }

    void Update() { this.UpdatePreviousTranform(); }

    Grasp[] GetGrasps() {
      var grasps = this.gameObject.GetComponentsInChildren<Grasp>();
      return grasps;
    }

    void SetVisiblity(bool visible) {
      foreach (var grab in this.GetGrasps()) {
        grab.gameObject.SetActive(visible);
      }
    }

    void ChangeIndicatorColor(Grasp grasp, Color color) {
      foreach (var child in grasp.GetComponentsInChildren<MeshRenderer>()) {
        child.material.SetColor("_Color1", color);
      }
    }

    //Main function
    //return grip vector/transform with highest score
    public Tuple<Grasp, float> GetOptimalGrasp(ScriptedGripper gripper) {
      var grasps = this.GetGrasps();
      var unobstructed_grasps = new List<Grasp>();

      foreach (var grasp in grasps) {
        if (!grasp.IsObstructed()) {
          unobstructed_grasps.Add(grasp);
          this.ChangeIndicatorColor(grasp, Color.yellow);
        } else {
          this.ChangeIndicatorColor(grasp, Color.red);
        }
      }

      Grasp optimal_grasp = null;
      var shortest_distance = float.MaxValue;
      foreach (var grasp in unobstructed_grasps) {
        var distance = Vector3.Distance(grasp.transform.position, gripper.transform.position);
        if (distance <= shortest_distance) {
          shortest_distance = distance;
          optimal_grasp = grasp;
        }
      }

      if (optimal_grasp != null) {
        this.ChangeIndicatorColor(optimal_grasp, Color.green);
        return new Tuple<Grasp, float>(optimal_grasp, shortest_distance);
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
    Dictionary<GameObject, float> DistanceToHandScore(
        GameObject hand,
        Dictionary<GameObject, float> grab_dict) {
      foreach (var pair in grab_dict) {
        var distance = Vector3.Distance(pair.Key.transform.position, hand.transform.position);
        grab_dict[pair.Key] += distance;
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