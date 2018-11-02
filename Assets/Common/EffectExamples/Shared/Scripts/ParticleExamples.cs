using System;
using UnityEngine;

namespace Common.EffectExamples.Shared.Scripts {
  /// <summary>
  /// </summary>
  [Serializable]
  public class ParticleExamples {
    [TextArea] public string _Description;
    public bool _IsWeaponEffect;
    public Vector3 _ParticlePosition, _ParticleRotation;
    public GameObject _ParticleSystemGo;
    public string _Title;
  }
}