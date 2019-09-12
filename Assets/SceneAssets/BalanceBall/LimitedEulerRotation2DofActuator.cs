using System;
using droid.Runtime.Interfaces;
using droid.Runtime.Prototyping.Actuators;
using droid.Runtime.Structs.Space;
using droid.Runtime.Utilities;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

#endif

namespace SceneAssets.BalanceBall {
  /// <inheritdoc />
  [AddComponentMenu(ActuatorComponentMenuPath._ComponentMenuPath
                    + "LimitedEulerRotationActuator2Dof"
                    + ActuatorComponentMenuPath._Postfix)]
  public class LimitedEulerRotation2DofActuator : Actuator {
    /// <summary>
    ///
    /// </summary>
    public override string PrototypingTypeName { get { return "EulerRotation"; } }

    /// <summary>
    ///
    /// </summary>
    [SerializeField]
    protected Space2 limits = Space2.MinusOneOne * 0.25f;

    [SerializeField] string _x;
    [SerializeField] string _z;

    /// <inheritdoc />
    /// <summary>
    /// </summary>
    protected override void RegisterComponent() {
      this._x = this.Identifier + "RotX_";

      this._z = this.Identifier + "RotZ_";

      this.Parent =
          NeodroidRegistrationUtilities.RegisterComponent((IHasRegister<IActuator>)this.Parent,
                                                          this,
                                                          this._x);
      this.Parent =
          NeodroidRegistrationUtilities.RegisterComponent((IHasRegister<IActuator>)this.Parent,
                                                          this,
                                                          this._z);
    }

    /// <summary>
    ///
    /// </summary>
    protected override void UnRegisterComponent() {
      this.Parent?.UnRegister(this, this._x);
      this.Parent?.UnRegister(this, this._z);
    }

    /// <inheritdoc />
    /// <summary>
    /// </summary>
    /// <param name="motion"></param>
    /// <exception cref="T:System.ArgumentOutOfRangeException"></exception>
    protected override void InnerApplyMotion(IMotion motion) {
      var m = motion.Strength;
      #if NEODROID_DEBUG
      if (this.Debugging) {
        Debug.Log($"Inner Applying {m} To {this.name}");
        Debug.Log($"Current rotation of {this.name} is {this.transform.rotation}");
      }
      #endif

      if (motion.ActuatorName == this._x) {
        this.transform.Rotate(Vector3.forward, m);
      } else if (motion.ActuatorName == this._z) {
        this.transform.Rotate(Vector3.right, m);
      }

      this.transform.rotation = RotationClamping.ClampRotation(this.transform.rotation.eulerAngles,
                                                               this.limits.Max.x,
                                                               this.limits.Max.x);
    }

    public override String[] InnerMotionNames { get; }

    #if UNITY_EDITOR
    void OnDrawGizmosSelected() {
      if (this.enabled) {
        var position = this.transform.position;

        Handles.DrawWireArc(this.transform.position,
                            this.transform.right,
                            -this.transform.forward,
                            180,
                            2);

        Handles.DrawWireArc(this.transform.position,
                            this.transform.forward,
                            -this.transform.right,
                            180,
                            2);
      }
    }

    #endif
  }
}
