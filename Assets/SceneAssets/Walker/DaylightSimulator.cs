namespace SceneAssets.Walker {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  [UnityEngine.ExecuteInEditMode]
  [UnityEngine.RequireComponent(requiredComponent : typeof(UnityEngine.Light))]
  [UnityEngine.RequireComponent(requiredComponent : typeof(UnityEngine.ParticleSystem))]
  public class DaylightSimulator : droid.Runtime.Prototyping.EnvironmentListener.EnvironmentListener {
    static readonly int _atmosphere_thickness = UnityEngine.Shader.PropertyToID("_AtmosphereThickness");
    [UnityEngine.SerializeField] float _day_atmosphere_thickness = 0.88f;

    [UnityEngine.SerializeField] UnityEngine.AnimationCurve _fog_density_curve;

    [UnityEngine.SerializeField] UnityEngine.Gradient _fog_gradient;
    [UnityEngine.SerializeField] float _fog_scale = 0.2f;
    [UnityEngine.SerializeField] UnityEngine.Light _light;

    [UnityEngine.SerializeField] UnityEngine.Gradient _light_gradient;

    [UnityEngine.SerializeField] float _max_ambient = 1f;

    [UnityEngine.SerializeField] float _max_intensity = 1.34f;
    [UnityEngine.SerializeField] float _min_ambient = 0.01f;
    [UnityEngine.SerializeField] float _min_ambient_point = -0.2f;
    [UnityEngine.SerializeField] float _min_intensity = 0.02f;
    [UnityEngine.SerializeField] float _min_point = -0.2f;
    [UnityEngine.SerializeField] float _night_atmosphere_thickness = 1.03f;

    [UnityEngine.SerializeField] UnityEngine.ParticleSystem _particle_system;

    [UnityEngine.SerializeField] UnityEngine.Vector3 _rotation = new UnityEngine.Vector3(1f, 0f, 1f);

    [UnityEngine.SerializeField] UnityEngine.Material _sky_mat;
    [UnityEngine.SerializeField] float _speed_multiplier = 1f;

    [UnityEngine.SerializeField] UnityEngine.Quaternion _start_rotation = UnityEngine.Quaternion.identity;
    [UnityEngine.SerializeField] UnityEngine.ParticleSystem.Particle[] _particles;

    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public override string PrototypingTypeName { get { return "DaylightSimulator"; } }

    void Update() {
      var a = 1 - this._min_point;
      var dot =
          UnityEngine.Mathf.Clamp01(value : (UnityEngine.Vector3.Dot(lhs : this._light.transform.forward,
                                                                     rhs : UnityEngine.Vector3.down)
                                             - this._min_point)
                                            / a);
      var intensity = (this._max_intensity - this._min_intensity) * dot + this._min_intensity;

      this._light.intensity = intensity;

      var stars_intensity = this._min_intensity / intensity;
      var particle_color = new UnityEngine.Color(1f,
                                                 1f,
                                                 1f,
                                                 a : stars_intensity);

      var num_alive_particles = this._particle_system.GetParticles(particles : this._particles);
      for (var i = 0; i < num_alive_particles; i++) {
        this._particles[i].startColor = particle_color;
      }

      this._particle_system.SetParticles(particles : this._particles, size : num_alive_particles);

      a = 1 - this._min_ambient_point;
      dot = UnityEngine.Mathf.Clamp01(value : (UnityEngine.Vector3.Dot(lhs : this._light.transform.forward,
                                                                       rhs : UnityEngine.Vector3.down)
                                               - this._min_ambient_point)
                                              / a);
      var ambient_intensity = (this._max_ambient - this._min_ambient) * dot + this._min_ambient;
      UnityEngine.RenderSettings.ambientIntensity = ambient_intensity;

      this._light.color = this._light_gradient.Evaluate(time : dot);
      UnityEngine.RenderSettings.ambientLight = this._light.color;

      UnityEngine.RenderSettings.fogColor = this._fog_gradient.Evaluate(time : dot);
      UnityEngine.RenderSettings.fogDensity = this._fog_density_curve.Evaluate(time : dot) * this._fog_scale;

      var atmosphere_thickness = (this._day_atmosphere_thickness - this._night_atmosphere_thickness) * dot
                                 + this._night_atmosphere_thickness;
      this._sky_mat.SetFloat(nameID : _atmosphere_thickness, value : atmosphere_thickness);

      this.transform.Rotate(eulers : this._rotation * UnityEngine.Time.deltaTime * this._speed_multiplier);
    }

    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public override void Setup() {
      if (this._fog_density_curve == null) {
        this._fog_density_curve = droid.Runtime.Utilities.NeodroidDefaultsUtilities.DefaultAnimationCurve();
      }

      if (this._fog_gradient == null) {
        this._fog_gradient = droid.Runtime.Utilities.NeodroidDefaultsUtilities.DefaultGradient();
      }

      if (this._light_gradient == null) {
        this._light_gradient = droid.Runtime.Utilities.NeodroidDefaultsUtilities.DefaultGradient();
      }

      this._light = this.GetComponent<UnityEngine.Light>();
      this._sky_mat = UnityEngine.RenderSettings.skybox;
      this.transform.rotation = this._start_rotation;
      if (!this._particle_system) {
        this._particle_system = this.GetComponent<UnityEngine.ParticleSystem>();
      }

      this._particle_system.Pause();
      if (this._particles == null || this._particles.Length < this._particle_system.main.maxParticles) {
        this._particles = new UnityEngine.ParticleSystem.Particle[this._particle_system.main.maxParticles];
      }
    }

    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public override void PrototypingReset() { }
  }
}