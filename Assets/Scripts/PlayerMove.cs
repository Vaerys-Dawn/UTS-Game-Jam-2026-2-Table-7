using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField]
    Rigidbody2D rb;

    [SerializeField]
    Vector3 offset;

    [SerializeField]
    float maxSpeed;

    [SerializeField]
    float jumpMod = 1;

    [SerializeField]
    float jumpDist = 0.5f;
    [SerializeField]
    private float rayStartPos = 0.9f;
    [SerializeField]
    private float jumpLock = 0f;

    private string[] floorLayers = { "Terrain" };

    private bool hasJumped = false;

    private bool IsFacingRight = true;

    Controls controls;

    [SerializeField]
    private GameObject projectile;

    [SerializeField]
    private Vector3 projectilePos;

    [SerializeField]
    private CollectNPCS npcCollector;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Transform playerSprite;

    private float attackDelay = -3;
    [SerializeField]
    private float windup = 0.5f;

    [SerializeField]
    private Transform reticle;
    [SerializeField]
    private float protDuration = 5;
    [SerializeField]
    private GameObject shieldPrefab;
    [SerializeField]
    private GameObject LazerPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controls = GameObject.FindWithTag("Managers").GetComponent<Controls>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleCooldowns();
        bool grounded = IsOnGround();
        animator.SetBool("InAir", !grounded);
        animator.SetBool("IsFalling", rb.linearVelocityY <= 0.01f);
        jumpLock -= Time.deltaTime;
        if (grounded && jumpLock <= 0f && rb.linearVelocityY <= 0.01f) hasJumped = false; //reset jump when on ground and not moving up

        if (controls.Jump) {
            Debug.Log("Jump Pressed");
        }
        
        if (controls.JumpHeld && grounded && !hasJumped)
        {
            rb.linearVelocityY = 0.0f;
            rb.AddForce(Vector2.up * 1000 * jumpMod);
            jumpLock = 0.5f; // Set a lock time for jumping
            hasJumped = true;
        }

  

        if (controls.Special)
        {
            GameObject lazer = Instantiate(LazerPrefab, transform);
            lazer.transform.position = transform.position + (Vector3.up * 3);
            foreach (GameObject g in npcCollector.getNpcs())
            {
                Rigidbody2D npcRB = g.GetComponent<Rigidbody2D>();
                float distance = Vector3.Distance(transform.position, npcRB.position);


                if (npcRB != null && distance <= 10)
                {
                    Vector3 direction = this.transform.position + (Vector3.up * 3) - npcRB.transform.position;
                    Vector2 dir = Vector3.Normalize(direction);
                    npcRB.AddForce(dir * 2000);
                }
            }
        }
        if (controls.Interact)
        {
            Debug.Log("Interact Pressed");
            GameObject shield = Instantiate(shieldPrefab);
            shield.transform.position = transform.position;
            foreach (GameObject g in npcCollector.getNpcs())
            {
                Protectable prot = g.GetComponent<Protectable>();
                float distance = Vector3.Distance(transform.position, g.transform.position);

                if (prot != null && distance <= 6)
                {
                    prot.SetProtection(protDuration);
                }
            }
        }


        HandleMovement();

        if (IsFacingRight)
        {
            playerSprite.localScale = new Vector3(4, 4, 4);
            projectilePos.x = Mathf.Abs(projectilePos.x);
        }else
        {
            playerSprite.localScale = new Vector3(-4, 4, 4);
            projectilePos.x = -Mathf.Abs(projectilePos.x);
        }

        if (controls.Attack && attackDelay == -3)
        {
            animator.SetTrigger("Attack");
            attackDelay = windup;
        }
        if (attackDelay >= 0)
        {
            attackDelay -= Time.deltaTime;
        }
        else if (attackDelay <= 0 && attackDelay != -3)
        {
            GameObject proj = Instantiate(projectile);
            proj.GetComponent<BombScript>().SetSource(this.gameObject, transform.position + projectilePos, Vector3.Normalize(controls.Look));
            attackDelay = -3;
        }
        RotateReticle();
    }

    private void RotateReticle()
    {
        reticle.localPosition = projectilePos;
        if (controls != null)
        {
            Vector3 endPos = new Vector3(controls.Look.x, controls.Look.y, 0) * 5;

            Vector3 dir = transform.position - transform.position + endPos;
            reticle.rotation = Quaternion.LookRotation(Vector3.forward, dir);
        }
    }

    private void HandleCooldowns()
    {
      
    }

    private void HandleMovement()
    {
        //Calculate the direction we want to move in and our desired velocity
        float targetSpeed = controls.Move.x * maxSpeed;

        //We can reduce are control using Lerp() this smooths changes to are direction and speed
        targetSpeed = Mathf.Lerp(rb.linearVelocity.x, targetSpeed, 1);


        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - rb.linearVelocity.x;
        //Calculate force along x-axis to apply to thr player

        float movement = speedDif;
        if (Mathf.Abs(movement) > 0.1f)
        {
            animator.SetBool("IsRunning", true);
        }else
        {
            animator.SetBool("IsRunning", false);
        }

        if (attackDelay != -3)
        {
            IsFacingRight = controls.Look.x > 0;
        }
        else
        {
            if (rb.linearVelocityX > 0.1f)
            {
                IsFacingRight = true;
            }
            else if (rb.linearVelocityX < -0.1f)
            {
                IsFacingRight = false;
            }
        }

        

        //Convert this to a vector and apply to rigidbody
        //Debug.Log("Movement: " + movement + ", SpeedDif: " + speedDif + ", Target Speed: " + targetSpeed + ", Move: " + controls.Move.x);
        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    private Vector3 GetRayPos()
    {
        return rb.transform.position + Vector3.down * rayStartPos;
    }

    private void OnDrawGizmos()
    {
        Debug.DrawLine(GetRayPos(), GetRayPos() + (Vector3.down * jumpDist), Color.red);
        Debug.DrawLine(GetRayPos() - offset, GetRayPos() - offset + (Vector3.down * jumpDist), Color.red);
        Debug.DrawLine(GetRayPos() + offset, GetRayPos() + offset + (Vector3.down * jumpDist), Color.red);

        if (controls != null)
        {
            Vector3 endPos = new Vector3(controls.Look.x, controls.Look.y, 0) * 5;

            Debug.DrawLine(this.transform.position, this.transform.position + endPos);
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position + projectilePos, 0.2f);
       
    }


    private bool IsOnGround()
    {

        LayerMask mask = LayerMask.GetMask(floorLayers);
        RaycastHit2D hit = Physics2D.Raycast(GetRayPos(), Vector2.down, jumpDist, mask);
        RaycastHit2D hitLeft = Physics2D.Raycast(GetRayPos() + offset, Vector2.down, jumpDist, mask);
        RaycastHit2D hitRight = Physics2D.Raycast(GetRayPos() - offset, Vector2.down, jumpDist, mask);
        //     Debug.Log(hit.point);

        if (hit || hitLeft || hitRight)
        {
            return true;
        }
        return false;
    }
}
