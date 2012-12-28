using UnityEngine;
using System.Collections;

public class GameMaster : MonoBehaviour
{
    public static Scoring sc;

    public static float orthSize;
    public static float orthSizeX;
    public static float orthSizeY;
    public static float camRatio;

    public static bool blockedRight = false;
    public static bool blockedLeft = false;
    public static bool blockedUp = false;
    public static bool blockedDown = false;

    public static float playerHitboxX = 0.225f;
    public static float playerHitboxY = 0.5f;
    
    public static bool isLeft;
    public static bool isRight;
    public static bool isUp;
    public static bool isDown;
    public static bool isShoot;
    
    public static bool alive;
    public static bool onLadder;
    public static bool onRope;
    public static bool falling;
    public static bool shooting;

    public static Facing facingDir = Facing.Left;

    public static Vector3 glx;

    void Start ()
    {
        camRatio = 1.333f;
        orthSize = Camera.mainCamera.camera.orthographicSize;
        orthSizeX = orthSize*camRatio;

        sc = (Scoring) gameObject.GetComponent("Scoring");
    }

    private void Update()
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
    }
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
}

public enum Facing
{
    Left = 1,
    Right = 2,
    Up = 3,
    Down = 4
}
