using UnityEngine;
using UnityEngine.Events;

public class PlayerTrigger : MonoBehaviour
{
    public UnityEvent onEntered = null;

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            onEntered.Invoke();
        }
    }
}
