﻿#if BIOIK_EXISTS

using System.Collections;
using droid.Runtime.Interfaces;
using droid.Runtime.Messaging.Messages;
using droid.Runtime.Prototyping.Configurables;
using droid.Runtime.Utilities.Structs;
using UnityEngine;
namespace SceneAssets.Manipulator.Scripts {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  public class IkSolverEnablerConfigurable : Configurable {
    /// <summary>
    /// </summary>
    public Excluded.BioIK.BioIK _Enablee;

    /// <summary>
    ///
    /// </summary>
    public bool _disable_every_next_frame=false;
    /// <summary>
    ///
    /// </summary>
    public bool _disabled_by_default=false;

    const float _tolerance = float.Epsilon;



    /// <inheritdoc />
    /// <summary>
    /// </summary>
    protected override void PreSetup() {
      base.PreSetup();
      if (!this._Enablee) {
        this._Enablee = this.GetComponent<Excluded.BioIK.BioIK>();
      }

      if(this._disabled_by_default) {
        this.Disable();
      } else {
        this.Enable();
      }
    }

    /// <inheritdoc />
    /// <summary>
    /// </summary>
    /// <param name="obj"></param>
    public override void ApplyConfiguration(IConfigurableConfiguration obj) {
      if (Mathf.Abs(obj.ConfigurableValue) < _tolerance) {
        this.Disable();
      } else {
        this.Enable();
        if (this._disable_every_next_frame) {
          this.StartCoroutine(this.DisableNextFrame());
        }
      }
    }

    IEnumerator DisableNextFrame() {
      yield return new WaitForEndOfFrame();
      this.Disable();
    }

    /// <summary>
    ///
    /// </summary>
    public void Disable() {
      if (this._Enablee) {
        this._Enablee.enabled = false;
      }
    }

    /// <summary>
    ///
    /// </summary>
    public void Enable() {
      if (this._Enablee) {
        this._Enablee.enabled = true;
      }
    }

    /// <summary>
    /// </summary>
    /// <param name="random_generator"></param>
    /// <returns></returns>
    public IConfigurableConfiguration SampleConfiguration(Random random_generator) {
      return new Configuration(this.Identifier, Space1.DiscreteZeroOne.Sample());
    }

    /// <summary>
    /// </summary>
    public void ActiveToggle() {
      if (this._Enablee) {
        this._Enablee.enabled = !this._Enablee.enabled;
      }
    }

    /// <summary>
    /// </summary>
    public void Activate() {
      var conf = new Configuration(this.Identifier, 1);

      this.ApplyConfiguration(conf);
    }
  }
}
#endif