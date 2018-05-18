using System;
using UnityEngine;

namespace EffectExamples.Shared.Scripts {
  [Serializable]
  public class ParticleExamples {
    public string _Title;
    [TextArea] public string _Description;
    public bool _IsWeaponEffect;
    public GameObject _ParticleSystemGo;
    public Vector3 _ParticlePosition, _ParticleRotation;
  }
}
