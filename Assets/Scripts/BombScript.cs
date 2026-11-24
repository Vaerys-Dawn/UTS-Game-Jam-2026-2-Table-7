using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class BombScript : MonoBehaviour, DamageSource
{
    GameObject source = null;
    private Vector3 lastPos;
    int doExplode = -400;
    private List<GameObject> triggerItems = new List<GameObject>();
    [SerializeField]
    GameObject animSource;
    Animator animator;

    [SerializeField]
    Transform arrowSprite;

    Rigidbody2D rb;

    [SerializeField] //Long added more
    float blastRadius = 1.5f; // radius used to damage breakable tiles 
    private bool exploded = false; // stops the bomb exploding twice 

    public GameObject GetSource()
    {
        return source;
    }

    public void SetSource(GameObject sourceObj, Vector3 pos, Vector3 dir)
    {
        this.source = sourceObj;
        transform.position = pos;
        lastPos = pos;
        Rigidbody2D rbProj = gameObject.GetComponent<Rigidbody2D>();
        if (dir == Vector3.zero) dir = Vector3.left;
        rbProj.AddForce(dir * 2000);
        UpdateRotation();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
       
        
        if (doExplode != -400 && Time.timeScale != 0)
        {
            if (doExplode > 0) {
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
            }
            doExplode--;
            if (doExplode == 0)
            {
                Explode();
            }
            else if (doExplode < -40)
            {
                Destroy(this.gameObject);
            }
        }
        if (lastPos != null && lastPos != transform.position && rb.constraints != RigidbodyConstraints2D.FreezeAll)
        {
            UpdateRotation();
        }
        lastPos = transform.position;
    }

    private void UpdateRotation()
    {
        Vector3 direction = Vector3.Normalize(transform.position - lastPos);
        arrowSprite.rotation = Quaternion.LookRotation(Vector3.forward, Quaternion.Euler(0, 0, 90) * direction);
    }

    private void Explode()
    {
        exploded = true; //Long added more
        animSource.SetActive(true);
        animator.SetTrigger("Explode");
        rb.linearVelocity = Vector3.zero;
        foreach (GameObject g in triggerItems)
        {
            Damageable d = g.GetComponent<Damageable>();
            if ( d!= null)
            {
                d.HandleDamage(this, 100);
            }
        }
        DamageTilemaps(); //Long added more
    }

    // Finds tilemaps on the Terrain layer inside the blast and adds one hit to their breakable tiles. //Long added more
    private void DamageTilemaps() {
        HashSet<DestructibleTilemap> damaged = new HashSet<DestructibleTilemap>(); 
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, blastRadius, LayerMask.GetMask("Terrain")); 
        foreach (Collider2D col in hits) 
        {
            DestructibleTilemap tilemap = col.GetComponentInParent<DestructibleTilemap>(); 
            if (tilemap != null && damaged.Add(tilemap)) // each tilemap only once per explosion 
            {
                tilemap.ApplyExplosion(transform.position, blastRadius); 
            }
        }
    }

    private void OnDrawGizmosSelected() // shows the blast radius in the Scene view //Long added more
    {
        Gizmos.color = Color.yellow; 
        Gizmos.DrawWireSphere(transform.position, blastRadius); 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (exploded) return; // already exploded, don't restart the countdown //Long added more
        doExplode = 10;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        triggerItems.Add(collision.gameObject);
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        triggerItems.Remove(collision.gameObject);
    }
}
