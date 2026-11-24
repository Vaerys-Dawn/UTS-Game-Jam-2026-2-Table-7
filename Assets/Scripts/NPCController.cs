using UnityEngine;

public class NPCController : MonoBehaviour, Damageable, Protectable
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
    private bool isDead = false;
    private float protectionTime = 0;

    [SerializeField]
    GameObject particles;

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
        if (protectionTime > 0)
        {
            protectionTime -= Time.deltaTime;
        }
        particles.SetActive(protectionTime > 0);
        time += Time.deltaTime;
        print(health);
        if (time > nextMoveTime)
        {
            nextMoveTime = Random.Range(0f, moveTimeInterval);
            Action();
            time = 0;
        }
        if (health <= 0 && !isDead)
        {
            isDead = true; // only play the death sound and start the destroy once
            AudioManager.Play(AudioManager.Sfx.NpcDeath);
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
        if (protectionTime > 0) return false;
        Debug.Log("damage");
        health -= damage;
        return true;
    }

    public void SetProtection(float duration)
    {
        protectionTime = duration;
    }
}
