using UnityEngine;

public class NPCController : MonoBehaviour, Damageable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private float time;
    private float nextMoveTime;
    public float moveTimeInterval;
    private GameObject sprite;
    private Rigidbody2D forces;
    public float forcesApplied = 10;
    public float health = 100;
    private Animator animation;

    //movement variables
    void Start()
    {
        sprite = GetComponentInChildren<SpriteRenderer>().gameObject;
        animation = sprite.GetComponent<Animator>();
        forces = GetComponent<Rigidbody2D>();
        nextMoveTime = Random.Range(1f, moveTimeInterval);
    }
    

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time > nextMoveTime)
        {
            nextMoveTime = Random.Range(1f, moveTimeInterval);
            Action();
            time = 0;
        }
        if (health <= 0)
        {
            Destroy(this.gameObject,1f);
        }
    }
    private void Action()
    {
        switch (Random.Range(0, 2))
        {
            case 0:
                forces.AddForceX(forcesApplied, ForceMode2D.Impulse);
                animation.Play("run");
                sprite.transform.localScale = new Vector2(5, sprite.transform.localScale.y);
                break;
            case 1:
                forces.AddForceX(-forcesApplied, ForceMode2D.Impulse);
                animation.Play("run");
                sprite.transform.localScale = new Vector2(-5, sprite.transform.localScale.y);
                
                break;
            default: // ono.
                break;
        }
    }
    
    public bool CanBeDamaged()
    {
        return true;
    }
    public bool HandleDamage(DamageSource source, int damage)
    {
        Debug.Log("damage");
        health -= damage;
        return true;
    }
}
