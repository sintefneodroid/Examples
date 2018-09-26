using System.Collections.Generic;
using UnityEngine;

namespace Common.EffectExamples.Shared.Scripts {
  /// <inheritdoc />
  /// <summary>
  /// This script demonstrate how to use the particle system collision callback.
  /// The sample using it is the "Extinguish" prefab. It use a second, non displayed
  /// particle system to lighten the load of collision detection.
  /// </summary>
  public class ParticleCollision : MonoBehaviour {
    List<ParticleCollisionEvent> _m_collision_events = new List<ParticleCollisionEvent>();
    ParticleSystem _m_particle_system;

    void Start() { this._m_particle_system = this.GetComponent<ParticleSystem>(); }

    void OnParticleCollision(GameObject other) {
      var num_collision_events = this._m_particle_system.GetCollisionEvents(other, this._m_collision_events);
      for (var i = 0; i < num_collision_events; ++i) {
        var col = this._m_collision_events[i].colliderComponent;

        var fire = col.GetComponent<ExtinguishableFire>();
        if (fire != null) {
          fire.Extinguish();
        }
      }
    }
  }
}
