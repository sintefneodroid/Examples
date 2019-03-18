using System;
using droid.Runtime.Utilities.Misc.Orientation;

namespace SceneAssets.Experiments.ScriptedManipulator.Utilities {
  #region Enums

  /// <summary>
  /// 
  /// </summary>
  public enum MotionState {
    /// <summary>
    /// 
    /// </summary>
    Is_at_rest_,
    /// <summary>
    /// 
    /// </summary>
    Was_moving_,
    /// <summary>
    /// 
    /// </summary>
    Is_moving_
  }

  /// <summary>
  /// 
  /// </summary>
  public enum PathFindingState {
    /// <summary>
    /// 
    /// </summary>
    Waiting_for_target_,
    /// <summary>
    /// 
    /// </summary>
    Waiting_for_resting_environment_,
    /// <summary>
    /// 
    /// </summary>
    Navigating_,
    /// <summary>
    /// 
    /// </summary>
    Approaching_,
    /// <summary>
    /// 
    /// </summary>
    Picking_up_target_,
    /// <summary>
    /// 
    /// </summary>
    Returning_
  }

  /// <summary>
  /// 
  /// </summary>
  public enum GripperState {
    /// <summary>
    /// 
    /// </summary>
    Closed_,
    /// <summary>
    /// 
    /// </summary>
    Open_,
    /// <summary>
    /// 
    /// </summary>
    Closing_,
    /// <summary>
    /// 
    /// </summary>
    Opening_
  }

  /// <summary>
  /// 
  /// </summary>
  public enum ClawState {
    /// <summary>
    /// 
    /// </summary>
    Touching_target_,
    /// <summary>
    /// 
    /// </summary>
    Not_touching_target_
  }

  /// <summary>
  /// 
  /// </summary>
  public enum TargetState {
    /// <summary>
    /// 
    /// </summary>
    Grabbed_,
    /// <summary>
    /// 
    /// </summary>
    Not_grabbed_,
    /// <summary>
    /// 
    /// </summary>
    Outside_region_,
    /// <summary>
    /// 
    /// </summary>
    Inside_region_
  }

  #endregion

  /// <summary>
  /// 
  /// </summary>
  public class States {
    ClawState _current_claw_1_state, _current_claw_2_state;

    GripperState _current_gripper_state;
    PathFindingState _current_path_finding_state;

    TargetState _current_target_state;

    MotionState _obstruction_motion_state, _target_motion_state;
    Action _on_state_update_callback;

    public States(Action on_state_update_callback = null) {
      this._on_state_update_callback = on_state_update_callback;
    }

    public ClawState Claw1State {
      get { return this._current_claw_1_state; }
      set {
        this._current_claw_1_state = value;
        this._on_state_update_callback?.Invoke();
      }
    }

    public ClawState Claw2State {
      get { return this._current_claw_2_state; }
      set {
        this._current_claw_2_state = value;
        this._on_state_update_callback?.Invoke();
      }
    }

    public TargetState TargetState {
      get { return this._current_target_state; }
      set {
        this._current_target_state = value;
        this._on_state_update_callback?.Invoke();
      }
    }

    public GripperState GripperState {
      get { return this._current_gripper_state; }
      set {
        this._current_gripper_state = value;
        this._on_state_update_callback?.Invoke();
      }
    }

    public PathFindingState PathFindingState {
      get { return this._current_path_finding_state; }
      set {
        this._current_path_finding_state = value;
        this._on_state_update_callback?.Invoke();
      }
    }

    public MotionState ObstructionMotionState {
      get { return this._obstruction_motion_state; }
      set {
        this._obstruction_motion_state = value;
        this._on_state_update_callback?.Invoke();
      }
    }

    public MotionState TargetMotionState {
      get { return this._target_motion_state; }
      set {
        this._target_motion_state = value;
        this._on_state_update_callback?.Invoke();
      }
    }

    public MotionState GetMotionState<T>(T[] objects, MotionState previous_state, float sensitivity = 0.1f)
        where T : IMotionTracker {
      foreach (var o in objects) {
        if (o.IsInMotion(sensitivity)) {
          return MotionState.Is_moving_;
        }
      }

      return previous_state != MotionState.Is_moving_ ? MotionState.Is_at_rest_ : MotionState.Was_moving_;
    }

    public void TargetIsGrabbed() {
      this.TargetState = TargetState.Grabbed_;
      this.GripperState = GripperState.Closed_;
      this.PathFindingState = PathFindingState.Returning_;
    }

    public void OpenClaw() { this.GripperState = GripperState.Opening_; }

    public void WaitForRestingEnvironment() {
      this.PathFindingState = PathFindingState.Waiting_for_resting_environment_;
    }

    public void ReturnToStartPosition() { this.PathFindingState = PathFindingState.Returning_; }

    public void NavigateToTarget() { this.PathFindingState = PathFindingState.Navigating_; }

    public bool IsTargetGrabbed() { return this.TargetState == TargetState.Grabbed_; }

    public void WaitForTarget() { this.PathFindingState = PathFindingState.Waiting_for_target_; }

    public bool IsTargetNotGrabbed() { return this.TargetState != TargetState.Grabbed_; }

    public bool IsGripperClosed() { return this.GripperState == GripperState.Closed_; }

    public bool IsEnvironmentAtRest() {
      return this.TargetMotionState == MotionState.Is_at_rest_
             && this.ObstructionMotionState == MotionState.Is_at_rest_;
    }

    public bool WasEnvironmentMoving() {
      return this.ObstructionMotionState == MotionState.Was_moving_
             || this.TargetMotionState == MotionState.Was_moving_;
    }

    public bool IsEnvironmentMoving() {
      return this.ObstructionMotionState == MotionState.Is_moving_
             || this.TargetMotionState == MotionState.Is_moving_;
    }

    public bool WereObstructionMoving() { return this.ObstructionMotionState == MotionState.Was_moving_; }

    public bool IsObstructionsAtRest() { return this.ObstructionMotionState == MotionState.Is_at_rest_; }

    public bool AreBothClawsTouchingTarget() {
      return this.Claw1State == ClawState.Not_touching_target_
             && this.Claw2State == ClawState.Not_touching_target_;
    }

    public void TargetIsNotGrabbed() { this.TargetState = TargetState.Not_grabbed_; }

    public bool IsTargetTouchingAndInsideRegion() {
      return this.Claw1State == ClawState.Touching_target_
             && this.Claw2State == ClawState.Touching_target_
             && this.TargetState == TargetState.Inside_region_;
    }

    public bool IsTargetInsideRegionOrTouching() {
      return this.Claw1State == ClawState.Touching_target_
             || this.Claw2State == ClawState.Touching_target_
             || this.TargetState == TargetState.Inside_region_;
    }

    public bool IsGripperOpen() { return this.GripperState == GripperState.Open_; }

    public void ApproachTarget() { this.PathFindingState = PathFindingState.Approaching_; }

    public void GripperIsOpen() { this.GripperState = GripperState.Open_; }

    public void PickUpTarget() {
      //TargetState = TargetState.InsideRegion;
      this.GripperState = GripperState.Closing_;
      this.PathFindingState = PathFindingState.Picking_up_target_;
    }

    public void GripperIsClosed() { this.GripperState = GripperState.Closed_; }

    public void Claw1IsNotTouchingTarget() {
      this.Claw1State = ClawState.Not_touching_target_;
      //TargetIsNotGrabbed();
    }

    public void Claw2IsNotTouchingTarget() {
      this.Claw2State = ClawState.Not_touching_target_;
      //TargetIsNotGrabbed();
    }

    public void Claw2IsTouchingTarget() { this.Claw2State = ClawState.Touching_target_; }

    public void Claw1IsTouchingTarget() { this.Claw1State = ClawState.Touching_target_; }

    public void TargetIsOutsideRegion() { this.TargetState = TargetState.Outside_region_; }

    public void TargetIsInsideRegion() { this.TargetState = TargetState.Inside_region_; }

    public bool IsPickingUpTarget() { return this.PathFindingState == PathFindingState.Picking_up_target_; }

    public void ResetStates() {
      this.TargetState = TargetState.Outside_region_;
      this.ObstructionMotionState = MotionState.Is_at_rest_;
      this.TargetMotionState = MotionState.Is_at_rest_;
      this.GripperState = GripperState.Open_;
      this.PathFindingState = PathFindingState.Waiting_for_target_;
      this.Claw1State = ClawState.Not_touching_target_;
      this.Claw2State = ClawState.Not_touching_target_;
    }
  }
}
