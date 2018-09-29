using UnityEngine;

namespace Common.EffectExamples.Shared.Scripts {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  public class GunShoot : MonoBehaviour {
    Animator _anim;
    public ParticleSystem _CartridgeEjection;
    public float _FireRate = 0.25f; // Number in seconds which controls how often the player can fire
    public GameObject[] _FleshHitEffects;
    GunAim _gun_aim;

    public Transform _GunEnd;

    public GameObject _MetalHitEffect;
    public ParticleSystem _MuzzleFlash;

    float _next_fire; // Float to store the time the player will be allowed to fire again, after firing
    public GameObject _SandHitEffect;
    public GameObject _StoneHitEffect;
    public GameObject _WaterLeakEffect;
    public GameObject _WaterLeakExtinguishEffect;
    public float _WeaponRange = 20f; // Distance in Unity units over which the player can fire
    public GameObject _WoodHitEffect;

    void Start() {
      this._anim = this.GetComponent<Animator>();
      this._gun_aim = this.GetComponentInParent<GunAim>();
    }

    void Update() {
      if (Input.GetButtonDown("Fire1") && Time.time > this._next_fire && !this._gun_aim.GetIsOutOfBounds()) {
        this._next_fire = Time.time + this._FireRate;
        this._MuzzleFlash.Play();
        this._CartridgeEjection.Play();
        this._anim.SetTrigger("Fire");

        var ray_origin = this._GunEnd.position;
        RaycastHit hit;
        if (Physics.Raycast(ray_origin, this._GunEnd.forward, out hit, this._WeaponRange)) {
          this.HandleHit(hit);
        }
      }
    }

    void HandleHit(RaycastHit hit) {
      if (hit.collider.sharedMaterial != null) {
        var material_name = hit.collider.sharedMaterial.name;

        switch (material_name) {
          case "Metal":
            this.SpawnDecal(hit, this._MetalHitEffect);
            break;
          case "Sand":
            this.SpawnDecal(hit, this._SandHitEffect);
            break;
          case "Stone":
            this.SpawnDecal(hit, this._StoneHitEffect);
            break;
          case "WaterFilled":
            this.SpawnDecal(hit, this._WaterLeakEffect);
            this.SpawnDecal(hit, this._MetalHitEffect);
            break;
          case "Wood":
            this.SpawnDecal(hit, this._WoodHitEffect);
            break;
          case "Meat":
            this.SpawnDecal(hit, this._FleshHitEffects[Random.Range(0, this._FleshHitEffects.Length)]);
            break;
          case "Character":
            this.SpawnDecal(hit, this._FleshHitEffects[Random.Range(0, this._FleshHitEffects.Length)]);
            break;
          case "WaterFilledExtinguish":
            this.SpawnDecal(hit, this._WaterLeakExtinguishEffect);
            this.SpawnDecal(hit, this._MetalHitEffect);
            break;
        }
      }
    }

    void SpawnDecal(RaycastHit hit, GameObject prefab) {
      var spawned_decal = Instantiate(prefab, hit.point, Quaternion.LookRotation(hit.normal));
      spawned_decal.transform.SetParent(hit.collider.transform);
    }
  }
}
