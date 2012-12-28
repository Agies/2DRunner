using UnityEngine;

public class Character : MonoBehaviour
{
    public LayerMask groundMask; // Layer == Ground
    public LayerMask shootMask; // Layer = ground or ladder

    public float moveSpeed = 5;
    public float gravitySpeed = 5;
    
    public bool blockedRight = false;
    public bool blockedLeft = false;
    public bool blockedUp = false;
    public bool blockedDown = false;

    public float playerHitboxX = 0.225f;
    public float playerHitboxY = 0.5f;

    public bool isLeft;
    public bool isRight;
    public bool isUp;
    public bool isDown;
    public bool isShoot;

    public bool alive;
    public bool onLadder;
    public bool onRope;
    public bool falling;
    public bool shooting;

    public Facing facingDir = Facing.Left;

    public Vector3 glx;

    protected Transform characterTransform;
    protected bool dropFromRope = false;
    protected bool shotBlockedLeft;
    protected bool shotBlockedRight;
    private Vector3 _ladderHitbox;
    
    private int _moveDirX;
    private int _moveDirY;
    private Vector3 _movement;

    private float _rayBlockedDistX = 0.6f;
    private RaycastHit _hit;

    public virtual void Awake()
    {
        characterTransform = transform;
    }

    public virtual void Start()
    {
        alive = true;
    }

    protected virtual void UpdateMovement()
    {
        UpdateRaycasts();
        UpdateRaycasts();
        
        _moveDirX = 0;
        _moveDirY = 0;

        if (isLeft && !blockedLeft && !shooting)
        {
            _moveDirX = -1;
            facingDir = Facing.Left;
        }

        if (isRight && !blockedRight && !shooting)
        {
            _moveDirX = 1;
            facingDir = Facing.Right;
        }

        if (isUp && !blockedUp && onLadder)
        {
            _moveDirY = 1;
            facingDir = Facing.Up;
        }

        if (isDown && !blockedDown && onLadder)
        {
            _moveDirY = -1;
            facingDir = Facing.Down;
        }

        if (isDown && onRope)
        {
            onRope = false;
            dropFromRope = true;
        }

        if (!falling || onLadder)
        {
            _movement = new Vector3(_moveDirX, _moveDirY, 0f);
            _movement *= Time.deltaTime * moveSpeed;
            GetComponent<OTSprite>().position += (Vector2)_movement;
        }
        else
        {
            _movement = new Vector3(0f, -1f, 0f);
            _movement *= Time.deltaTime * moveSpeed;
            GetComponent<OTSprite>().position += (Vector2)_movement;
        }
    }

    private void UpdateRaycasts()
    {
        blockedRight = false;
        blockedLeft = false;
        shotBlockedLeft = false;
        shotBlockedRight = false;

        // is the player standing on the ground?
        // cast 2 rays, one on each side of the character
        if (Physics.Raycast(new Vector3(characterTransform.position.x - 0.3f, characterTransform.position.y, characterTransform.position.z + 1f), -Vector3.up, out _hit, 0.7f, groundMask) ||
            Physics.Raycast(new Vector3(characterTransform.position.x + 0.3f, characterTransform.position.y, characterTransform.position.z + 1f), -Vector3.up, out _hit, 0.7f, groundMask))
        {
            falling = false;

            // snap the player to the top of a ground tile if she's not on a ladder
            if (!onLadder)
            {
                characterTransform.position = new Vector3(characterTransform.position.x, _hit.point.y + playerHitboxY, characterTransform.position.z);
            }
        }

            // then maybe she's falling
        else
        {
            if (!onRope && !falling && !onLadder)
            {
                falling = true;
            }
        }

        // player is blocked by something on the right
        // cast out 2 rays, one from the head and one from the feet
        if (Physics.Raycast(new Vector3(characterTransform.position.x, characterTransform.position.y + 0.3f, characterTransform.position.z + 1f), Vector3.right, _rayBlockedDistX, groundMask) ||
            Physics.Raycast(new Vector3(characterTransform.position.x, characterTransform.position.y - 0.4f, characterTransform.position.z + 1f), Vector3.right, _rayBlockedDistX, groundMask))
        {
            blockedRight = true;
        }

        // player is blocked by something on the left
        // cast out 2 rays, one from the head and one from the feet
        if (Physics.Raycast(new Vector3(characterTransform.position.x, characterTransform.position.y + 0.3f, characterTransform.position.z + 1f), -Vector3.right, _rayBlockedDistX, groundMask) ||
            Physics.Raycast(new Vector3(characterTransform.position.x, characterTransform.position.y - 0.4f, characterTransform.position.z + 1f), -Vector3.right, _rayBlockedDistX, groundMask))
        {
            blockedLeft = true;
        }

        // is there something blocking our shot to the right?
        if (Physics.Raycast(new Vector3(characterTransform.position.x, characterTransform.position.y, characterTransform.position.z + 1f), Vector3.right, 1f, shootMask))
        {
            shotBlockedRight = true;
        }

        // is there something blocking our shot to the left?
        if (Physics.Raycast(new Vector3(characterTransform.position.x, characterTransform.position.y, characterTransform.position.z + 1f), -Vector3.right, 1f, shootMask))
        {
            shotBlockedLeft = true;
        }

        // did the shot hit a brick tile to the left?
        if (Physics.Raycast(new Vector3(characterTransform.position.x - 1f, characterTransform.position.y, characterTransform.position.z + 1f), -Vector3.up, out _hit, 0.6f, groundMask))
        {
            if (!shotBlockedLeft && isShoot && facingDir == Facing.Left)
            {
                // breaking bricks will be added in an upcomming tutorial
                /*if (hit.transform.GetComponent<Brick>())
                {
                    StartCoroutine(hit.transform.GetComponent<Brick>().PlayBreakAnim());
                }*/
            }
        }

        // did the shot hit a brick tile to the right?
        if (Physics.Raycast(new Vector3(characterTransform.position.x + 1f, characterTransform.position.y, characterTransform.position.z + 1f), -Vector3.up, out _hit, 0.6f, groundMask))
        {
            if (!shotBlockedRight && isShoot && facingDir == Facing.Right)
            {
                // breaking bricks will be added in an upcomming tutorial
                /*if (hit.transform.GetComponent<Brick>())
                {
                    StartCoroutine(hit.transform.GetComponent<Brick>().PlayBreakAnim());
                }*/
            }
        }

        // is the player on the far right edge of the screen?
        if (characterTransform.position.x + playerHitboxX > (Camera.mainCamera.transform.position.x + GameMaster.orthSizeX))
        {
            blockedRight = true;
        }

        // is the player on the far left edge of the screen?
        if (characterTransform.position.x - playerHitboxX < (Camera.mainCamera.transform.position.x - GameMaster.orthSizeX))
        {
            blockedLeft = true;
        }
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        // has the player been crushed by a block?
        // this will be added in an upcomming tutorial
        /*if (other.gameObject.CompareTag("Crusher"))
        {
            if(alive)
            {
                alive = false;
                RespawnPlayer();
                sc.LifeSubtract();
            }
        }*/

        // is the player overlapping a ladder?
        if (other.gameObject.CompareTag("Ladder"))
        {
            onLadder = false;
            blockedUp = false;
            blockedDown = false;

            _ladderHitbox.y = other.transform.localScale.y * 0.5f; // get half the ladders Y height

            // is the player overlapping the ladder?
            // if player is landing on top of ladder from a fall, let him pass by
            if ((characterTransform.position.y + playerHitboxY) < ((_ladderHitbox.y + 0.1f) + other.transform.position.y))
            {
                onLadder = true;
                falling = false;
            }

            // if the player is at the top of the ladder, then snap her to the top
            if ((characterTransform.position.y + playerHitboxY) >= (_ladderHitbox.y + other.transform.position.y) && isUp)
            {
                blockedUp = true;
                glx = characterTransform.position;
                glx.y = (_ladderHitbox.y + other.transform.position.y) - playerHitboxY;
                characterTransform.position = glx;
            }

            // if the player is at the bottom of the ladder, then snap her to the bottom
            if ((characterTransform.position.y - playerHitboxY) <= (-_ladderHitbox.y + other.transform.position.y))
            {
                blockedDown = true;
                glx = characterTransform.position;
                glx.y = (-_ladderHitbox.y + other.transform.position.y) + playerHitboxY;
                characterTransform.position = glx;
            }
        }

        // is the player overlapping a rope?
        if (other.gameObject.CompareTag("Rope"))
        {
            onRope = false;

            if (!onRope && !dropFromRope)
            {
                // snap player to center of the rope
                if (characterTransform.position.y < (other.transform.position.y + 0.2f) && characterTransform.position.y > (other.transform.position.y - 0.2f))
                {
                    onRope = true;
                    falling = false;
                    glx = characterTransform.position;
                    glx.y = other.transform.position.y;
                    characterTransform.position = glx;
                }
            }
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        // did the player exit a rope trigger?
        if (other.gameObject.CompareTag("Rope"))
        {
            onRope = false;
            dropFromRope = false;
        }

        // did the player exit a ladder trigger?
        if (other.gameObject.CompareTag("Ladder"))
        {
            onLadder = false;
        }
    }
}