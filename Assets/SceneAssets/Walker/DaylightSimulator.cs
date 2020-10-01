using System;
using droid.Runtime.Prototyping.EnvironmentListener;
using droid.Runtime.Utilities;
using UnityEngine;
using NeodroidUtilities = droid.Runtime.Utilities.Extensions.NeodroidUtilities;

namespace SceneAssets.Walker {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  [ExecuteInEditMode]
  [RequireComponent(requiredComponent : typeof(Light))]
  [RequireComponent(requiredComponent : typeof(ParticleSystem))]
  public class DaylightSimulator : EnvironmentListener {
    [SerializeField] float _day_atmosphere_thickness = 0.88f;

    [SerializeField] AnimationCurve _fog_density_curve;

    [SerializeField] Gradient _fog_gradient;
    [SerializeField] float _fog_scale = 0.2f;
    [SerializeField] Light _light;

    [SerializeField] Gradient _light_gradient;

    [SerializeField] float _max_ambient = 1f;

    [SerializeField] float _max_intensity = 1.34f;
    [SerializeField] float _min_ambient = 0.01f;
    [SerializeField] float _min_ambient_point = -0.2f;
    [SerializeField] float _min_intensity = 0.02f;
    [SerializeField] float _min_point = -0.2f;
    [SerializeField] float _night_atmosphere_thickness = 1.03f;

    [SerializeField] ParticleSystem _particle_system;
    [SerializeField] ParticleSystem.Particle[] _particles;

    [SerializeField] Vector3 _rotation = new Vector3(1f, 0f, 1f);

    [SerializeField] Material _sky_mat;
    [SerializeField] float _speed_multiplier = 1f;

    [SerializeField] Quaternion _start_rotation = Quaternion.identity;
    static readonly int _atmosphere_thickness = Shader.PropertyToID("_AtmosphereThickness");

    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public override string PrototypingTypeName { get { return "DaylightSimulator"; } }

    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public override void Setup() {
      if (this._fog_density_curve == null) {
        this._fog_density_curve = NeodroidDefaultsUtilities.DefaultAnimationCurve();
      }

      if (this._fog_gradient == null) {
        this._fog_gradient = NeodroidDefaultsUtilities.DefaultGradient();
      }

      if (this._light_gradient == null) {
        this._light_gradient = NeodroidDefaultsUtilities.DefaultGradient();
      }

      this._light = this.GetComponent<Light>();
      this._sky_mat = RenderSettings.skybox;
      this.transform.rotation = this._start_rotation;
      if (!this._particle_system) {
        this._particle_system = this.GetComponent<ParticleSystem>();
      }

      this._particle_system.Pause();
      if (this._particles == null || this._particles.Length < this._particle_system.main.maxParticles) {
        this._particles = new ParticleSystem.Particle[this._particle_system.main.maxParticles];
      }
    }

    void Update() {
      var a = 1 - this._min_point;
      var dot =
          Mathf.Clamp01(value : (Vector3.Dot(lhs : this._light.transform.forward, rhs : Vector3.down) - this._min_point) / a);
      var intensity = (this._max_intensity - this._min_intensity) * dot + this._min_intensity;

      this._light.intensity = intensity;

      var stars_intensity = this._min_intensity / intensity;
      var particle_color = new Color(1f,
                                     1f,
                                     1f,
                                     a : stars_intensity);

      var num_alive_particles = this._particle_system.GetParticles(particles : this._particles);
      for (var i = 0; i < num_alive_particles; i++) {
        this._particles[i].startColor = particle_color;
      }

      this._particle_system.SetParticles(particles : this._particles, size : num_alive_particles);

      a = 1 - this._min_ambient_point;
      dot = Mathf.Clamp01(value : (Vector3.Dot(lhs : this._light.transform.forward, rhs : Vector3.down) - this._min_ambient_point)
                                  / a);
      var ambient_intensity = (this._max_ambient - this._min_ambient) * dot + this._min_ambient;
      RenderSettings.ambientIntensity = ambient_intensity;

      this._light.color = this._light_gradient.Evaluate(time : dot);
      RenderSettings.ambientLight = this._light.color;

      RenderSettings.fogColor = this._fog_gradient.Evaluate(time : dot);
      RenderSettings.fogDensity = this._fog_density_curve.Evaluate(time : dot) * this._fog_scale;

      var atmosphere_thickness = (this._day_atmosphere_thickness - this._night_atmosphere_thickness) * dot
                                 + this._night_atmosphere_thickness;
      this._sky_mat.SetFloat(nameID : _atmosphere_thickness, value : atmosphere_thickness);

      this.transform.Rotate(eulers : this._rotation * Time.deltaTime * this._speed_multiplier);
    }

    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public override void PrototypingReset() { }
  }
}
