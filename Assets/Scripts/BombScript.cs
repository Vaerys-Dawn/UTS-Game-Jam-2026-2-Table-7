using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BombScript : MonoBehaviour, DamageSource
{
    GameObject source = null;
    int doExplode = -400;
    private List<GameObject> triggerItems = new List<GameObject>();
    [SerializeField]
    GameObject animSource;
    Animator animator;

    public GameObject GetSource()
    {
        return source;
    }

    public void SetSource(GameObject gameObject)
    {
        this.source = gameObject;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (doExplode != -400 && Time.timeScale != 0)
        {
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
    }

    private void Explode()
    {
        animSource.SetActive(true);
        animator.SetTrigger("Explode");
        this.GetComponent<Rigidbody2D>().linearVelocity = Vector3.zero;
        Debug.Log(triggerItems);
        foreach (GameObject g in triggerItems)
        {
            Debug.Log(g.gameObject.name);
            Damageable d = g.GetComponent<Damageable>();
            if ( d!= null)
            {
                d.HandleDamage(this, 100);                
            }
        }
        Debug.Log("Kaboom");
        
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
