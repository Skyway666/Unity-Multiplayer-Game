using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectiveType
{
    Shootable,
    Collectable

}
public class Objective : MonoBehaviour
{
    // Start is called before the first frame update

    public ObjectiveType type = ObjectiveType.Shootable;

    public float maxSpeed = 0;
    public float minSpeed = 0;
    float speed = 3;

    private void Start()
    {
        speed = Random.Range(minSpeed, maxSpeed);
    }
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(1, 0, 0), 90 * Time.deltaTime);
        transform.position += new Vector3(0, speed * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        // A shootable object has been shot
        if(type == ObjectiveType.Shootable && other.gameObject.tag == "Bullet" )
        {
            Destroy(gameObject);
            Destroy(other.gameObject);

            Debug.Log("Points added to player");
        }
        if (type == ObjectiveType.Collectable && other.gameObject.tag == "Player")
        {
            Destroy(gameObject);
            // Give points to other.GameObject

            Debug.Log("Lots of points added to player");
        }

    }
}
