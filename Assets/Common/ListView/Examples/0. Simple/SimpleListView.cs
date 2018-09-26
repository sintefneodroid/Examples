using UnityEngine;

namespace Common.ListView.Examples._0._Simple {
  public class SimpleListView : MonoBehaviour {
    public GameObject _Prefab;
    public int _DataOffset;

    public float _ItemHeight = 1;
    public int _Range = 5;

    public string[] _Data;
    public GUISkin _Skin;

    TextMesh[] _m_items;

    void Start() {
      this._m_items = new TextMesh[this._Range];
      for (var i = 0; i < this._Range; i++) {
        this._m_items[i] = Instantiate(this._Prefab).GetComponent<TextMesh>();
        this._m_items[i].transform.position = this.transform.position + Vector3.down * i * this._ItemHeight;
        this._m_items[i].transform.parent = this.transform;
      }

      this.UpdateList();
    }

    void UpdateList() {
      for (var i = 0; i < this._Range; i++) {
        var data_idx = i + this._DataOffset;
        if (data_idx >= 0 && data_idx < this._Data.Length) {
          this._m_items[i].text = this._Data[data_idx];
        } else {
          this._m_items[i].text = "";
        }
      }
    }

    void OnGUI() {
      GUI.skin = this._Skin;
      GUILayout.BeginArea(new Rect(10, 10, 300, 300));
      GUILayout.Label(
          "This is an overly simplistic m_List view. Click the buttons below to scroll, or modify Data Offset in the inspector");
      if (GUILayout.Button("Scroll Next")) {
        this.ScrollNext();
      }

      if (GUILayout.Button("Scroll Prev")) {
        this.ScrollPrev();
      }

      GUILayout.EndArea();
    }

    void ScrollNext() {
      this._DataOffset++;
      this.UpdateList();
    }

    void ScrollPrev() {
      this._DataOffset--;
      this.UpdateList();
    }
  }
}
