using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class BoatLogic : AgentLogic
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag.Equals("Box"))
        {
            points += 2.0f;
            Destroy(other.gameObject);
        }
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag.Equals("Enemy"))
        {
            points -= 100.0f;
        }
    }
}
