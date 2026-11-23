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

    public GameObject GetSource()
    {
        return source;
    }

    public void SetSource(GameObject gameObject)
    {
        this.source = gameObject;
        lastPos = gameObject.transform.position;
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
        Debug.Log(direction);
        arrowSprite.rotation = Quaternion.LookRotation(Vector3.forward, Quaternion.Euler(0, 0, 90) * direction);
    }

    private void Explode()
    {
        animSource.SetActive(true);
        animator.SetTrigger("Explode");
        rb.linearVelocity = Vector3.zero;
        foreach (GameObject g in triggerItems)
        {
            Debug.Log(g.gameObject.name);
            Damageable d = g.GetComponent<Damageable>();
            if ( d!= null)
            {
                d.HandleDamage(this, 100);                
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
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
