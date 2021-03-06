using System;
using System.Collections;
using System.Collections.Generic;
using Ash.MyUtils;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] private GameObject cursor;
    [SerializeField] private GameObject fist;
    [SerializeField] private GameObject weapon;
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody2D rigidbody2D;
    [SerializeField] private SpriteRenderer spr;
    [SerializeField] private GameObject[] lifeHearts;
    public GameObject damageFlash;
    
    [SerializeField] private GameObject playerSelectedIndicator;
    public Animator anim;
    public bool isSelected = false;
    public bool isBusy = false;
    private Vector3 velocity = Vector3.zero;
    private float movementSmoothing = .05f;
    private Vector3 cursorDirection;
    public float angleToCursor;
    private Vector2 mousePosition;
    private PlayerControls playerControls;
    private Camera cam;
    private float fistXPosition;
    public GameController gameController;
    private Material matWhite;
    private Material matDefault;
    private CameraController cameraController;
    
    private int playerHealth = 5;

    private static readonly int IsIdle = Animator.StringToHash("IsIdle");
    //public float angleToCursor;
    
    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Awake()
    {
        cameraController = FindObjectOfType<CameraController>();
        matWhite = Resources.Load("WhiteFlash", typeof(Material)) as Material; 
        matDefault = spr.material;
        gameController = FindObjectOfType<GameController>();
        fistXPosition = fist.transform.localPosition.x;
        anim.SetBool(IsIdle, true);
        cam = Camera.main;
        playerControls = new PlayerControls();
        playerControls.Gameplay.MousePosition.performed += cxt => SetMousePosition(cxt.ReadValue<Vector2>());
        playerControls.Gameplay.ActionOne.performed += cxt => ActionOne();
        playerControls.Gameplay.ActionOne.canceled += cxt => ActionOneCancelled();
        playerControls.Gameplay.ActionTwo.performed += cxt => ActionTwo();

    }

    public virtual void Update()
    {
        AdjustCursorPosition();
        angleToCursor = MyUtils.GetAngleFromVectorFloat(cursorDirection.normalized);
    }

    public void PlayerSelected()
    {
        playerSelectedIndicator.SetActive(true);
        isSelected = true;
        cursor.SetActive(true);
    }

    public void PlayerDeselected()
    {
        playerSelectedIndicator.SetActive(false);

        isSelected = false;
        rigidbody2D.velocity = Vector2.zero;
        cursor.SetActive(false);
    }

    public void PlayerCharacterMove(Vector2 dir)
    {
        if (!isBusy)
        {
            Vector3 targetVelocity = new Vector3(dir.x, dir.y) * speed;
            rigidbody2D.velocity = Vector3.SmoothDamp(rigidbody2D.velocity, targetVelocity, ref velocity, movementSmoothing);
            
            if(dir == Vector2.zero)
                anim.SetBool(IsIdle, true);
            else
                anim.SetBool(IsIdle, false);
            
            ClampPlayerToMap();
        }
        else
        {
            rigidbody2D.velocity = Vector3.zero;
            anim.SetBool(IsIdle, true);
        }
    }
    
    public void ClampPlayerToMap()
    {
        Vector2 clampedPos = transform.position;
        clampedPos.x = Mathf.Clamp(transform.position.x, -12.5f, 12.5f);
        clampedPos.y = Mathf.Clamp(transform.position.y, -8f, 8f );
        transform.position = clampedPos;
    }
    
    public void SpriteFlip()
    {
        if (cursorDirection.x < 0)
        {
            spr.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            spr.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void RotateFist()
    {
        if (cursorDirection.x < 0)
        {
            fist.transform.localPosition = new Vector3(-fistXPosition, fist.transform.localPosition.y, 0);
            weapon.transform.localScale = new Vector3(1, -1, 1);
        }
        else
        {
            fist.transform.localPosition = new Vector3(fistXPosition, fist.transform.localPosition.y, (1));
            weapon.transform.localScale = new Vector3(1, 1, 1);
        }
        
        var dir = cursor.transform.position - transform.position;
        var rot = MyUtils.GetAngleFromVectorFloat(dir);
        fist.transform.rotation = Quaternion.Euler(0,0,rot);
    }
    
    private void AdjustCursorPosition()
    {
        var originPos = transform.position;
        var mousePos = cam.ScreenToWorldPoint((mousePosition));
        var convertedMousePos = new Vector3(mousePos.x, mousePos.y, 0f);
        var aimDirection = (convertedMousePos - originPos).normalized;

        cursor.transform.position = convertedMousePos;
        cursorDirection = new Vector3(aimDirection.x, aimDirection.y, 0f);
        
        //Vector3 clampedPos = cursor.transform.position;
        //clampedPos.x = Mathf.Clamp(cursor.transform.position.x, originPos.x - screenBounds.x, originPos.x + screenBounds.x);
        //clampedPos.z = Mathf.Clamp(cursor.transform.position.z, originPos.z - screenBounds.y, originPos.z + screenBounds.y);
        //cursor.transform.position = clampedPos;
    }
    
    private void SetMousePosition(Vector2 pos) => mousePosition = pos;

    public virtual void ActionOne()
    {
        // perform first action
    }
    
    public virtual void ActionTwo()
    {
        // perform second action
    }

    public virtual void ActionOneCancelled()
    {
        
    }

    public void TakeDamage()
    {
        if (gameController.gameOver) return;
        
        cameraController.CameraShake();
        damageFlash.SetActive(true);
        spr.material = matWhite;
        Invoke("SwapMaterialToDefault", .1f);
        playerHealth--;
        lifeHearts[playerHealth].SetActive(false);
        if (playerHealth <= 0)
        {
            gameController.GameOver();
        }
    }
    
    private void SwapMaterialToDefault()
    {
        damageFlash.SetActive(false);
        spr.material = matDefault;
    }
    

    
    
}
