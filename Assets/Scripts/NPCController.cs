using UnityEngine;

public class NPCController : MonoBehaviour, Damageable, Protectable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private float time;
    private float nextMoveTime;
    private Rigidbody2D forces;
    public float forcesApplied = 10;
    public float health = 100;
    private Animator animation;
    private float protectionTime = 0;

    [SerializeField]
    GameObject particles;

    //movement variables
    void Start()
    {
        animation = GetComponent<Animator>();
        forces = GetComponent<Rigidbody2D>();
        nextMoveTime = Random.Range(1f, 10f);
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
        if (time > nextMoveTime)
        {
            nextMoveTime = Random.Range(1f, 10f);
            Action();
            time = 0;
        }
        if (health <= 0)
        {
            animation.Play("Die");
            Destroy(this.gameObject,1f);
        }
    }
    private void Action()
    {
        switch (Random.Range(0, 2))
        {
            case 0:
                forces.AddForceX(forcesApplied, ForceMode2D.Impulse);
                animation.Play("Right");
                break;
            case 1:
                forces.AddForceX(-forcesApplied, ForceMode2D.Impulse);
                animation.Play("Left");
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
        if (source.GetSource().CompareTag("Player"))
        {
            //Debug.Log("hit by Player");
        }
        else if (source.GetSource().CompareTag("Enemy"))
        {
            //Debug.Log("hit by enemy");
        }
        health -= damage;
        return true;
    }

    public void SetProtection(float duration)
    {
        protectionTime = duration;
    }
}
