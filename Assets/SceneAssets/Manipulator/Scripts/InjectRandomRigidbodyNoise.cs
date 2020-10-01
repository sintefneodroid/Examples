﻿using UnityEngine;

namespace SceneAssets.Manipulator.Scripts {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  public class InjectRandomRigidbodyNoise : MonoBehaviour {
    [SerializeField] float _magnitude = 0.1f;
    [SerializeField] ForceMode _forceMode = ForceMode.Impulse;
    Rigidbody _rigidbody;

    void Start() { this._rigidbody = this.GetComponent<Rigidbody>(); }

    void Update() {
      this._rigidbody.AddForce(force : Random.insideUnitSphere * this._magnitude, mode : this._forceMode);
      this._rigidbody.AddTorque(torque : Random.insideUnitSphere * this._magnitude, mode : this._forceMode);
    }
  }
}
