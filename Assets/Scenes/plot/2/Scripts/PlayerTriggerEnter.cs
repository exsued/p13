using UnityEngine;
using UnityEngine.Events;

public class PlayerTriggerEnter : MonoBehaviour
{
    public UnityEvent[] events;
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            foreach (var ev in events)
                ev.Invoke();
        }
    }
}
