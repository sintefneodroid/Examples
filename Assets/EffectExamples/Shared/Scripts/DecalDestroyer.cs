using System.Collections;
using UnityEngine;

namespace EffectExamples.Shared.Scripts {
  public class DecalDestroyer : MonoBehaviour {
    public float _LifeTime = 5.0f;

    IEnumerator Start() {
      yield return new WaitForSeconds(this._LifeTime);
      Destroy(this.gameObject);
    }
  }
}
