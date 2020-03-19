using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SenseLogic : MonoBehaviour
{
    [SerializeField]
    private BoatLogic notifier;

    [SerializeField, Range(0.0f, 1.0f)]
    private float influence = 0.5f;

    // private void OnTriggerEnter(Collider other)
    // {
    //     notifier.NotifiedBySense(other.gameObject, influence);
    // }
    //
    // private void OnTriggerStay(Collider other)
    // {
    //     notifier.NotifiedBySense(other.gameObject, influence);
    // }

    /**
     * Change the influence to the new value passed as a parameter. Value will be limited to be between 0% and 100%.
     */
    public void ChangeInfluence(float newInfluence)
    {
        influence = Mathf.Min(1.0f, Mathf.Max(0.0f, newInfluence));
    }
}
