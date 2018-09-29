using UnityEngine;
using UnityEngine.UI;

namespace Common.EffectExamples.Shared.Scripts {
  /// <summary>
  /// </summary>
  public class ParticleMenu : MonoBehaviour {
    // the currently shown prefab game object
    GameObject _current_go;

    // a private integer to store the current position in the array
    int _current_index;
    public Text _Description;

    // the gun GameObject
    public GameObject _GunGameObject;

    public Text _NavigationDetails;

    // our ParticleExamples class being turned into an array of things that can be referenced
    public ParticleExamples[] _ParticleSystems;

    // where to spawn prefabs 
    public Transform _SpawnLocation;

    // references to the UI Text components
    public Text _Title;

    // setting up the first menu item and resetting the currentIndex to ensure it's at zero
    void Start() {
      this.Navigate(0);
      this._current_index = 0;
    }

    // our public function that gets called by our menu's buttons
    public void Navigate(int i) {
      // set the current position in the array to the next or previous position depending on whether i is -1 or 1, defined in our button event
      this._current_index = (this._ParticleSystems.Length + this._current_index + i)
                            % this._ParticleSystems.Length;

      // check if there is a currentGO, if there is (if its not null), then destroy it to make space for the new one..
      if (this._current_go != null) {
        Destroy(this._current_go);
      }

      // ..spawn the relevant game object based on the array of potential game objects, according to the current index (position in the array)
      this._current_go = Instantiate(
          this._ParticleSystems[this._current_index]._ParticleSystemGo,
          this._SpawnLocation.position + this._ParticleSystems[this._current_index]._ParticlePosition,
          Quaternion.Euler(this._ParticleSystems[this._current_index]._ParticleRotation));

      // only activate the gun GameObject if the current effect is a weapon effect
      this._GunGameObject.SetActive(this._ParticleSystems[this._current_index]._IsWeaponEffect);

      // setup the UI texts according to the strings in the array 
      this._Title.text = this._ParticleSystems[this._current_index]._Title;
      this._Description.text = this._ParticleSystems[this._current_index]._Description;
      this._NavigationDetails.text =
          "" + (this._current_index + 1) + " out of " + this._ParticleSystems.Length;
    }
  }
}
