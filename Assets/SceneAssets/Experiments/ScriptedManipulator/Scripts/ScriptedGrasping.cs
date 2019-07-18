using System;
using System.Collections;
using System.Collections.Generic;
using droid.Runtime.GameObjects.ChildSensors;
using droid.Runtime.Utilities;
using droid.Runtime.Utilities.Extensions;
using droid.Runtime.Utilities.Grasping;
using SceneAssets.Experiments.ScriptedManipulator.Scripts.Grasps;
using SceneAssets.Experiments.ScriptedManipulator.Scripts.Navigation;
using SceneAssets.Experiments.ScriptedManipulator.Utilities;
using SceneAssets.Experiments.ScriptedManipulator.Utilities.Pathfinding;
using UnityEngine;
using NeodroidUtilities = droid.Runtime.Utilities.Extensions.NeodroidUtilities;

namespace SceneAssets.Experiments.ScriptedManipulator.Scripts {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  public class ScriptedGrasping : MonoBehaviour {


      public GraspableObject TargetGameObject
      {
        get { return this._target_game_object; }
        set
        {
          this._target_game_object = value;

          this.StopCoroutine("gripper_movement");
          this.StartCoroutine("gripper_movement", this.FollowPathToApproach1(this._target_game_object.transform));
        }
      }

       IEnumerator FollowPathToApproach1(Transform trans){
        while (true){
          if ((Vector3.Distance(this.transform.position, this._intermediate_target) <= this._precision)) {
            this._intermediate_target = this._path.Next(1);
          }

          if (this._debugging) {
            Debug.DrawRay(this._intermediate_target, this.transform.forward, Color.green);
          }

          this.transform.position = Vector3.MoveTowards(this.transform.position, this._intermediate_target, 1);
        }
      }



    /// <summary>
    ///
    /// </summary>
    /// <param name="state"></param>
    public void respawn_obstructions(GripperState state) {
      var obstacles_spawner = FindObjectOfType<ObstacleSpawner>();
      //obstacles_spawner.SpawnObstacles(obstacles_spawner.number_of_cubes, obstacles_spawner.number_of_spheres);
    }

    /// <summary>
    /// </summary>
    public States State { get { return this._state; } set { this._state = value; } }

    #region Helpers

    void PerformReactionToCurrentState(States state) {
      switch (state.PathFindingState) {
        case PathFindingState.Waiting_for_target_:
          this.FindTargetAndUpdatePath();
          if (this._target_grasp != null) {
            state.NavigateToTarget();
          }

          break;

        case PathFindingState.Navigating_:
          if (state.IsEnvironmentMoving() && this._wait_for_resting_environment) {
            state.WaitForRestingEnvironment();
            break;
          }

          if (state.WasEnvironmentMoving()) {
            this.FindTargetAndUpdatePath();
          }

          this.FollowPathToApproach(this._step_size, this._target_grasp.transform.rotation);
          state.OpenClaw();
          this.CheckIfGripperIsOpen();
          this.MaybeBeginApproachProcedure();
          break;

        case PathFindingState.Approaching_:
          this.ApproachTarget(this._step_size);
          break;

        case PathFindingState.Picking_up_target_:
          if (state.IsGripperClosed() && state.IsTargetNotGrabbed()) {
            //state.ReturnToStartPosition();
          }

          break;

        case PathFindingState.Waiting_for_resting_environment_:
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

        case PathFindingState.Returning_:
          if (state.WereObstructionMoving()) {
            this._path = this.FindPath(this.transform.position, this._reset_position);
          }

          if (this._wait_for_resting_environment) {
            if (state.IsObstructionsAtRest()) {
              this.FollowPathToApproach(this._step_size, Quaternion.Euler(90, 0, 0));
              this.MaybeBeginReleaseProcedure();
            }
          } else {
            this.FollowPathToApproach(this._step_size, Quaternion.Euler(90, 0, 0));
            this.MaybeBeginReleaseProcedure();
          }

          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      switch (state.GripperState) {
        case GripperState.Opening_:
          this.OpenClaws(this._step_size);
          break;

        case GripperState.Closing_:
          this.CloseClaws(this._step_size);
          break;

        case GripperState.Closed_:
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
        this._path = this.FindPath(this.transform.position, this._reset_position);
        this._intermediate_target = this._path.Next(0.001f);
      }

      this.MaybeClawIsClosed();
    }

    #endregion

    #region PrivateMembers

    GraspableObject _target_game_object;
    Vector3 _approach_position;
    Grasp _target_grasp;
    BezierCurvePath _path;

    Vector3 _default_motor_position;
    Vector3 _closed_motor_position;

    Vector3 _intermediate_target;
    //GameObject[] _targets;

    Vector3 _reset_position;
    BezierCurve _bezier_curve;

    float _step_size;

    #endregion

    #region PublicMembers

    [Space(1)]
    [Header("Game Objects")]
    [Tooltip("Game Object references")]
    [SerializeField]
    GameObject _claw_motor=null;

    [SerializeField] GameObject _grab_region=null;
    [SerializeField] GameObject _begin_grab_region=null;

    [SerializeField] GameObject _claw_1=null;
    [SerializeField] GameObject _claw_2=null;

    [SerializeField] States _state=null;
    [SerializeField] Transform _closed_motor_transform=null;
    [SerializeField] ObstacleSpawner _obstacle_spawner=null;
    [SerializeField] BezierCurve _bezier_curve_prefab=null;

    [Space(1)]
    [Header("Path Finding Parameters")]
    [SerializeField]
    float _search_boundary = 6f;

    [SerializeField] float _actor_size = 0.3f;
    [SerializeField] float _grid_granularity = 0.4f;
    [SerializeField] float _speed = 0.5f;
    [SerializeField] float _precision = 0.02f;
    [SerializeField] float _sensitivity = 0.2f;
    [SerializeField] float _approach_distance = 0.6f;
    [SerializeField] bool _wait_for_resting_environment =false;

    [Space(1)]
    [Header("Show debug logs")]
    [SerializeField]
    bool _debugging=false;

    [Space(1)]
    [Header("Draw Search Boundary")]
    [SerializeField]
    bool _draw_search_boundary = true;

    #endregion

    #region Setup

    void UpdateMeshFilterBounds() {
      var actor_bounds = GraspingUtilities.GetMaxBounds(this.gameObject);
      //_environment_size = agent_bounds.size.magnitude;
      //_approach_distance = agent_bounds.size.magnitude + _precision;
      this._actor_size =
          actor_bounds.extents.magnitude
          * 2; //Mathf.Max(agent_bounds.extents.x, Mathf.Max(agent_bounds.extents.y, agent_bounds.extents.z)) * 2;
      this._approach_distance = this._actor_size + this._precision;
    }

    void SetupEnvironment() {
      NeodroidRegistrationUtilities.RegisterCollisionTriggerCallbacksOnChildren<ChildCollider3DSensor, Collider, Collision>(this,
                                                                                                   this
                                                                                                       .transform,
                                                                                                   this
                                                                                                       .OnCollisionEnterChild,
                                                                                                   this
                                                                                                       .OnTriggerEnterChild,
                                                                                                   this
                                                                                                       .OnCollisionExitChild,
                                                                                                   this
                                                                                                       .OnTriggerExitChild,
                                                                                                   this
                                                                                                       .OnCollisionStayChild,
                                                                                                   this
                                                                                                       .OnTriggerStayChild,
                                                                                                   this
                                                                                                       ._debugging);
    }

    #endregion

    #region UnityCallbacks

    void Start() {
      this._state = new States();
      this._reset_position = this.transform.position;
      this._default_motor_position = this._claw_motor.transform.localPosition;
      this._closed_motor_position = this._closed_motor_transform.localPosition;
      this._bezier_curve = FindObjectOfType<BezierCurve>();
      if (!this._bezier_curve) {
        this._bezier_curve = Instantiate(this._bezier_curve_prefab);
      }

      this.UpdateMeshFilterBounds();

      this.FindTargetAndUpdatePath();
      this._state.ResetStates();
      this.SetupEnvironment();
    }

    void Update() {
      this._step_size = this._speed * Time.deltaTime;
      if (this._draw_search_boundary) {
        GraspingUtilities.DrawBoxFromCenter(this.transform.position, this._search_boundary, Color.magenta);
      }

      this._state.ObstructionMotionState =
          this._state.GetMotionState(FindObjectsOfType<Obstruction>(),
                                     this._state.ObstructionMotionState,
                                     this._sensitivity);
      this._state.TargetMotionState = this._state.GetMotionState(FindObjectsOfType<GraspableObject>(),
                                                                 this._state.TargetMotionState,
                                                                 this._sensitivity);

      this.PerformReactionToCurrentState(this._state);
    }

    #endregion

    #region StateTransitions

    void MaybeBeginReleaseProcedure() {
      if (!(Vector3.Distance(this.transform.position, this._reset_position) < this._precision)) {
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
      if (!(Vector3.Distance(this._claw_motor.transform.localPosition, this._closed_motor_position)
            < this._precision)) {
        return;
      }

      this._state.GripperIsClosed();
      this._path = this.FindPath(this.transform.position, this._reset_position);
      this._intermediate_target = this._path.Next(0.001f);
      this._state.ReturnToStartPosition();
    }

    void MaybeBeginApproachProcedure() {
      if (Vector3.Distance(this.transform.position, this._path._Target_Position)
          < this._approach_distance + this._precision
          && Quaternion.Angle(this.transform.rotation, this._target_grasp.transform.rotation)
          < this._precision
          && this._state.IsGripperOpen()) {
        this._state.ApproachTarget();
      }
    }

    void CheckIfGripperIsOpen() {
      if (Vector3.Distance(this._claw_motor.transform.localPosition, this._default_motor_position)
          < this._precision) {
        this._state.GripperIsOpen();
      }
    }

    void CloseClaws(float step_size) {
      //Vector3 current_motor_position = _motor.transform.localPosition;
      //current_motor_position.y += step_size / 16;
      //_motor.transform.localPosition = current_motor_position;
      this._claw_motor.transform.localPosition =
          Vector3.MoveTowards(this._claw_motor.transform.localPosition,
                              this._closed_motor_position,
                              step_size / 8);
    }

    void OpenClaws(float step_size) {
      this._claw_motor.transform.localPosition =
          Vector3.MoveTowards(this._claw_motor.transform.localPosition,
                              this._default_motor_position,
                              step_size / 8);
      //StopCoroutine ("claw_movement");
      //StartCoroutine ("claw_movement", OpenClaws1 ());
    }

    IEnumerator OpenClaws1() {
      while (!this._state.IsTargetGrabbed()) {
        this._claw_motor.transform.localPosition =
            Vector3.MoveTowards(this._claw_motor.transform.localPosition,
                                this._default_motor_position,
                                Time.deltaTime);
        yield return null; // new WaitForSeconds(waitTime);
      }
    }

    IEnumerator CloseClaws1() {
      while (!this._state.IsTargetGrabbed()) {
        this._claw_motor.transform.localPosition =
            Vector3.MoveTowards(this._claw_motor.transform.localPosition,
                                this._closed_motor_position,
                                Time.deltaTime);
        yield return null; // new WaitForSeconds(waitTime);
      }
    }

    #endregion

    #region Collisions

    void Ab(GameObject child_game_object, GraspableObject other_maybe_graspable) {
      if (other_maybe_graspable) {
        if (child_game_object == this._grab_region.gameObject
            && other_maybe_graspable.gameObject == this._target_game_object.gameObject) {
          if (this._debugging) {
            Debug.Log($"Target {other_maybe_graspable.name} is inside region");
          }

          this._state.TargetIsInsideRegion();
        }

        if (child_game_object == this._begin_grab_region.gameObject
            && other_maybe_graspable.gameObject == this._target_game_object.gameObject
            && !this._state.IsTargetGrabbed()) {
          if (this._debugging) {
            Debug.Log($"Picking up target {other_maybe_graspable.name}");
          }

          this._state.PickUpTarget();
        }
      }
    }

    void OnTriggerEnterChild(GameObject child_game_object, Collider other_game_object) {
      var other_maybe_graspable = other_game_object.GetComponentInParent<GraspableObject>();
      this.Ab(child_game_object, other_maybe_graspable);
    }

    void OnTriggerStayChild(GameObject child_game_object, Collider other_game_object) {
      var other_maybe_graspable = other_game_object.GetComponentInParent<GraspableObject>();
      this.Ab(child_game_object, other_maybe_graspable);
    }

    void OnCollisionStayChild(GameObject child_game_object, Collision collision) {
      var other_maybe_graspable = collision.gameObject.GetComponentInParent<GraspableObject>();
      if (other_maybe_graspable) {
        if (child_game_object == this._claw_1.gameObject
            && other_maybe_graspable.gameObject == this._target_game_object.gameObject) {
          if (this._debugging) {
            Debug.Log($"Target {other_maybe_graspable.name} is touching {child_game_object.name}");
          }

          this._state.Claw1IsTouchingTarget();
        }

        if (child_game_object == this._claw_2.gameObject
            && other_maybe_graspable.gameObject == this._target_game_object.gameObject) {
          if (this._debugging) {
            Debug.Log($"Target {other_maybe_graspable.name} is touching {child_game_object.name}");
          }

          this._state.Claw2IsTouchingTarget();
        }
      }
    }

    void OnCollisionExitChild(GameObject child_game_object, Collision collision) {
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

    void OnTriggerExitChild(GameObject child_game_object, Collider other_game_object) {
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

    void OnCollisionEnterChild(GameObject child_game_object, Collision collision) {
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

    Tuple<GraspableObject, Grasp> GetOptimalTargetAndGrasp() {
      var targets = FindObjectsOfType<GraspableObject>();
      if (targets.Length == 0) {
        return null;
      }

      var shortest_distance = float.MaxValue;
      GraspableObject optimal_target = null;
      Grasp optimal_grasp = null;
      foreach (var target in targets) {
        var pair = target.GetOptimalGrasp(this);
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

      return new Tuple<GraspableObject, Grasp>(optimal_target, optimal_grasp);
    }

    public void FindTargetAndUpdatePath() {
      var pair = this.GetOptimalTargetAndGrasp();
      if (pair == null || pair.Item1 == null || pair.Item2 == null) {
        this._state.PathFindingState = PathFindingState.Returning_;
        this._path = this.FindPath(this.transform.position, this._reset_position);
        return;
      }

      this._target_game_object = pair.Item1;
      this._target_grasp = pair.Item2;
      var transform1 = this._target_grasp.transform;
      this._approach_position = transform1.position - transform1.forward * this._approach_distance;
      if (Vector3.Distance(this.transform.position, this._approach_position) > this._search_boundary) {
        return;
      }

      this._path = this.FindPath(this.transform.position, this._approach_position);
      this._intermediate_target = this._path.Next(this._step_size);
    }

    BezierCurvePath FindPath(Vector3 start_position, Vector3 target_position) {
      this.UpdateMeshFilterBounds();
      var path_list = Astar.FindPathAstar(start_position,
                                          target_position,
                                          this._search_boundary,
                                          this._grid_granularity,
                                          this._actor_size,
                                          this._approach_distance);
      if (path_list != null && path_list.Count > 0) {
        path_list = Astar.SimplifyPath(path_list, this._actor_size);
        path_list.Add(target_position);
      } else {
        path_list = new List<Vector3> {start_position, target_position};
      }

      var path = new BezierCurvePath(start_position, target_position, this._bezier_curve, path_list);
      return path;
    }

    void ApproachTarget(float step_size) {
      this.transform.position =
          Vector3.MoveTowards(this.transform.position, this._target_grasp.transform.position, step_size);
      if (this._debugging) {
        Debug.DrawLine(this.transform.position, this._target_grasp.transform.position, Color.green);
      }
    }

    void FollowPathToApproach(float step_size, Quaternion rotation, bool rotate = true) {
      if (Vector3.Distance(this.transform.position, this._intermediate_target) <= this._precision) {
        this._intermediate_target = this._path.Next(step_size);
      }

      if (this._debugging) {
        Debug.DrawRay(this._intermediate_target, this.transform.forward, Color.magenta);
      }

      if (rotate) {
        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, rotation, step_size * 50);
      }

      this.transform.position =
          Vector3.MoveTowards(this.transform.position, this._intermediate_target, step_size);
    }

    #endregion
  }
}
