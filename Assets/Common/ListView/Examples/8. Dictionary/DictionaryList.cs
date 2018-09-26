#if UNITY_EDITOR
/*using System;
using System.Data;
using System.IO;
using System.Threading;
using Excluded.ListView.Scripts;
using Mono.Data.Sqlite;
using UnityEngine;

//Borrows from http://answers.unity3d.com/questions/743400/database-sqlite-setup-for-unity.html
//Dictionary from https://wordnet.princeton.edu/

namespace Excluded.ListView.Examples._8._Dictionary {
  public class DictionaryList : ListViewController<DictionaryListItemData, DictionaryListItem> {
    public const string _EditorDatabasePath = "ListView/Examples/8. Dictionary/wordnet30.db";
    public const string _DatabasePath = "wordnet30.db";
    public int _BatchSize = 15;
    public float _ScrollDamping = 15f;
    public float _MaxMomentum = 200f;
    public string _DefaultTemplate = "DictionaryItem";
    public GameObject _LoadingIndicator;

    public int _MaxWordCharacters = 30; //Wrap word after 30 characters
    public int _DefinitionCharacterWrap = 40; //Wrap definition after 40 characters
    public int _MaxDefinitionLines = 4; //Max 4 lines per definition

    delegate void WordsResult(DictionaryListItemData[] words);

    volatile bool _m_db_lock;

    DictionaryListItemData[] _m_cleanup;
    int _m_data_length; //Total number of items in the data set
    int _m_batch_offset; //Number of batches we are offset
    bool _m_scrolling;
    bool _m_loading;
    float _m_scroll_return = float.MaxValue;
    float _m_scroll_delta;
    float _m_last_scroll_offset;

    IDbConnection _m_db_connection;

    protected override void Setup() {
      base.Setup();

      #if UNITY_EDITOR
      var conn = "URI=file:" + Path.Combine(Application.dataPath, _EditorDatabasePath);
      #else
            string conn = "URI=file:" + Path.Combine(Application.dataPath, databasePath);
#endif

      this._m_db_connection = new SqliteConnection(conn);
      this._m_db_connection.Open(); //Open connection to the database.

      if (this._MaxWordCharacters < 4) {
        Debug.LogError("Max word length must be > 3");
      }

      try {
        var dbcmd = this._m_db_connection.CreateCommand();
        var sql_query =
            "SELECT COUNT(lemma) FROM word as W JOIN sense as S on W.wordid=S.wordid JOIN synset as Y on S.synsetid=Y.synsetid";
        dbcmd.CommandText = sql_query;
        var reader = dbcmd.ExecuteReader();
        while (reader.Read()) {
          this._m_data_length = reader.GetInt32(0);
        }

        reader.Close();
        dbcmd.Dispose();
      } catch {
        Debug.LogError("DB error, couldn't get total data length");
      }

      this._Data = null;
      //Start off with some data
      this.GetWords(
          0,
          this._BatchSize * 3,
          words => {
            this._Data = words;
          });
    }

    void OnDestroy() {
      this._m_db_connection.Close();
      this._m_db_connection = null;
    }

    void GetWords(int offset, int range, WordsResult result) {
      if (this._m_db_lock) {
        return;
      }

      if (result == null) {
        Debug.LogError("Called GetWords without a result callback");
        return;
      }

      this._m_db_lock = true;
      //Not sure what the current deal is with threads. Hopefully this is OK?
      new Thread(
          () => {
            try {
              var words = new DictionaryListItemData[range];
              var dbcmd = this._m_db_connection.CreateCommand();
              var sql_query =
                  $"SELECT lemma, definition FROM word as W JOIN sense as S on W.wordid=S.wordid JOIN synset as Y on S.synsetid=Y.synsetid ORDER BY W.wordid limit {range} OFFSET {offset}";
              dbcmd.CommandText = sql_query;
              var reader = dbcmd.ExecuteReader();
              var count = 0;
              while (reader.Read()) {
                var lemma = reader.GetString(0);
                var definition = reader.GetString(1);
                words[count] = new DictionaryListItemData {_Template = this._DefaultTemplate};

                //truncate word if necessary
                if (lemma.Length > this._MaxWordCharacters) {
                  lemma = lemma.Substring(0, this._MaxWordCharacters - 3) + "...";
                }

                words[count]._Word = lemma;

                //Wrap definition
                var wrds = definition.Split(' ');
                var char_count = 0;
                var line_count = 0;
                foreach (var wrd in wrds) {
                  char_count += wrd.Length + 1;
                  if (char_count > this._DefinitionCharacterWrap) { //Guesstimate
                    if (++line_count >= this._MaxDefinitionLines) {
                      words[count]._Definition += "...";
                      break;
                    }

                    words[count]._Definition += "\n";
                    char_count = 0;
                  }

                  words[count]._Definition += wrd + " ";
                }

                count++;
              }

              if (count < this._BatchSize) {
                Debug.LogWarning("reached end");
              }

              reader.Close();
              dbcmd.Dispose();
              result(words);
            } catch (Exception e) {
              Debug.LogError("Exception reading from DB: " + e.Message);
            }

            this._m_db_lock = false;
            this._m_loading = false;
          }).Start();
    }

    protected override void ComputeConditions() {
      if (this._Templates.Length > 0) {
        //Use first template to get item size
        this._M_ItemSize = this.GetObjectSize(this._Templates[0]);
      }

      //Resize range to nearest multiple of item width
      this._M_NumItems = Mathf.RoundToInt(this._Range / this._M_ItemSize.y); //Number of cards that will fit
      this._Range = this._M_NumItems * this._M_ItemSize.y;

      //Get initial conditions. This procedure is done every frame in case the collider bounds change at runtime
      this._M_LeftSide = this.transform.position
                         + Vector3.up * this._Range * 0.5f
                         + Vector3.left * this._M_ItemSize.x * 0.5f;

      this._M_DataOffset = (int)(this._ScrollOffset / this.ItemSize.y);
      if (this._ScrollOffset < 0) {
        this._M_DataOffset--;
      }

      var curr_batch = -this._M_DataOffset / this._BatchSize;
      if (-this._M_DataOffset > (this._m_batch_offset + 2) * this._BatchSize) {
        //Check how many batches we jumped
        if (curr_batch == this._m_batch_offset + 2) { //Just one batch, fetch only the next one
          this.GetWords(
              (this._m_batch_offset + 3) * this._BatchSize,
              this._BatchSize,
              words => {
                Array.Copy(this._Data, this._BatchSize, this._Data, 0, this._BatchSize * 2);
                Array.Copy(words, 0, this._Data, this._BatchSize * 2, this._BatchSize);
                this._m_batch_offset++;
              });
        } else if (curr_batch != this._m_batch_offset) { //Jumped multiple batches. Get a whole new dataset
          if (!this._m_loading) {
            this._m_cleanup = this._Data;
          }

          this._m_loading = true;
          this.GetWords(
              (curr_batch - 1) * this._BatchSize,
              this._BatchSize * 3,
              words => {
                this._Data = words;
                this._m_batch_offset = curr_batch - 1;
              });
        }
      } else if (this._m_batch_offset > 0
                 && -this._M_DataOffset < (this._m_batch_offset + 1) * this._BatchSize) {
        if (curr_batch == this._m_batch_offset) { //Just one batch, fetch only the next one
          this.GetWords(
              (this._m_batch_offset - 1) * this._BatchSize,
              this._BatchSize,
              words => {
                Array.Copy(this._Data, 0, this._Data, this._BatchSize, this._BatchSize * 2);
                Array.Copy(words, 0, this._Data, 0, this._BatchSize);
                this._m_batch_offset--;
              });
        } else if (curr_batch != this._m_batch_offset) { //Jumped multiple batches. Get a whole new dataset
          if (!this._m_loading) {
            this._m_cleanup = this._Data;
          }

          this._m_loading = true;
          if (curr_batch < 1) {
            curr_batch = 1;
          }

          this.GetWords(
              (curr_batch - 1) * this._BatchSize,
              this._BatchSize * 3,
              words => {
                this._Data = words;
                this._m_batch_offset = curr_batch - 1;
              });
        }
      }

      if (this._m_cleanup != null) {
        //Clean up all existing game_objects
        foreach (var item in this._m_cleanup) {
          if (item._Item != null) {
            this.RecycleItem(item._Template, item._Item);
            item._Item = null;
          }
        }

        this._m_cleanup = null;
      }

      if (this._m_scrolling) {
        this._m_scroll_delta = (this._ScrollOffset - this._m_last_scroll_offset) / Time.deltaTime;
        this._m_last_scroll_offset = this._ScrollOffset;
        if (this._m_scroll_delta > this._MaxMomentum) {
          this._m_scroll_delta = this._MaxMomentum;
        }

        if (this._m_scroll_delta < -this._MaxMomentum) {
          this._m_scroll_delta = -this._MaxMomentum;
        }
      } else {
        this._ScrollOffset += this._m_scroll_delta * Time.deltaTime;
        if (this._m_scroll_delta > 0) {
          this._m_scroll_delta -= this._ScrollDamping * Time.deltaTime;
          if (this._m_scroll_delta < 0) {
            this._m_scroll_delta = 0;
          }
        } else if (this._m_scroll_delta < 0) {
          this._m_scroll_delta += this._ScrollDamping * Time.deltaTime;
          if (this._m_scroll_delta > 0) {
            this._m_scroll_delta = 0;
          }
        }
      }

      if (this._M_DataOffset >= this._m_data_length) {
        this._m_scroll_return = this._ScrollOffset;
      }
    }

    public void OnStartScrolling() { this._m_scrolling = true; }

    public void OnStopScrolling() {
      this._m_scrolling = false;
      if (this._ScrollOffset > 0) {
        this._ScrollOffset = 0;
        this._m_scroll_delta = 0;
      }

      if (this._m_scroll_return < float.MaxValue) {
        this._ScrollOffset = this._m_scroll_return;
        this._m_scroll_return = float.MaxValue;
        this._m_scroll_delta = 0;
      }
    }

    protected override void UpdateItems() {
      if (this._Data == null || this._Data.Length == 0 || this._m_loading) {
        this._LoadingIndicator.SetActive(true);
        return;
      }

      for (var i = 0; i < this._Data.Length; i++) {
        if (i + this._M_DataOffset + this._m_batch_offset * this._BatchSize < -1) {
          //Checking against -1 lets the first element overflow
          this.ExtremeLeft(this._Data[i]);
        } else if (i + this._M_DataOffset + this._m_batch_offset * this._BatchSize > this._M_NumItems) {
          this.ExtremeRight(this._Data[i]);
        } else {
          this.ListMiddle(this._Data[i], i + this._m_batch_offset * this._BatchSize);
        }
      }

      this._LoadingIndicator.SetActive(false);
    }

    protected override void Positioning(Transform t, int offset) {
      t.position = this._M_LeftSide + (offset * this._M_ItemSize.y + this._ScrollOffset) * Vector3.down;
    }
  }
}*/
#endif
