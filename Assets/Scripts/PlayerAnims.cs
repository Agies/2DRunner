using UnityEngine;
using System.Collections;

public class PlayerAnims : MonoBehaviour {

    OTAnimatingSprite _sprite;
    GameMaster.Animation _currentAnim;

    // Use this for initialization
    void Start()
    {
        _sprite = GetComponent<OTAnimatingSprite>();
    }

    void Update()
    {
        // run left
        if (GameMaster.isLeft && !GameMaster.onRope && !GameMaster.onLadder && !GameMaster.falling && _currentAnim != GameMaster.Animation.WalkLeft)
        {
            _currentAnim = GameMaster.Animation.WalkLeft;
            _sprite.Play("runLeft");
        }
        if (!GameMaster.isLeft && !GameMaster.onRope && !GameMaster.falling && _currentAnim != GameMaster.Animation.StandLeft && GameMaster.facingDir == Facing.Left)
        {
            _currentAnim = GameMaster.Animation.StandLeft;
            _sprite.ShowFrame(13); // stand left
        }

        // run right
        if (GameMaster.isRight && !GameMaster.onRope && !GameMaster.onLadder && !GameMaster.falling && _currentAnim != GameMaster.Animation.WalkRight)
        {
            _currentAnim = GameMaster.Animation.WalkRight;
            _sprite.Play("runRight");
        }
        if (!GameMaster.isRight && !GameMaster.onRope && !GameMaster.falling && _currentAnim != GameMaster.Animation.StandRight && GameMaster.facingDir == Facing.Right)
        {
            _currentAnim = GameMaster.Animation.StandRight;
            _sprite.ShowFrame(16); // stand left
        }

        // climb
        if (GameMaster.isUp && GameMaster.onLadder && _currentAnim != GameMaster.Animation.Climb)
        {
            _currentAnim = GameMaster.Animation.Climb;
            _sprite.Play("climb");
        }
        if (!GameMaster.isUp && GameMaster.onLadder && _currentAnim != GameMaster.Animation.ClimbStop && GameMaster.facingDir == Facing.Up)
        {
            _currentAnim = GameMaster.Animation.ClimbStop;
            _sprite.ShowFrame(1); // climb left
        }

        if (GameMaster.isDown && GameMaster.onLadder && _currentAnim != GameMaster.Animation.Climb)
        {
            _currentAnim = GameMaster.Animation.Climb;
            _sprite.Play("climb");
        }
        if (!GameMaster.isDown && GameMaster.onLadder && _currentAnim != GameMaster.Animation.ClimbStop && GameMaster.facingDir == Facing.Down)
        {
            _currentAnim = GameMaster.Animation.ClimbStop;
            _sprite.ShowFrame(1); // climb left
        }

        // rope
        if (GameMaster.isLeft && GameMaster.onRope && _currentAnim != GameMaster.Animation.RopeLeft)
        {
            _currentAnim = GameMaster.Animation.RopeLeft;
            _sprite.Play("ropeLeft");
        }
        if (!GameMaster.isLeft && GameMaster.onRope && _currentAnim != GameMaster.Animation.HangLeft && GameMaster.facingDir == Facing.Left)
        {
            _currentAnim = GameMaster.Animation.HangLeft;
            _sprite.ShowFrame(6); // hang left
        }

        if (GameMaster.isRight && GameMaster.onRope && _currentAnim != GameMaster.Animation.RopeRight)
        {
            _currentAnim = GameMaster.Animation.RopeRight;
            _sprite.Play("ropeRight");
        }
        if (!GameMaster.isRight && GameMaster.onRope && _currentAnim != GameMaster.Animation.HangRight && GameMaster.facingDir == Facing.Right)
        {
            _currentAnim = GameMaster.Animation.HangRight;
            _sprite.ShowFrame(9); // hang right
        }

        // falling
        if (GameMaster.falling && _currentAnim != GameMaster.Animation.FallLeft && GameMaster.facingDir == Facing.Left )
        {
            _currentAnim = GameMaster.Animation.FallLeft;
            _sprite.ShowFrame(2); // fall left
        }
        if (GameMaster.falling && _currentAnim != GameMaster.Animation.FallRight && GameMaster.facingDir == Facing.Right)
        {
            _currentAnim = GameMaster.Animation.FallRight;
            _sprite.ShowFrame(3); // fall right
        }

        // shooting
        if (GameMaster.shooting && _currentAnim != GameMaster.Animation.ShootLeft && GameMaster.facingDir == Facing.Left)
        {
            _currentAnim = GameMaster.Animation.ShootLeft;
            _sprite.ShowFrame(10); // shoot left
        }
        if (GameMaster.shooting && _currentAnim != GameMaster.Animation.ShootRight && GameMaster.facingDir == Facing.Right)
        {
            _currentAnim = GameMaster.Animation.ShootRight;
            _sprite.ShowFrame(11); // shoot right
        }
    }
}
