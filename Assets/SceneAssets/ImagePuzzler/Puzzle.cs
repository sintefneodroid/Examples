﻿namespace SceneAssets.ImagePuzzler {
  public class Puzzle : UnityEngine.MonoBehaviour {
    [UnityEngine.SerializeField] float _default_move_duration = .2f;
    [UnityEngine.SerializeField] Block _empty_block;
    [UnityEngine.SerializeField] int _horisontal_divisions = 6;

    [UnityEngine.SerializeField] UnityEngine.Texture2D _image = null;
    [UnityEngine.SerializeField] int _shuffle_length = 20;
    [UnityEngine.SerializeField] float _shuffle_move_duration = .1f;
    [UnityEngine.SerializeField] int _vertical_divisions = 6;
    bool _block_is_moving = false;
    Block[,] _blocks;
    System.Collections.Generic.Queue<Block> _inputs;
    UnityEngine.Vector2Int _prev_shuffle_offset;
    int _shuffle_moves_remaining;

    PuzzleState _state;

    void Start() { this.CreatePuzzle(); }

    void Update() {
      if (this._state == PuzzleState.Solved_
          && UnityEngine.Input.GetKeyDown(key : UnityEngine.KeyCode.Space)) {
        this.StartShuffle();
      }
    }

    void CreatePuzzle() {
      this._blocks = new Block[this._horisontal_divisions, this._vertical_divisions];
      var image_slices = new UnityEngine.Texture2D[this._horisontal_divisions, this._vertical_divisions];
      if (this._image) {
        image_slices = ImageSlicer.GetSlices(image : this._image,
                                             horisontal_divisions : this._horisontal_divisions,
                                             vertical_divisions : this._vertical_divisions);
      }

      var dominant_division =
          UnityEngine.Mathf.Max(a : this._vertical_divisions, b : this._horisontal_divisions);
      //var lesser_division = Mathf.Min (this._vertical_divisions, this._horisontal_divisions);

      for (var y = 0; y < this._vertical_divisions; y++) {
        for (var x = 0; x < this._horisontal_divisions; x++) {
          var block_object = UnityEngine.GameObject.CreatePrimitive(type : UnityEngine.PrimitiveType.Quad);
          block_object.transform.position = -UnityEngine.Vector2.one * (dominant_division - 1) * .5f
                                            + new UnityEngine.Vector2(x : x, y : y);
          block_object.transform.parent = this.transform;

          var block = block_object.AddComponent<Block>();
          block.OnBlockPressed += this.PlayerMoveBlockInput;
          block.OnFinishedMoving += this.OnBlockFinishedMoving;
          block.Init(starting_coord : new UnityEngine.Vector2Int(x : x, y : y), image : image_slices[x, y]);
          this._blocks[x, y] = block;

          if (y == 0 && x == dominant_division - 1) {
            this._empty_block = block;
          }
        }
      }

      UnityEngine.Camera.main.orthographicSize = dominant_division * .55f;
      this._inputs = new System.Collections.Generic.Queue<Block>();
    }

    void PlayerMoveBlockInput(Block block_to_move) {
      if (this._state == PuzzleState.In_play_) {
        this._inputs.Enqueue(item : block_to_move);
        this.MakeNextPlayerMove();
      }
    }

    void MakeNextPlayerMove() {
      while (this._inputs.Count > 0 && !this._block_is_moving) {
        this.MoveBlock(block_to_move : this._inputs.Dequeue(), duration : this._default_move_duration);
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
        block_to_move.MoveToPosition(target : target_position, duration : duration);
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
      UnityEngine.Vector2Int[] offsets = {
                                             new UnityEngine.Vector2Int(1, 0),
                                             new UnityEngine.Vector2Int(-1, 0),
                                             new UnityEngine.Vector2Int(0, 1),
                                             new UnityEngine.Vector2Int(0, -1)
                                         };
      var random_index = UnityEngine.Random.Range(0, maxExclusive : offsets.Length);

      for (var i = 0; i < offsets.Length; i++) {
        var offset = offsets[(random_index + i) % offsets.Length];
        if (offset != this._prev_shuffle_offset * -1) {
          var move_block_coord = this._empty_block.Coord + offset;

          if (move_block_coord.x >= 0
              && move_block_coord.x < this._horisontal_divisions
              && move_block_coord.y >= 0
              && move_block_coord.y < this._vertical_divisions) {
            this.MoveBlock(block_to_move : this._blocks[move_block_coord.x, move_block_coord.y],
                           duration : this._shuffle_move_duration);
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