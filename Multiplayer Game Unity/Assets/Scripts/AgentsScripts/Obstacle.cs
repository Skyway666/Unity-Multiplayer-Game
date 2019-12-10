using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    void Update()
    {
        transform.position += new Vector3(0, 3 * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {
            Destroy(gameObject);
            // Substract points to other.GameObject
            Debug.Log("Points substracted from player");
        }

        if(other.gameObject.tag == "Bullet")
        {
            Destroy(other.gameObject);

            Debug.Log("Destroyed bullet against obstacle");
        }

    }
}
