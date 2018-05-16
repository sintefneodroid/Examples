using System.Collections;
using UnityEngine;

namespace EffectExamples.Shared.Scripts {
  /// <summary>
  /// This simulate an extinguishable fire, 
  /// </summary>
  public class ExtinguishableFire : MonoBehaviour {
    public ParticleSystem _FireParticleSystem;
    public ParticleSystem _SmokeParticleSystem;

    protected bool _M_IsExtinguished;

    const float _m_fire_starting_time = 2.0f;

    void Start() {
      this._M_IsExtinguished = true;

      this._SmokeParticleSystem.Stop();
      this._FireParticleSystem.Stop();

      this.StartCoroutine(this.StartingFire());
    }

    public void Extinguish() {
      if (this._M_IsExtinguished) {
        return;
      }

      this._M_IsExtinguished = true;
      this.StartCoroutine(this.Extinguishing());
    }

    IEnumerator Extinguishing() {
      this._FireParticleSystem.Stop();
      this._SmokeParticleSystem.time = 0;
      this._SmokeParticleSystem.Play();

      var elapsed_time = 0.0f;
      while (elapsed_time < _m_fire_starting_time) {
        var ratio = Mathf.Max(0.0f, 1.0f - (elapsed_time / _m_fire_starting_time));

        this._FireParticleSystem.transform.localScale = Vector3.one * ratio;

        yield return null;

        elapsed_time += Time.deltaTime;
      }

      yield return new WaitForSeconds(2.0f);

      this._SmokeParticleSystem.Stop();
      this._FireParticleSystem.transform.localScale = Vector3.one;

      yield return new WaitForSeconds(4.0f);

      this.StartCoroutine(this.StartingFire());
    }

    IEnumerator StartingFire() {
      this._SmokeParticleSystem.Stop();
      this._FireParticleSystem.time = 0;
      this._FireParticleSystem.Play();

      var elapsed_time = 0.0f;
      while (elapsed_time < _m_fire_starting_time) {
        var ratio = Mathf.Min(1.0f, (elapsed_time / _m_fire_starting_time));

        this._FireParticleSystem.transform.localScale = Vector3.one * ratio;

        yield return null;

        elapsed_time += Time.deltaTime;
      }

      this._FireParticleSystem.transform.localScale = Vector3.one;
      this._M_IsExtinguished = false;
    }
  }
}
