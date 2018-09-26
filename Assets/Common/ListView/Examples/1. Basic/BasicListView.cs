using Common.ListView.Scripts;
using UnityEngine;

namespace Common.ListView.Examples._1._Basic {
  public class BasicListView : ListViewController {
    public float _ScrollSpeed = 10f;
    public GUISkin _Skin;

    void OnGUI() {
      GUI.skin = this._Skin;
      GUILayout.BeginArea(new Rect(10, 10, 300, 300));
      GUILayout.Label(
          "This is a basic List View. We are only extending the class in order to add the GUI.  Use the buttons below to scroll the m_List, or feel free to modify the value of Scroll Offset in the inspector");
      if (GUILayout.Button("Scroll Next")) {
        this.ScrollNext();
      }

      if (GUILayout.Button("Scroll Prev")) {
        this.ScrollPrev();
      }

      if (GUILayout.RepeatButton("Smooth Scroll Next")) {
        this._ScrollOffset -= this._ScrollSpeed * Time.deltaTime;
      }

      if (GUILayout.RepeatButton("Smooth Scroll Prev")) {
        this._ScrollOffset += this._ScrollSpeed * Time.deltaTime;
      }

      GUILayout.EndArea();
    }
  }
}
