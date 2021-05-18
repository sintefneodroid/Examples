using droid.Runtime.Environments;
using droid.Runtime.Prototyping.EnvironmentListener;
using TMPro;
using UnityEngine;

namespace SceneAssets.Tests {
  public class TextMeshProCounter : EnvironmentListener {
    [SerializeField] TextMeshProUGUI _a;
    [SerializeField] NeodroidEnvironment _b;

    public override void PreSetup() {
      base.PreSetup();
      if (!this._a) {
        this._a = this.GetComponent<TextMeshProUGUI>();
      }

      if (!this._b) {
        this._b = this.GetComponentInParent<NeodroidEnvironment>();
      }
    }

    public override void Step() {
      base.Step();
      if (this._a && this._b) {
        this._a.text = this._b.StepI.ToString();
      } else if (this._a) {
        this._a.text = "Env Not Found!";
      }
    }
  }
}