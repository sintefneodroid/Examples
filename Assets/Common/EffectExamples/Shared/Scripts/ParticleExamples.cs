using System;
using UnityEngine;

namespace Common.EffectExamples.Shared.Scripts {
  /// <summary>
  /// 
  /// </summary>
  [Serializable]
  public class ParticleExamples {
    public string _Title;
    [TextArea] public string _Description;
    public bool _IsWeaponEffect;
    public GameObject _ParticleSystemGo;
    public Vector3 _ParticlePosition, _ParticleRotation;
  }
}
