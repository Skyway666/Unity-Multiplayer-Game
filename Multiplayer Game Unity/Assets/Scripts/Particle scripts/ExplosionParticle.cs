using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Emmit noise and destroy itself over time
public class ExplosionParticle : MonoBehaviour
{
    float life = 1; // Seconds
    float timeCreated = 0;
    // Start is called before the first frame update
    void Start()
    {
        timeCreated = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - timeCreated >= life)
              Destroy(gameObject);
    }
}
