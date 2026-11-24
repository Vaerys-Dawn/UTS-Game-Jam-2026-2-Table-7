using Unity.VisualScripting;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private Transform shootpoint;
    public float throwforce = 2000;
    public float throwCD = 1;
    private float time = 0;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time > throwCD)
        {
            time = 0;

            // now, get all NPC's with the "NPC" tag.
            GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
            if (npcs.Length == 0)
            {
                Debug.Log("There are no NPCs left.");
            }
            else
            {
                GameObject newbullet = Instantiate(bullet);
                newbullet.GetComponent<BombScript>().SetSource(this.gameObject);
                newbullet.transform.position = shootpoint.position;
                Rigidbody2D forces = newbullet.GetComponent<Rigidbody2D>();
                GameObject nearest = npcs[0];
                foreach (var item in npcs)
                {
                    if (Vector2.Distance(transform.position, item.gameObject.transform.position) < Vector2.Distance(transform.position, nearest.gameObject.transform.position))
                    {
                        nearest = item;
                    }
                }

                Vector2 angle = Vector2.Normalize(nearest.transform.position - transform.position + (Vector3.up * Mathf.Sqrt(Vector3.Distance(transform.position, nearest.transform.position)) * 0.2f));
                forces.AddForce(angle * throwforce);


            }
            
        }
    }
}
