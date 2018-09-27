/*using UnityEngine; using UnityEditor; using System.Collections;
using UnityEngine; using UnityEditor; using System.Collections.Generic;
using UnityEngine; using UnityEditor; using UnityEngine; using UnityEditor;

public class VRGrip : MonoBehaviour {

  SteamVR_TrackedController controller;
  FishSpawn fishSpawn;

  public GameObject slider;
  public GameObject grab;
  GameObject targetGO;
  public float slide_speed = 1.5f;
  bool move_slider = false;

	void Start () {
    controller = GetComponent<SteamVR_TrackedController>();
    fishSpawn = GetComponent<FishSpawn>();
	}
	
	void Update () {
    move_slider = controller.triggerPressed ? true : false;

    if (controller.padPressed) {
      ResetSlider();
      PlaceGrabPoint();
    }
    if (move_slider) {
      Grip();
    }

    if (controller.gripped) {
      fishSpawn.SpawnFish();
    }

  }

  void Grip() {
    Vector3 sliderPos = slider.transform.localPosition;
    if (sliderPos.x <= 7.95f) {
      sliderPos.x += slide_speed * Time.deltaTime;
      slider.transform.localPosition = sliderPos;
    }
  }

  void ResetSlider() {
    slider.transform.localPosition = new Vector3(1, 0, 0);
  }

  private void OnTriggerStay(Collider other) {
    targetGO = other.gameObject;
  }

  private void OnTriggerExit(Collider other) {
    targetGO = null;
  }

  void PlaceGrabPoint() {
    if (targetGO != null) {
      GameObject grabPoint = Instantiate(grab, grab.transform.position, grab.transform.rotation, targetGO.transform);
      grabPoint.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
    }
  }
}
*/


