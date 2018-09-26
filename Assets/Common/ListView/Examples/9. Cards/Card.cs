using System;
using Common.ListView.Scripts;
using UnityEngine;

namespace Common.ListView.Examples._9._Cards {
  public class Card : ListViewItem<CardData> {
    public enum Suit {
      Diamonds_,
      Hearts_,
      Spades_,
      Clubs_
    }

    public TextMesh _TopNum, _BotNum;
    public float _CenterScale = 3f;

    [SerializeField] GameObject _m_diamond;
    [SerializeField] GameObject _m_heart;
    [SerializeField] GameObject _m_spade;
    [SerializeField] GameObject _m_club;

    Vector3 _m_size;
    const float _k_quad_offset = 0.001f; //Local y offset for placing quads

    public override void Setup(CardData data) {
      base.Setup(data);
      this._TopNum.text = data._Value;
      this._BotNum.text = data._Value;

      this._m_size = this.GetComponent<BoxCollider>().size;

      this.SetupCard();
    }

    void DestroyChildren(Transform trans) {
      foreach (Transform child in trans) {
        if (child.gameObject != this._BotNum.gameObject && child.gameObject != this._TopNum.gameObject) {
          Destroy(child.gameObject);
        }
      }
    }

    void SetupCard() {
      this.DestroyChildren(this.transform);
      var prefab = this._m_heart;
      var color = Color.red;
      switch (this._Data._Suit) {
        case Suit.Clubs_:
          color = Color.black;
          prefab = this._m_club;
          break;
        case Suit.Diamonds_:
          prefab = this._m_diamond;
          break;
        case Suit.Spades_:
          color = Color.black;
          prefab = this._m_spade;
          break;
      }

      switch (this._Data._Value) {
        case "J":
        case "K":
        case "Q":
        case "A": {
          var quad = this.AddQuad(prefab);
          quad.transform.localScale *= this._CenterScale;
          quad.transform.localRotation = Quaternion.AngleAxis(90, Vector3.right);
          break;
        }
        default: {
          var val_num = Convert.ToInt32(this._Data._Value);
          float division_y = 0;
          float division_x = 0;
          var cols = (val_num < 4 ? 1 : 2);
          var rows = val_num / cols;
          if (val_num == 8) {
            rows = 3;
          }

          division_y = 1f / (rows + 1);
          division_x = 1f / (cols + 1);
          for (var j = 0; j < cols; j++) {
            for (var i = 0; i < rows; i++) {
              var quad = this.AddQuad(prefab);
              quad.transform.localPosition +=
                  Vector3.forward * this._m_size.z * (division_y * (i + 1) - 0.5f);
              quad.transform.localPosition += Vector3.right * this._m_size.x * (division_x * (j + 1) - 0.5f);
              quad.transform.localRotation = Quaternion.AngleAxis(90, Vector3.right);
            }
          }

          var leftover = 0;
          switch (val_num) {
            case 5:
              division_y = 0f;
              leftover = 1;
              break;
            case 7:
              division_y = 0.125f;
              leftover = 1;
              break;
            case 8:
              division_y = 0.125f;
              leftover = 3;
              break;
            case 9:
              division_y = 0.2f;
              leftover = 1;
              break;
            case 10:
              division_y = 0.25f;
              leftover = 3;
              break;
          }

          for (var i = 0; i < leftover; i += 2) {
            var quad = this.AddQuad(prefab);
            quad.transform.localPosition -= Vector3.forward * this._m_size.z * (division_y * (i - 1));
            quad.transform.localRotation = Quaternion.AngleAxis(90, Vector3.right);
          }

          break;
        }
      }

      this._TopNum.text = this._Data._Value;
      this._TopNum.color = color;
      this._BotNum.text = this._Data._Value;
      this._BotNum.color = color;
    }

    GameObject AddQuad(GameObject prefab) {
      //NOTE: If we were really concerned about performance, we could pool the quads
      var quad = Instantiate(prefab);
      quad.transform.parent = this.transform;
      quad.transform.localPosition = Vector3.up * _k_quad_offset;
      return quad;
    }
  }

  //[System.Serializable]     //Will cause warnings, but helpful for debugging
  public class CardData : ListViewItemData {
    //Ace is 1, King is 13
    public string _Value;
    public Card.Suit _Suit;
  }
}
