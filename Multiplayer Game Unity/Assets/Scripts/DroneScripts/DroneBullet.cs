using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneBullet : MonoBehaviour
{

    public float speed = 100.0f;
    public float life = 4.0f;

    float spawnedTime = 0;

    void Start()
    {
        spawnedTime = Time.time;
    }
    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        if (Time.time - spawnedTime > life)
            Destroy(gameObject);
    }
}
