using UnityEngine;
using System.Collections;

public class PlayerAnims : MonoBehaviour
{
    public enum Animation
    {
        None,
        WalkLeft,
        WalkRight,
        RopeLeft,
        RopeRight,
        Climb,
        ClimbStop,
        StandLeft,
        StandRight,
        HangLeft,
        HangRight,
        FallLeft,
        FallRight,
        ShootLeft,
        ShootRight
    }

    private OTAnimatingSprite _sprite;
    private Animation _currentAnim;
    private Character _character;
    
    void Start()
    {
        _sprite = GetComponent<OTAnimatingSprite>();
        _character = GetComponent<Character>();
    }

    void Update()
    {
        // run left
        if (_character.isLeft && !_character.onRope && !_character.onLadder && !_character.falling && _currentAnim != Animation.WalkLeft)
        {
            _currentAnim = Animation.WalkLeft;
            _sprite.Play("runLeft");
        }
        if (!_character.isLeft && !_character.onRope && !_character.falling && _currentAnim != Animation.StandLeft && _character.facingDir == Facing.Left)
        {
            _currentAnim = Animation.StandLeft;
            _sprite.ShowFrame(13); // stand left
        }

        // run right
        if (_character.isRight && !_character.onRope && !_character.onLadder && !_character.falling && _currentAnim != Animation.WalkRight)
        {
            _currentAnim = Animation.WalkRight;
            _sprite.Play("runRight");
        }
        if (!_character.isRight && !_character.onRope && !_character.falling && _currentAnim != Animation.StandRight && _character.facingDir == Facing.Right)
        {
            _currentAnim = Animation.StandRight;
            _sprite.ShowFrame(16); // stand left
        }

        // climb
        if (_character.isUp && _character.onLadder && _currentAnim != Animation.Climb)
        {
            _currentAnim = Animation.Climb;
            _sprite.Play("climb");
        }
        if (!_character.isUp && _character.onLadder && _currentAnim != Animation.ClimbStop && _character.facingDir == Facing.Up)
        {
            _currentAnim = Animation.ClimbStop;
            _sprite.ShowFrame(1); // climb left
        }

        if (_character.isDown && _character.onLadder && _currentAnim != Animation.Climb)
        {
            _currentAnim = Animation.Climb;
            _sprite.Play("climb");
        }
        if (!_character.isDown && _character.onLadder && _currentAnim != Animation.ClimbStop && _character.facingDir == Facing.Down)
        {
            _currentAnim = Animation.ClimbStop;
            _sprite.ShowFrame(1); // climb left
        }

        // rope
        if (_character.isLeft && _character.onRope && _currentAnim != Animation.RopeLeft)
        {
            _currentAnim = Animation.RopeLeft;
            _sprite.Play("ropeLeft");
        }
        if (!_character.isLeft && _character.onRope && _currentAnim != Animation.HangLeft && _character.facingDir == Facing.Left)
        {
            _currentAnim = Animation.HangLeft;
            _sprite.ShowFrame(6); // hang left
        }

        if (_character.isRight && _character.onRope && _currentAnim != Animation.RopeRight)
        {
            _currentAnim = Animation.RopeRight;
            _sprite.Play("ropeRight");
        }
        if (!_character.isRight && _character.onRope && _currentAnim != Animation.HangRight && _character.facingDir == Facing.Right)
        {
            _currentAnim = Animation.HangRight;
            _sprite.ShowFrame(9); // hang right
        }

        // falling
        if (_character.falling && _currentAnim != Animation.FallLeft && _character.facingDir == Facing.Left)
        {
            _currentAnim = Animation.FallLeft;
            _sprite.ShowFrame(2); // fall left
        }
        if (_character.falling && _currentAnim != Animation.FallRight && _character.facingDir == Facing.Right)
        {
            _currentAnim = Animation.FallRight;
            _sprite.ShowFrame(3); // fall right
        }

        // shooting
        if (_character.shooting && _currentAnim != Animation.ShootLeft && _character.facingDir == Facing.Left)
        {
            _currentAnim = Animation.ShootLeft;
            _sprite.ShowFrame(10); // shoot left
        }
        if (_character.shooting && _currentAnim != Animation.ShootRight && _character.facingDir == Facing.Right)
        {
            _currentAnim = Animation.ShootRight;
            _sprite.ShowFrame(11); // shoot right
        }
    }
}
