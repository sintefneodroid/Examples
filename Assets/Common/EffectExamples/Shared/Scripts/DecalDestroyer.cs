using System.Collections;
using UnityEngine;

namespace Common.EffectExamples.Shared.Scripts {
  /// <summary>
  /// </summary>
  public class DecalDestroyer : MonoBehaviour {
    public float _LifeTime = 5.0f;

    IEnumerator Start() {
      yield return new WaitForSeconds(this._LifeTime);
      Destroy(this.gameObject);
    }
  }
}