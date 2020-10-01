﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SceneAssets.Experiments.ScriptedManipulator.Scripts.Navigation {
  [Serializable]
  class BezierCurvePath {
    BezierCurve _bezier_curve;
    List<Vector3> _path_list;
    float _progress;

    public Vector3 _Start_Position;
    public Vector3 _Target_Position;

    public BezierCurvePath(Vector3 start_position,
                           Vector3 target_position,
                           BezierCurve game_object,
                           List<Vector3> path_list) {
      this._Start_Position = start_position;
      this._Target_Position = target_position;
      this._path_list = path_list;
      this._bezier_curve = game_object;
      this.CurvifyPath();
    }

    void CurvifyPath() {
      for (var i = 0; i < this._bezier_curve.PointCount; i++) {
        Object.Destroy(obj : this._bezier_curve[index : i].gameObject);
      }

      this._bezier_curve.ClearPoints();
      foreach (var t in this._path_list) {
        this._bezier_curve.AddPointAt(position : t);
      }

      this.SetHandlePosition(bc : this._bezier_curve);
    }

    void SetHandlePosition(BezierCurve bc) {
      for (var i = 0; i < bc.PointCount; i++) {
        bc[index : i]._Style = BezierPoint.HandleStyle.Broken_;

        if (i != 0 && i + 1 != bc.PointCount) {
          var curr_point = bc[index : i].Position;
          var prev_point = bc[index : i - 1].Position;
          var next_point = bc[index : i + 1].Position;
          var direction_forward = (next_point - prev_point).normalized;
          var direction_back = (prev_point - next_point).normalized;
          var handle_scalar = 0.33f;
          var distance_previous = Vector3.Distance(a : prev_point, b : curr_point);
          var distance_next = Vector3.Distance(a : curr_point, b : next_point);

          bc[index : i].GlobalHandle1 += direction_back.normalized * distance_previous * handle_scalar;
          bc[index : i].GlobalHandle2 += direction_forward.normalized * distance_next * handle_scalar;

          //if (this.Debugging) Debug.DrawLine(bc[i].globalHandle1, bc[i].globalHandle2, Color.blue, 5);
        }
      }
    }

    public Vector3 Next(float step_size) {
      this._progress += step_size;
      return this._bezier_curve.GetPointAt(t : this._progress);
    }
  }
}
