using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MC_Movement : MonoBehaviour
{
    public float moveSpeed = 10;
    public float jumpforce = 0;
    public bool grounded = false;
    public int jumpcount = 0;
    private float jumpCooldown = 0;
    private Rigidbody2D rb;
    //Dash variables
    private int dashCount = 1;
    private bool isDashing;
    private float dashingPower = 25f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 5f;
    public Color newColor;
    private Color originalColor;

    [SerializeField] private TrailRenderer tr;

    //Animation variables
    private Animator animator;
    private SpriteRenderer sr;
    bool mayRun = false;
    bool mayJump = false;
    bool mayFall = false;
    Vector2 dashDir;
    // Start is called before the first frame update

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        tr = GetComponent<TrailRenderer>();
        originalColor = sr.color;
    }

    void Update()
    {
        if (isDashing == true) {
            return;
        }
        Walk();
        Jump();
        Attack();
        dashDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        if (Input.GetKeyDown(KeyCode.LeftControl) && dashCount > 0) {
            StartCoroutine(Dash());
        }
    }
    void Walk()
    {
        //Run action
        float horizontalMovement = Input.GetAxis("Horizontal");
        Vector2 newVelocity = new Vector2(horizontalMovement * moveSpeed, rb.velocity.y);
        rb.velocity = newVelocity;
        Vector2 dashDir = new Vector2(horizontalMovement * moveSpeed, Input.GetAxis("Vertical") * moveSpeed);

        //Run/Idle animation
        if (!mayJump && newVelocity.x > 0.01f && grounded == true || !mayJump && newVelocity.x < -0.01f && grounded == true)
        {
            animator.SetBool("mayRun", true);
            animator.SetBool("mayIdle", false);
            animator.SetBool("mayFall", false);
            animator.SetBool("mayDash", false);
        }
        else if(!mayJump && (newVelocity.x > -0.01f && newVelocity.x < 0.01f))
        {
            animator.SetBool("mayRun", false);
            animator.SetBool("mayIdle", true);
            animator.SetBool("mayFall", false);
            animator.SetBool("mayDash", false);
        }
        if(newVelocity.x < -0.01f)
        {
            sr.flipX = true;
        }else if(newVelocity.x > 0.01f)
        {
            sr.flipX = false;
        }
    }
    void Jump()
    {
        if (Physics2D.Raycast(this.transform.position, Vector2.down, 1.2f))
        {
            grounded = true;
            animator.SetBool("mayJump", false);
            mayJump = false;
            mayFall = false;
            jumpcount = 1;
            dashCount = 2;
        }
        else
        {
            //Fall animation
            animator.SetBool("mayIdle", false);
            animator.SetBool("mayRun", false);
            animator.SetBool("mayJump", true);
            animator.SetBool("mayDash", false);
            if (rb.velocity.y < -0.1f) {
                animator.SetBool("mayFall", true);
                animator.SetBool("mayJump", false);
            };
        }
        if (jumpcount > 0 && Input.GetKeyDown(KeyCode.Space))
        {
            //Jump animation
            mayJump = true;
            animator.SetBool("mayJump", true);
            
            //Jump action
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0, 200f), ForceMode2D.Impulse);
            jumpcount--;
        }
    }

    void Attack() { 
        
    
    
    }

    private IEnumerator Dash() {
        int tempDir;
        if (sr.flipX)
        {
            tempDir = -1;
        }
        else { tempDir = 1; }
        animator.SetBool("mayIdle", false);
        animator.SetBool("mayRun", false);
        animator.SetBool("mayJump", false);
        animator.SetBool("mayFall", false);
        animator.SetBool("mayDash", true);

        sr.color = Color.cyan;
        dashCount--;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = dashDir * dashingPower;
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        sr.color = originalColor;
        yield return new WaitForSeconds(dashingCooldown);
    }
}
