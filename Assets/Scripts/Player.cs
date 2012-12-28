using System.Collections;
using UnityEngine;

public class Player : Character
{
    private Transform _shootParent;
    private Renderer _shootRenderer;
    private OTAnimatingSprite _shootSprite;
    private Vector3 _spawnPoint;

    public override void Start ()
	{
        base.Start();
	    
        _spawnPoint = characterTransform.position;

	    _shootParent = transform.Find("shootParent");
	    var shoot = GameObject.Find("shoot");
	    _shootRenderer = shoot.renderer;
	    _shootSprite = shoot.GetComponent<OTAnimatingSprite>();
	}
	
	void Update ()
	{
        isLeft = false;
        isRight = false;
        isUp = false;
        isDown = false;
        isShoot = false;

        // keyboard input
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            isLeft = true;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            isRight = true;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            isUp = true;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            isDown = true;
        }

        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.E))
        {
            isShoot = true;
        }

        UpdateMovement();

        if (isShoot && 
            !shooting && 
            !onRope && 
            !falling && 
            !shotBlockedLeft &&
            !shotBlockedRight)
        {
            StartCoroutine(Shoot());
        }
	}
    
    private IEnumerator Shoot()
    {
        shooting = true;

        _shootRenderer.enabled = true;
        _shootSprite.Play("shoot");

        if (facingDir == Facing.Left)
        {
            _shootParent.localScale = new Vector3(1, 1, 1); // left side
        }
        if (facingDir == Facing.Right)
        {
            _shootParent.localScale = new Vector3(-1, 1, 1); // right side
        }

        yield return new WaitForSeconds(0.4f);

        _shootRenderer.enabled = false;
        shooting = false;
    }
    
    void RespawnPlayer()
    {
        // respawn the player at her initial start point
        characterTransform.position = _spawnPoint;
        alive = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pickup"))
        {
            if (other.GetComponent<Pickup>())
            {
                other.GetComponent<Pickup>().PickMeUp();
                GameMaster.sc.Pickup();
            }
        }
    }

    
}
