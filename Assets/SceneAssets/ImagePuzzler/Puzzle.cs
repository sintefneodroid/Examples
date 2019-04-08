using System.Collections.Generic;
using UnityEngine;

namespace SceneAssets.ImagePuzzler {
  public class Puzzle : MonoBehaviour {
    bool _block_is_moving=false;
    Block[,] _blocks;
    [SerializeField] float _default_move_duration = .2f;
    [SerializeField] Block _empty_block;
    [SerializeField] int _horisontal_divisions = 6;

    [SerializeField] Texture2D _image=null;
    Queue<Block> _inputs;
    Vector2Int _prev_shuffle_offset;
    [SerializeField] int _shuffle_length = 20;
    [SerializeField] float _shuffle_move_duration = .1f;
    int _shuffle_moves_remaining;

    PuzzleState _state;
    [SerializeField] int _vertical_divisions = 6;

    void Start() { this.CreatePuzzle(); }

    void Update() {
      if (this._state == PuzzleState.Solved_ && Input.GetKeyDown(KeyCode.Space)) {
        this.StartShuffle();
      }
    }

    void CreatePuzzle() {
      this._blocks = new Block[this._horisontal_divisions, this._vertical_divisions];
      var image_slices = new Texture2D[this._horisontal_divisions, this._vertical_divisions];
      if (this._image) {
        image_slices = ImageSlicer.GetSlices(
            this._image,
            this._horisontal_divisions,
            this._vertical_divisions);
      }

      var dominant_division = Mathf.Max(this._vertical_divisions, this._horisontal_divisions);
      //var lesser_division = Mathf.Min (this._vertical_divisions, this._horisontal_divisions);

      for (var y = 0; y < this._vertical_divisions; y++) {
        for (var x = 0; x < this._horisontal_divisions; x++) {
          var block_object = GameObject.CreatePrimitive(PrimitiveType.Quad);
          block_object.transform.position = -Vector2.one * (dominant_division - 1) * .5f + new Vector2(x, y);
          block_object.transform.parent = this.transform;

          var block = block_object.AddComponent<Block>();
          block.OnBlockPressed += this.PlayerMoveBlockInput;
          block.OnFinishedMoving += this.OnBlockFinishedMoving;
          block.Init(new Vector2Int(x, y), image_slices[x, y]);
          this._blocks[x, y] = block;

          if (y == 0 && x == dominant_division - 1) {
            this._empty_block = block;
          }
        }
      }

      Camera.main.orthographicSize = dominant_division * .55f;
      this._inputs = new Queue<Block>();
    }

    void PlayerMoveBlockInput(Block block_to_move) {
      if (this._state == PuzzleState.In_play_) {
        this._inputs.Enqueue(block_to_move);
        this.MakeNextPlayerMove();
      }
    }

    void MakeNextPlayerMove() {
      while (this._inputs.Count > 0 && !this._block_is_moving) {
        this.MoveBlock(this._inputs.Dequeue(), this._default_move_duration);
      }
    }

    void MoveBlock(Block block_to_move, float duration) {
      if ((block_to_move.Coord - this._empty_block.Coord).sqrMagnitude == 1) {
        this._blocks[block_to_move.Coord.x, block_to_move.Coord.y] = this._empty_block;
        this._blocks[this._empty_block.Coord.x, this._empty_block.Coord.y] = block_to_move;

        var target_coord = this._empty_block.Coord;
        this._empty_block.Coord = block_to_move.Coord;
        block_to_move.Coord = target_coord;

        var target_position = this._empty_block.transform.position;
        this._empty_block.transform.position = block_to_move.transform.position;
        block_to_move.MoveToPosition(target_position, duration);
        this._block_is_moving = true;
      }
    }

    void OnBlockFinishedMoving() {
      this._block_is_moving = false;
      this.CheckIfSolved();

      if (this._state == PuzzleState.In_play_) {
        this.MakeNextPlayerMove();
      } else if (this._state == PuzzleState.Shuffling_) {
        if (this._shuffle_moves_remaining > 0) {
          this.MakeNextShuffleMove();
        } else {
          this._state = PuzzleState.In_play_;
        }
      }
    }

    void StartShuffle() {
      this._state = PuzzleState.Shuffling_;
      this._shuffle_moves_remaining = this._shuffle_length;
      this._empty_block.gameObject.SetActive(false);
      this.MakeNextShuffleMove();
    }

    void MakeNextShuffleMove() {
      Vector2Int[] offsets = {
          new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1)
      };
      var random_index = Random.Range(0, offsets.Length);

      for (var i = 0; i < offsets.Length; i++) {
        var offset = offsets[(random_index + i) % offsets.Length];
        if (offset != this._prev_shuffle_offset * -1) {
          var move_block_coord = this._empty_block.Coord + offset;

          if (move_block_coord.x >= 0
              && move_block_coord.x < this._horisontal_divisions
              && move_block_coord.y >= 0
              && move_block_coord.y < this._vertical_divisions) {
            this.MoveBlock(this._blocks[move_block_coord.x, move_block_coord.y], this._shuffle_move_duration);
            this._shuffle_moves_remaining--;
            this._prev_shuffle_offset = offset;
            break;
          }
        }
      }
    }

    void CheckIfSolved() {
      foreach (var block in this._blocks) {
        if (!block.IsAtStartingCoord()) {
          return;
        }
      }

      this._state = PuzzleState.Solved_;
      this._empty_block.gameObject.SetActive(true);
    }

    enum PuzzleState {
      Solved_,
      Shuffling_,
      In_play_
    }
  }
}