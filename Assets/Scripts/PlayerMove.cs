using NUnit.Framework;
using System;
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

    private string[] floorLayers = { "Terrain" };

    private bool hasJumped = false;

    Controls controls;

    [SerializeField]
    private GameObject projectile;

    [SerializeField]
    private Transform projectilePos;



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
        if (!grounded) hasJumped = false;

        if (controls.Jump) {
            Debug.Log("Jump Pressed");
        }
        
        if (controls.JumpHeld && grounded && !hasJumped)
        {
            rb.linearVelocityY = 0.0f;
            rb.AddForce(Vector2.up * 1000 * jumpMod);
            hasJumped = true;
        }

        if (controls.Attack) {
            GameObject proj = Instantiate(projectile);
            proj.transform.position = projectilePos.position;
            Rigidbody2D rbProj = proj.GetComponent<Rigidbody2D>();
            Vector3 dir = Vector3.Normalize(controls.Look);
            if (dir == Vector3.zero) dir = Vector3.left;
            rbProj.AddForce(dir * 2000);
        }

        HandleMovement();


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
