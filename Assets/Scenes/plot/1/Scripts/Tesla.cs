using UnityEngine;

public class Tesla : MonoBehaviour
{
    public GameObject Bold = null;
    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            Bold.SetActive(true);
            Player.instance.GetElectricDamage(transform, Time.deltaTime * 6f);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
            Bold.SetActive(false);
    }
}
