namespace SceneAssets.Experiments.ScriptedManipulator.Utilities.DataCollection {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  public class GripperControl : UnityEngine.MonoBehaviour {
    [UnityEngine.SerializeField] UnityEngine.UI.Text _claw1_state = null;
    [UnityEngine.SerializeField] UnityEngine.UI.Text _claw2_state = null;
    [UnityEngine.SerializeField] UnityEngine.UI.Text _env_state = null;

    [UnityEngine.SpaceAttribute]
    [UnityEngine.HeaderAttribute("State Panel")]
    [UnityEngine.SerializeField]
    UnityEngine.UI.Text _gripper_state = null;

    //float gripper_target_distance;
    //int iterations;
    //int obstacle_num;
    [UnityEngine.SerializeField]
    SceneAssets.Experiments.ScriptedManipulator.Scripts.ScriptedGrasping _pf = null;

    [UnityEngine.SerializeField] UnityEngine.UI.Text _pf_state = null;

    [UnityEngine.SerializeField] UnityEngine.UI.Slider _s_distance = null;
    [UnityEngine.SerializeField] UnityEngine.UI.Slider _s_obstacle = null;

    [UnityEngine.SerializeField] UnityEngine.UI.Text _t_gripper_target_distance = null;
    [UnityEngine.SerializeField] UnityEngine.UI.Text _t_obstacle_num = null;
    [UnityEngine.SerializeField] UnityEngine.UI.Text _t_waiting = null;

    [UnityEngine.SerializeField] Targets _target;
    [UnityEngine.SerializeField] UnityEngine.UI.Text _target_state = null;

    void Start() {
      this._pf = FindObjectOfType<SceneAssets.Experiments.ScriptedManipulator.Scripts.ScriptedGrasping>();
      this._t_gripper_target_distance.text = this._s_distance.value.ToString("0.00");
      this._t_obstacle_num.text =
          this._s_obstacle.value.ToString(provider : System.Globalization.CultureInfo.InvariantCulture);
      this._t_waiting.text = "";
    }

    void Update() {
      this._gripper_state.text = this._pf.State.GripperState.ToString();
      this._env_state.text = this._pf.State.ObstructionMotionState.ToString();
      this._pf_state.text = this._pf.State.PathFindingState.ToString();
      this._target_state.text = this._pf.State.TargetState.ToString();
      this._claw1_state.text = this._pf.State.Claw1State.ToString();
      this._claw2_state.text = this._pf.State.Claw2State.ToString();
      this._t_waiting.text = this._pf.State.PathFindingState == PathFindingState.Waiting_for_target_
                                 ? "Detecting movement\nWaiting..."
                                 : "";
    }

    public void DistanceSlider() {
      this._t_gripper_target_distance.text = this._s_distance.value.ToString("0.00");
    }

    public void ObstacleSlider() {
      this._t_obstacle_num.text =
          this._s_obstacle.value.ToString(provider : System.Globalization.CultureInfo.InvariantCulture);
    }

    public void ChooseTarget() {
      switch (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name) {
        case "Sill":
          this._target = Targets.Sill_;
          break;

        case "Sardin":
          this._target = Targets.Sardin_;
          break;

        case "Button":
          this._target = Targets.Button_;
          break;
      }

      UnityEngine.Debug.Log(message : "Target = " + this._target);
    }

    enum Targets {
      Sill_,
      Sardin_,
      Button_
    }
  }
}