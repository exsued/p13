using UnityEngine;

public class Optimization : MonoBehaviour
{
    public GameObject[] HideOnEnter = null;
    public GameObject[] ShowOnEnter = null;

    void Start()
    {
        foreach (var item in HideOnEnter)
            item.SetActive(true);
        foreach (var item in ShowOnEnter)
            item.SetActive(false);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            foreach (var item in HideOnEnter)
                item.SetActive(false);
            foreach (var item in ShowOnEnter)
                item.SetActive(true);
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            foreach (var item in HideOnEnter)
                item.SetActive(true);
            foreach (var item in ShowOnEnter)
                item.SetActive(false);
        }
    }
}
