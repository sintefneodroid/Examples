namespace SceneAssets.Experiments.ScriptedManipulator.Scripts.Navigation {
  [System.SerializableAttribute]
  class BezierCurvePath {
    public UnityEngine.Vector3 _Start_Position;
    public UnityEngine.Vector3 _Target_Position;
    BezierCurve _bezier_curve;
    System.Collections.Generic.List<UnityEngine.Vector3> _path_list;
    float _progress;

    public BezierCurvePath(UnityEngine.Vector3 start_position,
                           UnityEngine.Vector3 target_position,
                           BezierCurve game_object,
                           System.Collections.Generic.List<UnityEngine.Vector3> path_list) {
      this._Start_Position = start_position;
      this._Target_Position = target_position;
      this._path_list = path_list;
      this._bezier_curve = game_object;
      this.CurvifyPath();
    }

    void CurvifyPath() {
      for (var i = 0; i < this._bezier_curve.PointCount; i++) {
        UnityEngine.Object.Destroy(obj : this._bezier_curve[index : i].gameObject);
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
          var distance_previous = UnityEngine.Vector3.Distance(a : prev_point, b : curr_point);
          var distance_next = UnityEngine.Vector3.Distance(a : curr_point, b : next_point);

          bc[index : i].GlobalHandle1 += direction_back.normalized * distance_previous * handle_scalar;
          bc[index : i].GlobalHandle2 += direction_forward.normalized * distance_next * handle_scalar;

          //if (this.Debugging) Debug.DrawLine(bc[i].globalHandle1, bc[i].globalHandle2, Color.blue, 5);
        }
      }
    }

    public UnityEngine.Vector3 Next(float step_size) {
      this._progress += step_size;
      return this._bezier_curve.GetPointAt(t : this._progress);
    }
  }
}