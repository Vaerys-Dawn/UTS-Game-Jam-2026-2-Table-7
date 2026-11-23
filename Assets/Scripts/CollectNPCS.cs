using System.Collections.Generic;
using UnityEngine;

public class CollectNPCS : MonoBehaviour
{

    [SerializeField]
    private List<GameObject> npcs = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<GameObject> getNpcs()
    {
        return npcs;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("NPC")) {
            npcs.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        npcs.Remove(collision.gameObject);
    }
}
