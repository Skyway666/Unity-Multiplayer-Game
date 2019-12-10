using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(1, 0, 0), 90 * Time.deltaTime);
        transform.position += new Vector3(0, 3 * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision!");
        Destroy(gameObject);
        Destroy(other.gameObject);
    }
}
