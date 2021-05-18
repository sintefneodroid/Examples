namespace SceneAssets.Experiments.ScriptedManipulator.Scripts {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  public class ScriptedGrasping : UnityEngine.MonoBehaviour {
    public GraspableObject TargetGameObject {
      get { return this._target_game_object; }
      set {
        this._target_game_object = value;

        this.StopCoroutine("gripper_movement");
        this.StartCoroutine("gripper_movement",
                            value : this.FollowPathToApproach1(trans : this._target_game_object.transform));
      }
    }

    /// <summary>
    /// </summary>
    public SceneAssets.Experiments.ScriptedManipulator.Utilities.States State {
      get { return this._state; }
      set { this._state = value; }
    }

    System.Collections.IEnumerator FollowPathToApproach1(UnityEngine.Transform trans) {
      while (true) {
        if (UnityEngine.Vector3.Distance(a : this.transform.position, b : this._intermediate_target)
            <= this._precision) {
          this._intermediate_target = this._path.Next(1);
        }

        if (this._debugging) {
          UnityEngine.Debug.DrawRay(start : this._intermediate_target,
                                    dir : this.transform.forward,
                                    color : UnityEngine.Color.green);
        }

        this.transform.position = UnityEngine.Vector3.MoveTowards(current : this.transform.position,
                                                                  target : this._intermediate_target,
                                                                  maxDistanceDelta : 1);
      }
    }

    /// <summary>
    /// </summary>
    /// <param name="state"></param>
    public void
        respawn_obstructions(SceneAssets.Experiments.ScriptedManipulator.Utilities.GripperState state) {
      var obstacles_spawner = FindObjectOfType<ObstacleSpawner>();
      //obstacles_spawner.SpawnObstacles(obstacles_spawner.number_of_cubes, obstacles_spawner.number_of_spheres);
    }

    #region Helpers

    void PerformReactionToCurrentState(SceneAssets.Experiments.ScriptedManipulator.Utilities.States state) {
      switch (state.PathFindingState) {
        case SceneAssets.Experiments.ScriptedManipulator.Utilities.PathFindingState.Waiting_for_target_:
          this.FindTargetAndUpdatePath();
          if (this._target_grasp != null) {
            state.NavigateToTarget();
          }

          break;

        case SceneAssets.Experiments.ScriptedManipulator.Utilities.PathFindingState.Navigating_:
          if (state.IsEnvironmentMoving() && this._wait_for_resting_environment) {
            state.WaitForRestingEnvironment();
            break;
          }

          if (state.WasEnvironmentMoving()) {
            this.FindTargetAndUpdatePath();
          }

          this.FollowPathToApproach(step_size : this._step_size,
                                    rotation : this._target_grasp.transform.rotation);
          state.OpenClaw();
          this.CheckIfGripperIsOpen();
          this.MaybeBeginApproachProcedure();
          break;

        case SceneAssets.Experiments.ScriptedManipulator.Utilities.PathFindingState.Approaching_:
          this.ApproachTarget(step_size : this._step_size);
          break;

        case SceneAssets.Experiments.ScriptedManipulator.Utilities.PathFindingState.Picking_up_target_:
          if (state.IsGripperClosed() && state.IsTargetNotGrabbed()) {
            //state.ReturnToStartPosition();
          }

          break;

        case SceneAssets.Experiments.ScriptedManipulator.Utilities.PathFindingState
                        .Waiting_for_resting_environment_:
          if (state.WasEnvironmentMoving()) {
            this.FindTargetAndUpdatePath();
          }

          if (state.IsEnvironmentAtRest()) {
            this.FindTargetAndUpdatePath();
            if (state.IsTargetGrabbed()) {
              state.ReturnToStartPosition();
            } else {
              state.NavigateToTarget();
            }
          }

          break;

        case SceneAssets.Experiments.ScriptedManipulator.Utilities.PathFindingState.Returning_:
          if (state.WereObstructionMoving()) {
            this._path = this.FindPath(start_position : this.transform.position,
                                       target_position : this._reset_position);
          }

          if (this._wait_for_resting_environment) {
            if (state.IsObstructionsAtRest()) {
              this.FollowPathToApproach(step_size : this._step_size,
                                        rotation : UnityEngine.Quaternion.Euler(90, 0, 0));
              this.MaybeBeginReleaseProcedure();
            }
          } else {
            this.FollowPathToApproach(step_size : this._step_size,
                                      rotation : UnityEngine.Quaternion.Euler(90, 0, 0));
            this.MaybeBeginReleaseProcedure();
          }

          break;
        default:
          throw new System.ArgumentOutOfRangeException();
      }

      switch (state.GripperState) {
        case SceneAssets.Experiments.ScriptedManipulator.Utilities.GripperState.Opening_:
          this.OpenClaws(step_size : this._step_size);
          break;

        case SceneAssets.Experiments.ScriptedManipulator.Utilities.GripperState.Closing_:
          this.CloseClaws(step_size : this._step_size);
          break;

        case SceneAssets.Experiments.ScriptedManipulator.Utilities.GripperState.Closed_:
          break;
      }

      if (!state.IsTargetInsideRegionOrTouching()) {
        state.TargetIsNotGrabbed();
        if (this._target_game_object != null) {
          this._target_game_object.transform.parent = null;
        }
      }

      if (state.IsTargetTouchingAndInsideRegion()) {
        if (this._target_game_object != null) {
          this._target_game_object.transform.parent = this.transform;
        }

        state.TargetIsGrabbed();
        this._path = this.FindPath(start_position : this.transform.position,
                                   target_position : this._reset_position);
        this._intermediate_target = this._path.Next(0.001f);
      }

      this.MaybeClawIsClosed();
    }

    #endregion

    #region PrivateMembers

    GraspableObject _target_game_object;
    UnityEngine.Vector3 _approach_position;
    SceneAssets.Experiments.ScriptedManipulator.Scripts.Grasps.Grasp _target_grasp;
    SceneAssets.Experiments.ScriptedManipulator.Scripts.Navigation.BezierCurvePath _path;

    UnityEngine.Vector3 _default_motor_position;
    UnityEngine.Vector3 _closed_motor_position;

    UnityEngine.Vector3 _intermediate_target;
    //GameObject[] _targets;

    UnityEngine.Vector3 _reset_position;
    SceneAssets.Experiments.ScriptedManipulator.Scripts.Navigation.BezierCurve _bezier_curve;

    float _step_size;

    #endregion

    #region PublicMembers

    [UnityEngine.SpaceAttribute(1)]
    [UnityEngine.HeaderAttribute("Game Objects")]
    [UnityEngine.TooltipAttribute("Game Object references")]
    [UnityEngine.SerializeField]
    UnityEngine.GameObject _claw_motor = null;

    [UnityEngine.SerializeField] UnityEngine.GameObject _grab_region = null;
    [UnityEngine.SerializeField] UnityEngine.GameObject _begin_grab_region = null;

    [UnityEngine.SerializeField] UnityEngine.GameObject _claw_1 = null;
    [UnityEngine.SerializeField] UnityEngine.GameObject _claw_2 = null;

    [UnityEngine.SerializeField] SceneAssets.Experiments.ScriptedManipulator.Utilities.States _state = null;
    [UnityEngine.SerializeField] UnityEngine.Transform _closed_motor_transform = null;
    [UnityEngine.SerializeField] ObstacleSpawner _obstacle_spawner = null;

    [UnityEngine.SerializeField]
    SceneAssets.Experiments.ScriptedManipulator.Scripts.Navigation.BezierCurve _bezier_curve_prefab = null;

    [UnityEngine.SpaceAttribute(1)]
    [UnityEngine.HeaderAttribute("Path Finding Parameters")]
    [UnityEngine.SerializeField]
    float _search_boundary = 6f;

    [UnityEngine.SerializeField] float _actor_size = 0.3f;
    [UnityEngine.SerializeField] float _grid_granularity = 0.4f;
    [UnityEngine.SerializeField] float _speed = 0.5f;
    [UnityEngine.SerializeField] float _precision = 0.02f;
    [UnityEngine.SerializeField] float _sensitivity = 0.2f;
    [UnityEngine.SerializeField] float _approach_distance = 0.6f;
    [UnityEngine.SerializeField] bool _wait_for_resting_environment = false;

    [UnityEngine.SpaceAttribute(1)]
    [UnityEngine.HeaderAttribute("Show debug logs")]
    [UnityEngine.SerializeField]
    bool _debugging = false;

    [UnityEngine.SpaceAttribute(1)]
    [UnityEngine.HeaderAttribute("Draw Search Boundary")]
    [UnityEngine.SerializeField]
    bool _draw_search_boundary = true;

    #endregion

    #region Setup

    void UpdateMeshFilterBounds() {
      var actor_bounds = droid.Runtime.Utilities.Grasping.GraspingUtilities.GetMaxBounds(g : this.gameObject);
      //_environment_size = agent_bounds.size.magnitude;
      //_approach_distance = agent_bounds.size.magnitude + _precision;
      this._actor_size =
          actor_bounds.extents.magnitude
          * 2; //Mathf.Max(agent_bounds.extents.x, Mathf.Max(agent_bounds.extents.y, agent_bounds.extents.z)) * 2;
      this._approach_distance = this._actor_size + this._precision;
    }

    void SetupEnvironment() {
      droid.Runtime.Utilities.NeodroidRegistrationUtilities
           .RegisterCollisionTriggerCallbacksOnChildren<
               droid.Runtime.GameObjects.ChildSensors.ChildCollider3DSensor, UnityEngine.Collider,
               UnityEngine.Collision>(caller : this,
                                      parent : this.transform,
                                      on_collision_enter_child : this.OnCollisionEnterChild,
                                      on_trigger_enter_child : this.OnTriggerEnterChild,
                                      on_collision_exit_child : this.OnCollisionExitChild,
                                      on_trigger_exit_child : this.OnTriggerExitChild,
                                      on_collision_stay_child : this.OnCollisionStayChild,
                                      on_trigger_stay_child : this.OnTriggerStayChild,
                                      debug : this._debugging);
    }

    #endregion

    #region UnityCallbacks

    void Start() {
      this._state = new SceneAssets.Experiments.ScriptedManipulator.Utilities.States();
      this._reset_position = this.transform.position;
      this._default_motor_position = this._claw_motor.transform.localPosition;
      this._closed_motor_position = this._closed_motor_transform.localPosition;
      this._bezier_curve =
          FindObjectOfType<SceneAssets.Experiments.ScriptedManipulator.Scripts.Navigation.BezierCurve>();
      if (!this._bezier_curve) {
        this._bezier_curve = Instantiate(original : this._bezier_curve_prefab);
      }

      this.UpdateMeshFilterBounds();

      this.FindTargetAndUpdatePath();
      this._state.ResetStates();
      this.SetupEnvironment();
    }

    void Update() {
      this._step_size = this._speed * UnityEngine.Time.deltaTime;
      if (this._draw_search_boundary) {
        droid.Runtime.Utilities.Grasping.GraspingUtilities.DrawBoxFromCenter(p : this.transform.position,
          r : this._search_boundary,
          c : UnityEngine.Color.magenta);
      }

      this._state.ObstructionMotionState =
          this._state.GetMotionState(objects : FindObjectsOfType<droid.Runtime.Utilities.Extensions.Obstruction>(),
                                     previous_state : this._state.ObstructionMotionState,
                                     sensitivity : this._sensitivity);
      this._state.TargetMotionState =
          this._state.GetMotionState(objects : FindObjectsOfType<GraspableObject>(),
                                     previous_state : this._state.TargetMotionState,
                                     sensitivity : this._sensitivity);

      this.PerformReactionToCurrentState(state : this._state);
    }

    #endregion

    #region StateTransitions

    void MaybeBeginReleaseProcedure() {
      if (!(UnityEngine.Vector3.Distance(a : this.transform.position, b : this._reset_position)
            < this._precision)) {
        return;
      }

      if (this._target_game_object) {
        this._target_game_object.transform.parent = null;
      }

      this._state.OpenClaw();
      this._state.WaitForTarget();

      if (this._obstacle_spawner != null) {
        this._obstacle_spawner.Setup();
      }
    }

    void MaybeClawIsClosed() {
      if (!(UnityEngine.Vector3.Distance(a : this._claw_motor.transform.localPosition,
                                         b : this._closed_motor_position)
            < this._precision)) {
        return;
      }

      this._state.GripperIsClosed();
      this._path = this.FindPath(start_position : this.transform.position,
                                 target_position : this._reset_position);
      this._intermediate_target = this._path.Next(0.001f);
      this._state.ReturnToStartPosition();
    }

    void MaybeBeginApproachProcedure() {
      if (UnityEngine.Vector3.Distance(a : this.transform.position, b : this._path._Target_Position)
          < this._approach_distance + this._precision
          && UnityEngine.Quaternion.Angle(a : this.transform.rotation,
                                          b : this._target_grasp.transform.rotation)
          < this._precision
          && this._state.IsGripperOpen()) {
        this._state.ApproachTarget();
      }
    }

    void CheckIfGripperIsOpen() {
      if (UnityEngine.Vector3.Distance(a : this._claw_motor.transform.localPosition,
                                       b : this._default_motor_position)
          < this._precision) {
        this._state.GripperIsOpen();
      }
    }

    void CloseClaws(float step_size) {
      //Vector3 current_motor_position = _motor.transform.localPosition;
      //current_motor_position.y += step_size / 16;
      //_motor.transform.localPosition = current_motor_position;
      this._claw_motor.transform.localPosition =
          UnityEngine.Vector3.MoveTowards(current : this._claw_motor.transform.localPosition,
                                          target : this._closed_motor_position,
                                          maxDistanceDelta : step_size / 8);
    }

    void OpenClaws(float step_size) {
      this._claw_motor.transform.localPosition =
          UnityEngine.Vector3.MoveTowards(current : this._claw_motor.transform.localPosition,
                                          target : this._default_motor_position,
                                          maxDistanceDelta : step_size / 8);
      //StopCoroutine ("claw_movement");
      //StartCoroutine ("claw_movement", OpenClaws1 ());
    }

    System.Collections.IEnumerator OpenClaws1() {
      while (!this._state.IsTargetGrabbed()) {
        this._claw_motor.transform.localPosition =
            UnityEngine.Vector3.MoveTowards(current : this._claw_motor.transform.localPosition,
                                            target : this._default_motor_position,
                                            maxDistanceDelta : UnityEngine.Time.deltaTime);
        yield return null; // new WaitForSeconds(waitTime);
      }
    }

    System.Collections.IEnumerator CloseClaws1() {
      while (!this._state.IsTargetGrabbed()) {
        this._claw_motor.transform.localPosition =
            UnityEngine.Vector3.MoveTowards(current : this._claw_motor.transform.localPosition,
                                            target : this._closed_motor_position,
                                            maxDistanceDelta : UnityEngine.Time.deltaTime);
        yield return null; // new WaitForSeconds(waitTime);
      }
    }

    #endregion

    #region Collisions

    void Ab(UnityEngine.GameObject child_game_object, GraspableObject other_maybe_graspable) {
      if (other_maybe_graspable) {
        if (child_game_object == this._grab_region.gameObject
            && other_maybe_graspable.gameObject == this._target_game_object.gameObject) {
          if (this._debugging) {
            UnityEngine.Debug.Log(message : $"Target {other_maybe_graspable.name} is inside region");
          }

          this._state.TargetIsInsideRegion();
        }

        if (child_game_object == this._begin_grab_region.gameObject
            && other_maybe_graspable.gameObject == this._target_game_object.gameObject
            && !this._state.IsTargetGrabbed()) {
          if (this._debugging) {
            UnityEngine.Debug.Log(message : $"Picking up target {other_maybe_graspable.name}");
          }

          this._state.PickUpTarget();
        }
      }
    }

    void OnTriggerEnterChild(UnityEngine.GameObject child_game_object,
                             UnityEngine.Collider other_game_object) {
      var other_maybe_graspable = other_game_object.GetComponentInParent<GraspableObject>();
      this.Ab(child_game_object : child_game_object, other_maybe_graspable : other_maybe_graspable);
    }

    void OnTriggerStayChild(UnityEngine.GameObject child_game_object,
                            UnityEngine.Collider other_game_object) {
      var other_maybe_graspable = other_game_object.GetComponentInParent<GraspableObject>();
      this.Ab(child_game_object : child_game_object, other_maybe_graspable : other_maybe_graspable);
    }

    void OnCollisionStayChild(UnityEngine.GameObject child_game_object, UnityEngine.Collision collision) {
      var other_maybe_graspable = collision.gameObject.GetComponentInParent<GraspableObject>();
      if (other_maybe_graspable) {
        if (child_game_object == this._claw_1.gameObject
            && other_maybe_graspable.gameObject == this._target_game_object.gameObject) {
          if (this._debugging) {
            UnityEngine.Debug.Log(message : $"Target {other_maybe_graspable.name} is touching {child_game_object.name}");
          }

          this._state.Claw1IsTouchingTarget();
        }

        if (child_game_object == this._claw_2.gameObject
            && other_maybe_graspable.gameObject == this._target_game_object.gameObject) {
          if (this._debugging) {
            UnityEngine.Debug.Log(message : $"Target {other_maybe_graspable.name} is touching {child_game_object.name}");
          }

          this._state.Claw2IsTouchingTarget();
        }
      }
    }

    void OnCollisionExitChild(UnityEngine.GameObject child_game_object, UnityEngine.Collision collision) {
      /*if (collision.gameObject.GetComponent<Obstruction> () != null) {
        _state.GripperState = GripperState.Opening;
      }*/

      var other_maybe_graspable = collision.gameObject.GetComponentInParent<GraspableObject>();
      if (other_maybe_graspable) {
        if (child_game_object == this._claw_1.gameObject
            && other_maybe_graspable.gameObject == this._target_game_object.gameObject) {
          this._state.Claw1IsNotTouchingTarget();
        }

        if (child_game_object == this._claw_2.gameObject
            && other_maybe_graspable.gameObject == this._target_game_object.gameObject) {
          this._state.Claw2IsNotTouchingTarget();
        }
      }
    }

    void OnTriggerExitChild(UnityEngine.GameObject child_game_object,
                            UnityEngine.Collider other_game_object) {
      /*if (other_game_object.gameObject.GetComponent<Obstruction> () != null) {
        _state.GripperState = GripperState.Opening;
      }*/

      var other_maybe_graspable = other_game_object.GetComponentInParent<GraspableObject>();
      if (other_maybe_graspable) {
        if (child_game_object == this._grab_region.gameObject
            && other_maybe_graspable.gameObject == this._target_game_object.gameObject) {
          this._state.TargetIsOutsideRegion();
        }
      }
    }

    void OnCollisionEnterChild(UnityEngine.GameObject child_game_object, UnityEngine.Collision collision) {
      /*if (collision.gameObject.GetComponent<Obstruction> () != null) {
        _state.GripperState = GripperState.Closing;
      }*/

      var other_maybe_graspable = collision.gameObject.GetComponentInParent<GraspableObject>();
      if (other_maybe_graspable) {
        if (child_game_object == this._claw_1.gameObject
            && other_maybe_graspable.gameObject == this._target_game_object.gameObject) {
          this._state.Claw1IsTouchingTarget();
        }

        if (child_game_object == this._claw_2.gameObject
            && other_maybe_graspable.gameObject == this._target_game_object.gameObject) {
          this._state.Claw2IsTouchingTarget();
        }
      }
    }

    #endregion

    #region PathFinding

    System.Tuple<GraspableObject, SceneAssets.Experiments.ScriptedManipulator.Scripts.Grasps.Grasp>
        GetOptimalTargetAndGrasp() {
      var targets = FindObjectsOfType<GraspableObject>();
      if (targets.Length == 0) {
        return null;
      }

      var shortest_distance = float.MaxValue;
      GraspableObject optimal_target = null;
      SceneAssets.Experiments.ScriptedManipulator.Scripts.Grasps.Grasp optimal_grasp = null;
      foreach (var target in targets) {
        var pair = target.GetOptimalGrasp(grasping : this);
        if (pair == null || pair.Item1 == null || pair.Item1.IsObstructed()) {
          continue;
        }

        var target_grasp = pair.Item1;
        var distance = pair.Item2;
        if (distance < shortest_distance) {
          shortest_distance = distance;
          optimal_grasp = target_grasp;
          optimal_target = target;
        }
      }

      return new System.Tuple<GraspableObject,
          SceneAssets.Experiments.ScriptedManipulator.Scripts.Grasps.Grasp>(item1 : optimal_target,
        item2 : optimal_grasp);
    }

    public void FindTargetAndUpdatePath() {
      var pair = this.GetOptimalTargetAndGrasp();
      if (pair == null || pair.Item1 == null || pair.Item2 == null) {
        this._state.PathFindingState =
            SceneAssets.Experiments.ScriptedManipulator.Utilities.PathFindingState.Returning_;
        this._path = this.FindPath(start_position : this.transform.position,
                                   target_position : this._reset_position);
        return;
      }

      this._target_game_object = pair.Item1;
      this._target_grasp = pair.Item2;
      var transform1 = this._target_grasp.transform;
      this._approach_position = transform1.position - transform1.forward * this._approach_distance;
      if (UnityEngine.Vector3.Distance(a : this.transform.position, b : this._approach_position)
          > this._search_boundary) {
        return;
      }

      this._path = this.FindPath(start_position : this.transform.position,
                                 target_position : this._approach_position);
      this._intermediate_target = this._path.Next(step_size : this._step_size);
    }

    SceneAssets.Experiments.ScriptedManipulator.Scripts.Navigation.BezierCurvePath FindPath(
        UnityEngine.Vector3 start_position,
        UnityEngine.Vector3 target_position) {
      this.UpdateMeshFilterBounds();
      var path_list =
          SceneAssets.Experiments.ScriptedManipulator.Utilities.Pathfinding.Astar.FindPathAstar(source : start_position,
            destination : target_position,
            search_boundary : this._search_boundary,
            grid_granularity : this._grid_granularity,
            agent_size : this._actor_size,
            near_stopping_distance : this._approach_distance);
      if (path_list != null && path_list.Count > 0) {
        path_list =
            SceneAssets.Experiments.ScriptedManipulator.Utilities.Pathfinding.Astar
                       .SimplifyPath(path : path_list, sphere_cast_radius : this._actor_size);
        path_list.Add(item : target_position);
      } else {
        path_list =
            new System.Collections.Generic.List<UnityEngine.Vector3> {start_position, target_position};
      }

      var path =
          new SceneAssets.Experiments.ScriptedManipulator.Scripts.Navigation.BezierCurvePath(start_position : start_position,
            target_position : target_position,
            game_object : this._bezier_curve,
            path_list : path_list);
      return path;
    }

    void ApproachTarget(float step_size) {
      this.transform.position = UnityEngine.Vector3.MoveTowards(current : this.transform.position,
                                                                target : this._target_grasp.transform
                                                                             .position,
                                                                maxDistanceDelta : step_size);
      if (this._debugging) {
        UnityEngine.Debug.DrawLine(start : this.transform.position,
                                   end : this._target_grasp.transform.position,
                                   color : UnityEngine.Color.green);
      }
    }

    void FollowPathToApproach(float step_size, UnityEngine.Quaternion rotation, bool rotate = true) {
      if (UnityEngine.Vector3.Distance(a : this.transform.position, b : this._intermediate_target)
          <= this._precision) {
        this._intermediate_target = this._path.Next(step_size : step_size);
      }

      if (this._debugging) {
        UnityEngine.Debug.DrawRay(start : this._intermediate_target,
                                  dir : this.transform.forward,
                                  color : UnityEngine.Color.magenta);
      }

      if (rotate) {
        this.transform.rotation =
            UnityEngine.Quaternion.RotateTowards(@from : this.transform.rotation,
                                                 to : rotation,
                                                 maxDegreesDelta : step_size * 50);
      }

      this.transform.position = UnityEngine.Vector3.MoveTowards(current : this.transform.position,
                                                                target : this._intermediate_target,
                                                                maxDistanceDelta : step_size);
    }

    #endregion
  }
}