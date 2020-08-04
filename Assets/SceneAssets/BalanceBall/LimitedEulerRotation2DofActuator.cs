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
  [AddComponentMenu(menuName : ActuatorComponentMenuPath._ComponentMenuPath
                               + "LimitedEulerRotationActuator2Dof"
                               + ActuatorComponentMenuPath._Postfix)]
  public class LimitedEulerRotation2DofActuator : Actuator {
    /// <inheritdoc />
    ///  <summary>
    ///  </summary>
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
          NeodroidRegistrationUtilities.RegisterComponent(r : (IHasRegister<IActuator>)this.Parent,
                                                          c : this,
                                                          identifier : this._x);
      this.Parent =
          NeodroidRegistrationUtilities.RegisterComponent(r : (IHasRegister<IActuator>)this.Parent,
                                                          c : this,
                                                          identifier : this._z);
    }

    /// <inheritdoc />
    ///  <summary>
    ///  </summary>
    protected override void UnRegisterComponent() {
      this.Parent?.UnRegister(t : this, obj : this._x);
      this.Parent?.UnRegister(t : this, obj : this._z);
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
        Debug.Log(message : $"Inner Applying {m} To {this.name}");
        Debug.Log(message : $"Current rotation of {this.name} is {this.transform.rotation}");
      }
      #endif

      if (motion.ActuatorName == this._x) {
        this.transform.Rotate(axis : Vector3.forward, angle : m);
      } else if (motion.ActuatorName == this._z) {
        this.transform.Rotate(axis : Vector3.right, angle : m);
      }

      this.transform.rotation = RotationClamping.ClampRotation(temp_eulers : this.transform.rotation.eulerAngles,
                                                               low : this.limits.Max.x,
                                                               high : this.limits.Max.x);
    }

    public override String[] InnerMotionNames { get; }

    #if UNITY_EDITOR
    void OnDrawGizmosSelected() {
      if (this.enabled) {
        var position = this.transform.position;

        Handles.DrawWireArc(center : this.transform.position,
                            normal : this.transform.right,
                            @from : -this.transform.forward,
                            angle : 180,
                            radius : 2);

        Handles.DrawWireArc(center : this.transform.position,
                            normal : this.transform.forward,
                            @from : -this.transform.right,
                            angle : 180,
                            radius : 2);
      }
    }

    #endif
  }
}
