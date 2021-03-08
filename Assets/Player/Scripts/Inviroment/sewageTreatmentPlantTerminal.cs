using UnityEngine;
using System.Collections;

public class sewageTreatmentPlantTerminal : MonoBehaviour, Interactable
{
    public Canvas monitor;
    public Transform workPoint; //В какой позиции игрок работает
    public Transform playerLookPoint;
    public float camAngle;

    public GameObject loadingWindow;

    private FMOD.Studio.EventInstance audioSource;

    void Start()
    {
        monitor.gameObject.SetActive(false);
    }
    public void OnTeslaActivated()
    {
        var soundPath = "event:/teslaOn";
        PlayAudio(soundPath);
    }
    public void OnTeslaDeactivated()
    {
        var soundPath = "event:/teslaOff";
        PlayAudio(soundPath);
    }
    void PlayAudio(string soundPath)
    {
        StartCoroutine(LoadingWindow(3f));
        audioSource = FMODUnity.RuntimeManager.CreateInstance(soundPath);
        audioSource.start();
        audioSource.release();
    }
    public void Interact()
    {
        StartCoroutine(PlayerWorkWithTerminal());
    }
    IEnumerator LoadingWindow(float time)
    {
        loadingWindow.SetActive(true);
        yield return new WaitForSeconds(time);
        loadingWindow.SetActive(false);
    }
    IEnumerator PlayerWorkWithTerminal()
    {
        var player = Player.instance;
        var playerCam = player.alignCamera;

        Player.instance.enabled = false;

        playerCam.XLockRotate = playerCam.YLockRotate = true;
        playerCam.StartLookAt(workPoint.rotation, camAngle);

        yield return StartCoroutine(Player.instance.TranslateAtPosition(workPoint));
        monitor.gameObject.SetActive(true);
        playerCam.CursorActived = true;
        while (Input.GetKey(KeyCode.F))
        {
            yield return null;
        }
        while (!Input.GetKey(KeyCode.F))
        {
            yield return null;
        }
        playerCam.CursorActived = false;
        monitor.gameObject.SetActive(false);
        Player.instance.enabled = true;
        playerCam.StopLookAt();
        playerCam.XLockRotate = playerCam.YLockRotate = false;
    }
}
