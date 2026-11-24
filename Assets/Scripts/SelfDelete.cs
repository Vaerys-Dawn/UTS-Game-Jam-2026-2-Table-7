using UnityEngine;

public class SelfDelete : MonoBehaviour
{
    [SerializeField]
    private float liveTime = 0.25f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (liveTime >0)
        {
            liveTime-= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
