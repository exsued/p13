using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] string soundPath = "event:/PlayerSteps/Player";
    private FMOD.Studio.EventInstance foosteps;

    void OnTriggerEnter(Collider body)
    {
        if(body.tag == "Player")
        {
            foosteps = FMODUnity.RuntimeManager.CreateInstance(soundPath);
            foosteps.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(body.transform.position));
            foosteps.start();
            foosteps.release();
        }
    }
}
