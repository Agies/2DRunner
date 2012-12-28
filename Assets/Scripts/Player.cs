using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    private Transform _shootParent;
    private Renderer _shootRenderer;
    private OTAnimatingSprite _shootSprite;

    private float _moveSpeed = 5;
    private int _moveDirX;
    private int _moveDirY;
    private Vector3 _movement;
    private Transform _transform;

    private float _rayBlockedDistX = 0.6f;
    private RaycastHit _hit;

    public LayerMask groundMask; // Layer == Ground
    public LayerMask shootMask; // Layer = ground or ladder

    private bool _dropFromRope = false;
    private bool _shotBlockedLeft;
    private bool _shotBlockedRight;

    private Vector3 _spawnPoint;
    private Vector3 _ladderHitbox;

    void Awake()
    {
        _transform = transform;
    }

	void Start ()
	{
	    GameMaster.alive = true;
	    _spawnPoint = _transform.position;

	    _shootParent = transform.Find("shootParent");
	    var shoot = GameObject.Find("shoot");
	    _shootRenderer = shoot.renderer;
	    _shootSprite = shoot.GetComponent<OTAnimatingSprite>();
	}
	
	void Update ()
	{
	    UpdateRaycasts();
	    _moveDirX = 0;
	    _moveDirY = 0;

        if (GameMaster.isLeft && !GameMaster.blockedLeft && !GameMaster.shooting)
        {
            _moveDirX = -1;
            GameMaster.facingDir = Facing.Left;
        }

        if (GameMaster.isRight && !GameMaster.blockedRight && !GameMaster.shooting)
        {
            _moveDirX = 1;
            GameMaster.facingDir = Facing.Right;
        }

        if (GameMaster.isUp && !GameMaster.blockedUp && GameMaster.onLadder)
        {
            _moveDirY = 1;
            GameMaster.facingDir = Facing.Up;
        }

        if (GameMaster.isDown && !GameMaster.blockedDown && GameMaster.onLadder)
        {
            _moveDirY = -1;
            GameMaster.facingDir = Facing.Down;
        }

        if (GameMaster.isDown && GameMaster.onRope)
        {
            GameMaster.onRope = false;
            _dropFromRope = true;
        }

        if (GameMaster.isShoot && !GameMaster.shooting && !GameMaster.onRope && !GameMaster.falling && !_shotBlockedLeft &&
            !_shotBlockedRight)
        {
            StartCoroutine(Shoot());
        }
	    
        UpdateMovement();
	}

    private void UpdateMovement()
    {
        if (!GameMaster.falling || GameMaster.onLadder)
        {
            _movement = new Vector3(_moveDirX, _moveDirY, 0f);
            _movement *= Time.deltaTime * _moveSpeed;
            GetComponent<OTSprite>().position += (Vector2)_movement;
        }
        else
        {
            _movement = new Vector3(0f, -1f, 0f);
            _movement *= Time.deltaTime * _moveSpeed;
            GetComponent<OTSprite>().position += (Vector2)_movement;
        }
    }

    private IEnumerator Shoot()
    {
        GameMaster.shooting = true;

        // show the shoot sprite and play the animation
        _shootRenderer.enabled = true;
        _shootSprite.Play("shoot");

        // check facing direction and flip the shoot parent to the correct side
        if (GameMaster.facingDir == Facing.Left)
        {
            _shootParent.localScale = new Vector3(1, 1, 1); // left side
        }
        if (GameMaster.facingDir == Facing.Right)
        {
            _shootParent.localScale = new Vector3(-1, 1, 1); // right side
        }

        yield return new WaitForSeconds(0.4f);

        // hide the sprite
        _shootRenderer.enabled = false;
        GameMaster.shooting = false;
    }

    private void UpdateRaycasts()
    {
        GameMaster.blockedRight = false;
        GameMaster.blockedLeft = false;
        _shotBlockedLeft = false;
        _shotBlockedRight = false;

        // is the player standing on the ground?
        // cast 2 rays, one on each side of the character
        if (Physics.Raycast(new Vector3(_transform.position.x - 0.3f, _transform.position.y, _transform.position.z + 1f), -Vector3.up, out _hit, 0.7f, groundMask) || 
            Physics.Raycast(new Vector3(_transform.position.x + 0.3f, _transform.position.y, _transform.position.z + 1f), -Vector3.up, out _hit, 0.7f, groundMask))
        {
            GameMaster.falling = false;

            // snap the player to the top of a ground tile if she's not on a ladder
            if (!GameMaster.onLadder)
            {
                _transform.position = new Vector3(_transform.position.x, _hit.point.y + GameMaster.playerHitboxY, _transform.position.z);
            }
        }

        // then maybe she's falling
        else
        {
            if (!GameMaster.onRope && !GameMaster.falling && !GameMaster.onLadder)
            {
                GameMaster.falling = true;
            }
        }

        // player is blocked by something on the right
        // cast out 2 rays, one from the head and one from the feet
        if (Physics.Raycast(new Vector3(_transform.position.x, _transform.position.y + 0.3f, _transform.position.z + 1f), Vector3.right, _rayBlockedDistX, groundMask) || 
            Physics.Raycast(new Vector3(_transform.position.x, _transform.position.y - 0.4f, _transform.position.z + 1f), Vector3.right, _rayBlockedDistX, groundMask))
        {
            GameMaster.blockedRight = true;
        }

        // player is blocked by something on the left
        // cast out 2 rays, one from the head and one from the feet
        if (Physics.Raycast(new Vector3(_transform.position.x, _transform.position.y + 0.3f, _transform.position.z + 1f), -Vector3.right, _rayBlockedDistX, groundMask) || 
            Physics.Raycast(new Vector3(_transform.position.x, _transform.position.y - 0.4f, _transform.position.z + 1f), -Vector3.right, _rayBlockedDistX, groundMask))
        {
            GameMaster.blockedLeft = true;
        }

        // is there something blocking our shot to the right?
        if (Physics.Raycast(new Vector3(_transform.position.x, _transform.position.y, _transform.position.z + 1f), Vector3.right, 1f, shootMask))
        {
            _shotBlockedRight = true;
        }

        // is there something blocking our shot to the left?
        if (Physics.Raycast(new Vector3(_transform.position.x, _transform.position.y, _transform.position.z + 1f), -Vector3.right, 1f, shootMask))
        {
            _shotBlockedLeft = true;
        }

        // did the shot hit a brick tile to the left?
        if (Physics.Raycast(new Vector3(_transform.position.x - 1f, _transform.position.y, _transform.position.z + 1f), -Vector3.up, out _hit, 0.6f, groundMask))
        {
            if (!_shotBlockedLeft && GameMaster.isShoot && GameMaster.facingDir == Facing.Left)
            {
                // breaking bricks will be added in an upcomming tutorial
                /*if (hit.transform.GetComponent<Brick>())
                {
                    StartCoroutine(hit.transform.GetComponent<Brick>().PlayBreakAnim());
                }*/
            }
        }

        // did the shot hit a brick tile to the right?
        if (Physics.Raycast(new Vector3(_transform.position.x + 1f, _transform.position.y, _transform.position.z + 1f), -Vector3.up, out _hit, 0.6f, groundMask))
        {
            if (!_shotBlockedRight && GameMaster.isShoot && GameMaster.facingDir == Facing.Right)
            {
                // breaking bricks will be added in an upcomming tutorial
                /*if (hit.transform.GetComponent<Brick>())
                {
                    StartCoroutine(hit.transform.GetComponent<Brick>().PlayBreakAnim());
                }*/
            }
        }

        // is the player on the far right edge of the screen?
        if (_transform.position.x + GameMaster.playerHitboxX > (Camera.mainCamera.transform.position.x + GameMaster.orthSizeX))
        {
            GameMaster.blockedRight = true;
        }

        // is the player on the far left edge of the screen?
        if (_transform.position.x - GameMaster.playerHitboxX < (Camera.mainCamera.transform.position.x - GameMaster.orthSizeX))
        {
            GameMaster.blockedLeft = true;
        }
    }

    void RespawnPlayer()
    {
        // respawn the player at her initial start point
        _transform.position = _spawnPoint;
        GameMaster.alive = true;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided with " + other.name);
        // did the player collide with a pickup?
        // pickups and scoring will be added in an upcomming tutorial
        if (other.gameObject.CompareTag("Pickup"))
        {
            if (other.GetComponent<Pickup>())
            {
                other.GetComponent<Pickup>().PickMeUp();
                GameMaster.sc.Pickup();
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        // has the player been crushed by a block?
        // this will be added in an upcomming tutorial
        /*if (other.gameObject.CompareTag("Crusher"))
        {
            if(GameMaster.alive)
            {
                GameMaster.alive = false;
                RespawnPlayer();
                GameMaster.sc.LifeSubtract();
            }
        }*/

        // is the player overlapping a ladder?
        if (other.gameObject.CompareTag("Ladder"))
        {
            GameMaster.onLadder = false;
            GameMaster.blockedUp = false;
            GameMaster.blockedDown = false;

            _ladderHitbox.y = other.transform.localScale.y * 0.5f; // get half the ladders Y height

            // is the player overlapping the ladder?
            // if player is landing on top of ladder from a fall, let him pass by
            if ((_transform.position.y + GameMaster.playerHitboxY) < ((_ladderHitbox.y + 0.1f) + other.transform.position.y))
            {
                GameMaster.onLadder = true;
                GameMaster.falling = false;
            }

            // if the player is at the top of the ladder, then snap her to the top
            if ((_transform.position.y + GameMaster.playerHitboxY) >= (_ladderHitbox.y + other.transform.position.y) && GameMaster.isUp)
            {
                GameMaster.blockedUp = true;
                GameMaster.glx = _transform.position;
                GameMaster.glx.y = (_ladderHitbox.y + other.transform.position.y) - GameMaster.playerHitboxY;
                _transform.position = GameMaster.glx;
            }

            // if the player is at the bottom of the ladder, then snap her to the bottom
            if ((_transform.position.y - GameMaster.playerHitboxY) <= (-_ladderHitbox.y + other.transform.position.y))
            {
                GameMaster.blockedDown = true;
                GameMaster.glx = _transform.position;
                GameMaster.glx.y = (-_ladderHitbox.y + other.transform.position.y) + GameMaster.playerHitboxY;
                _transform.position = GameMaster.glx;
            }
        }

        // is the player overlapping a rope?
        if (other.gameObject.CompareTag("Rope"))
        {
            GameMaster.onRope = false;

            if (!GameMaster.onRope && !_dropFromRope)
            {
                // snap player to center of the rope
                if (_transform.position.y < (other.transform.position.y + 0.2f) && _transform.position.y > (other.transform.position.y - 0.2f))
                {
                    GameMaster.onRope = true;
                    GameMaster.falling = false;
                    GameMaster.glx = _transform.position;
                    GameMaster.glx.y = other.transform.position.y;
                    _transform.position = GameMaster.glx;
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // did the player exit a rope trigger?
        if (other.gameObject.CompareTag("Rope"))
        {
            GameMaster.onRope = false;
            _dropFromRope = false;
        }

        // did the player exit a ladder trigger?
        if (other.gameObject.CompareTag("Ladder"))
        {
            GameMaster.onLadder = false;
        }
    }
}
