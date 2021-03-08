using UnityEngine;

public class InfinityRoom : MonoBehaviour
{
    [SerializeField] Vector3 OffsetVector = new Vector3(0f, 5f, 0f);
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Player.instance.controller.enabled = false;
            other.transform.position = other.transform.position - OffsetVector;
            Player.instance.controller.enabled = true;
        }
    }
}
