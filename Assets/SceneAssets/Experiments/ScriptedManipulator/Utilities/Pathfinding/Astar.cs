namespace SceneAssets.Experiments.ScriptedManipulator.Utilities.Pathfinding {
  /// <summary>
  /// </summary>
  public static class Astar {
    static float Heuristic(UnityEngine.Vector3 source, UnityEngine.Vector3 destination) {
      return UnityEngine.Vector3.Distance(a : source, b : destination);
    }

    static System.Collections.Generic.IEnumerable<FastVector3> GetUnobstructedNeighbouringNodes(
        UnityEngine.Vector3 current_point,
        float search_boundary,
        float grid_granularity,
        float sphere_cast_radius) {
      var c = current_point;
      var g = grid_granularity;

      var neighbours = new[] {
                                 new UnityEngine.Vector3(x : c.x + g, y : c.y, z : c.z),
                                 new UnityEngine.Vector3(x : c.x + g, y : c.y + g, z : c.z),
                                 new UnityEngine.Vector3(x : c.x + g, y : c.y - g, z : c.z),
                                 new UnityEngine.Vector3(x : c.x + g, y : c.y, z : c.z + g),
                                 new UnityEngine.Vector3(x : c.x + g, y : c.y, z : c.z - g),
                                 new UnityEngine.Vector3(x : c.x + g, y : c.y + g, z : c.z + g),
                                 new UnityEngine.Vector3(x : c.x + g, y : c.y + g, z : c.z - g),
                                 new UnityEngine.Vector3(x : c.x + g, y : c.y - g, z : c.z + g),
                                 new UnityEngine.Vector3(x : c.x + g, y : c.y - g, z : c.z - g),
                                 new UnityEngine.Vector3(x : c.x - g, y : c.y, z : c.z),
                                 new UnityEngine.Vector3(x : c.x - g, y : c.y + g, z : c.z),
                                 new UnityEngine.Vector3(x : c.x - g, y : c.y - g, z : c.z),
                                 new UnityEngine.Vector3(x : c.x - g, y : c.y, z : c.z + g),
                                 new UnityEngine.Vector3(x : c.x - g, y : c.y, z : c.z - g),
                                 new UnityEngine.Vector3(x : c.x - g, y : c.y + g, z : c.z + g),
                                 new UnityEngine.Vector3(x : c.x - g, y : c.y + g, z : c.z - g),
                                 new UnityEngine.Vector3(x : c.x - g, y : c.y - g, z : c.z + g),
                                 new UnityEngine.Vector3(x : c.x - g, y : c.y - g, z : c.z - g),
                                 new UnityEngine.Vector3(x : c.x, y : c.y, z : c.z + g),
                                 new UnityEngine.Vector3(x : c.x, y : c.y, z : c.z - g),
                                 new UnityEngine.Vector3(x : c.x, y : c.y + g, z : c.z),
                                 new UnityEngine.Vector3(x : c.x, y : c.y - g, z : c.z),
                                 new UnityEngine.Vector3(x : c.x, y : c.y + g, z : c.z + g),
                                 new UnityEngine.Vector3(x : c.x, y : c.y + g, z : c.z - g),
                                 new UnityEngine.Vector3(x : c.x, y : c.y - g, z : c.z + g),
                                 new UnityEngine.Vector3(x : c.x, y : c.y - g, z : c.z - g)
                             };

      var return_set = new System.Collections.Generic.List<FastVector3>();

      foreach (var neighbour in neighbours) {
        if (IsObstructed(point : neighbour,
                         current : c,
                         search_boundary : search_boundary,
                         sphere_cast_radius : sphere_cast_radius)) {
          continue; // do not add obstructed points to returned set
        }

        return_set.Add(item : new FastVector3(v : neighbour));
      }

      return return_set;
    }

    static bool IsObstructed(UnityEngine.Vector3 point,
                             UnityEngine.Vector3 current,
                             float search_boundary,
                             float sphere_cast_radius,
                             string obstruction_tag = "Obstruction") {
      if (point.x <= -search_boundary
          || point.x >= search_boundary
          || point.y <= -search_boundary
          || point.y >= search_boundary
          || point.z <= -search_boundary
          || point.z >= search_boundary) {
        return true;
      }

      var ray = new UnityEngine.Ray(origin : current, direction : (point - current).normalized);
      return UnityEngine.Physics.SphereCast(ray : ray,
                                            radius : sphere_cast_radius,
                                            maxDistance :
                                            UnityEngine.Vector3.Distance(a : current, b : point),
                                            layerMask :
                                            UnityEngine.LayerMask.NameToLayer(layerName : obstruction_tag));
    }

    /// <summary>
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <param name="search_boundary"></param>
    /// <param name="grid_granularity"></param>
    /// <param name="agent_size"></param>
    /// <param name="near_stopping_distance"></param>
    /// <returns></returns>
    public static System.Collections.Generic.List<UnityEngine.Vector3> FindPathAstar(
        UnityEngine.Vector3 source,
        UnityEngine.Vector3 destination,
        float search_boundary = 20f,
        float grid_granularity = 1f,
        float agent_size = 1f,
        float near_stopping_distance = 3f) {
      var closed_set = new System.Collections.Generic.HashSet<FastVector3>();
      //FastPriorityQueue<FastVector3> frontier_set = new FastPriorityQueue<FastVector3>(MAX_VECTORS_IN_QUEUE);
      var frontier_set = new Priority_Queue.SimplePriorityQueue<FastVector3>();

      var fast_source = new FastVector3(v : source);
      var predecessor = new System.Collections.Generic.Dictionary<FastVector3, FastVector3>();
      var g_scores = new System.Collections.Generic.Dictionary<FastVector3, float> {{fast_source, 0}};

      frontier_set.Enqueue(item : fast_source,
                           priority : Heuristic(source : source,
                                                destination :
                                                destination)); // Priority is distance, lowest distance highest priority
      while (frontier_set.Count > 0) {
        var current_point = frontier_set.Dequeue();
        closed_set.Add(item : current_point);

        //Stopping condition and reconstruct path
        var distance_to_destination = Heuristic(source : current_point.V, destination : destination);
        if (distance_to_destination < near_stopping_distance) {
          var current_trace_back_point = current_point;
          var path = new System.Collections.Generic.List<UnityEngine.Vector3> {current_trace_back_point.V};
          while (predecessor.ContainsKey(key : current_trace_back_point)) {
            current_trace_back_point = predecessor[key : current_trace_back_point];
            path.Add(item : current_trace_back_point.V);
          }

          path.Reverse();
          return path;
        }

        //Get neighboring points
        var neighbours = GetUnobstructedNeighbouringNodes(current_point : current_point.V,
                                                          search_boundary : search_boundary,
                                                          grid_granularity : grid_granularity,
                                                          sphere_cast_radius : agent_size);

        //Calculate scores and add to frontier
        foreach (var neighbour in neighbours) {
          /*foreach(FastVector3 candidate_point in frontier_set) {  For FastPriorityQueue implementation this is check necessary
          if(neighbour == candidate_point) {
            neighbour.QueueIndex = candidate_point.QueueIndex;
          }
        }*/

          if (closed_set.Contains(item : neighbour)) {
            continue; // Skip if neighbour is already in closed set'
          }

          var temp_g_score = g_scores[key : current_point]
                             + Heuristic(source : current_point.V, destination : neighbour.V);

          if (frontier_set.Contains(item : neighbour)) {
            if (temp_g_score > g_scores[key : neighbour]) {
              continue; // Skip if neighbour g_score is already lower
            }

            g_scores[key : neighbour] = temp_g_score;
          } else {
            g_scores.Add(key : neighbour, value : temp_g_score);
          }

          var f_score = g_scores[key : neighbour]
                        + Heuristic(source : neighbour.V, destination : destination);

          if (frontier_set.Contains(item : neighbour)) {
            frontier_set.UpdatePriority(item : neighbour, priority : f_score);
            predecessor[key : neighbour] = current_point;
          } else {
            /*if (frontier_set.Count > MAX_VECTORS_IN_QUEUE-1) { For FastPriorityQueue implementation this is check necessary
                        return null;
                    }*/
            frontier_set.Enqueue(item : neighbour, priority : f_score);
            predecessor.Add(key : neighbour, value : current_point);
          }
        }
      }

      return null;
    }

    /// <summary>
    /// </summary>
    /// <param name="path"></param>
    /// <param name="sphere_cast_radius"></param>
    /// <returns></returns>
    public static System.Collections.Generic.List<UnityEngine.Vector3> SimplifyPath(
        System.Collections.Generic.List<UnityEngine.Vector3> path,
        float sphere_cast_radius = 1f) {
      var smooth_path = new System.Collections.Generic.List<UnityEngine.Vector3> {path[0]};
      path.RemoveAt(0);
      path.Reverse(); // reverse to walk from last point

      while (path.Count > 0) {
        var last_point =
            smooth_path[index : smooth_path.Count - 1]; // will be drawing from last point in smoothed path
        var new_point =
            path[index : path.Count - 1]; // next unsmoothed path point is the last in reversed array
        foreach (var point in path) {
          var ray = new UnityEngine.Ray(origin : last_point, direction : (point - last_point).normalized);
          if (UnityEngine.Physics.SphereCast(ray : ray,
                                             radius : sphere_cast_radius,
                                             maxDistance : UnityEngine.Vector3.Distance(a : point,
                                               b : last_point))) {
            continue;
          }

          new_point = point;
          break;
        }

        // new_point can still be unchanged here, so next point is the same as in unsmoothed path

        smooth_path.Add(item : new_point);
        var index_of_new_point = path.IndexOf(item : new_point);
        path.RemoveRange(index : index_of_new_point,
                         count : path.Count
                                 - index_of_new_point); // kill everything after (including) found point
      }

      return smooth_path;
    }

    #region Nested type: FastVector3

    //private const int MAX_VECTORS_IN_QUEUE = 1000000; For FastPriorityQueue implementation this is value necessary

    /// <summary>
    /// </summary>
    public class FastVector3 : Priority_Queue.FastPriorityQueueNode {
      public FastVector3(UnityEngine.Vector3 v) { this.V = v; }

      /// <summary>
      /// </summary>
      public UnityEngine.Vector3 V { get; }

      //public int QueueIndex { get; set; }

      /// <summary>
      /// </summary>
      /// <returns></returns>
      public override int GetHashCode() { return this.V.GetHashCode(); }

      /// <summary>
      /// </summary>
      /// <param name="obj"></param>
      /// <returns></returns>
      public override bool Equals(object obj) {
        if (obj == null) {
          return false;
        }

        // If parameter cannot be cast to Point return false.
        var p = obj as FastVector3;
        return this.Equals(obj : p);
      }

      /// <summary>
      /// </summary>
      /// <param name="obj"></param>
      /// <returns></returns>
      public bool Equals(FastVector3 obj) { return this.V == obj.V; }
    }

    #endregion
  }
}