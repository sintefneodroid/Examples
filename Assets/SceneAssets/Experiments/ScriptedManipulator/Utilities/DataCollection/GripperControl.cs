using System.Globalization;
using SceneAssets.Experiments.ScriptedManipulator.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SceneAssets.Experiments.ScriptedManipulator.Utilities.DataCollection {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  public class GripperControl : MonoBehaviour {
    [SerializeField] Text _claw1_state=null;
    [SerializeField] Text _claw2_state=null;
    [SerializeField] Text _env_state=null;

    [Space]
    [Header("State Panel")]
    [SerializeField]
    Text _gripper_state=null;

    //float gripper_target_distance;
    //int iterations;
    //int obstacle_num;
    [SerializeField] ScriptedGrasping _pf=null;
    [SerializeField] Text _pf_state=null;

    [SerializeField] Slider _s_distance=null;
    [SerializeField] Slider _s_obstacle=null;

    [SerializeField] Text _t_gripper_target_distance=null;
    [SerializeField] Text _t_obstacle_num=null;
    [SerializeField] Text _t_waiting=null;

    [SerializeField] Targets _target;
    [SerializeField] Text _target_state=null;

    void Start() {
      this._pf = FindObjectOfType<ScriptedGrasping>();
      this._t_gripper_target_distance.text = this._s_distance.value.ToString("0.00");
      this._t_obstacle_num.text = this._s_obstacle.value.ToString(CultureInfo.InvariantCulture);
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
                                 ? $"Detecting movement\nWaiting..."
                                 : "";
    }

    public void DistanceSlider() {
      this._t_gripper_target_distance.text = this._s_distance.value.ToString("0.00");
    }

    public void ObstacleSlider() {
      this._t_obstacle_num.text = this._s_obstacle.value.ToString(CultureInfo.InvariantCulture);
    }

    public void ChooseTarget() {
      switch (EventSystem.current.currentSelectedGameObject.name) {
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

      Debug.Log("Target = " + this._target);
    }

    enum Targets {
      Sill_,
      Sardin_,
      Button_
    }
  }
}
