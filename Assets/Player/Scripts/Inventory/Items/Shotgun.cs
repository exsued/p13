using System.Collections;
using UnityEngine;

public class Shotgun : Item
{
    [SerializeField] GameObject concreteHole = null;
    [SerializeField] GameObject bloodHole = null;

    [SerializeField] float minRange, maxRange;
    public float shotPeriod = 2f;
    public ushort bulletsLeft = 100;
    [SerializeField] Animator animator = null;

    private FMOD.Studio.EventInstance sound;
    Transform playerCam;

    float timer;
    IEnumerator Start()
    {
        yield return new WaitUntil(() => Player.instance != null);
        playerCam = Player.instance.alignCamera.transform;
    }

    private void OnEnable()
    {
        animator.Play("TakeOn");
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryShoot();
        }
        if (playerCam != null)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(playerCam.forward), Time.deltaTime * 10f);
        }

    }
    void TryShoot()
    {
        if (bulletsLeft > 0 && timer < Time.time)
        {
            var playerCam = Player.instance.alignCamera.transform;
            RaycastHit hit1;
            int balls = Random.Range(4, 6);

            timer = Time.time + shotPeriod;

            string soundPath = "event:/ShotgunFire";
            sound = FMODUnity.RuntimeManager.CreateInstance(soundPath);
            sound.start();
            sound.release();
            animator.PlayInFixedTime("Shotgun", -1, 0);

            for (int i = 0; i < balls; i++)
            {
                if (Physics.Raycast(
                    playerCam.position,
                    Vector3.Lerp(playerCam.forward, playerCam.up * Random.Range(0f, 1f) + playerCam.right * Random.Range(0f, 1f),
                    Random.Range(minRange, maxRange)), out hit1)
                    && hit1.transform.tag != "Player")
                {
                    GameObject go;
                    switch (hit1.transform.tag)
                    {
                        case "Enemy":
                            go = Instantiate(bloodHole, hit1.point + hit1.normal * 0.001f, Quaternion.LookRotation(hit1.normal), hit1.transform);
                            Destroy(go, 0.5f);
                            break;
                        default:
                            go = Instantiate(concreteHole, hit1.point + hit1.normal * 0.001f, Quaternion.LookRotation(-hit1.normal), hit1.transform);
                            Destroy(go.gameObject, 15f);
                            break;
                    }
            }
            }
        }
    }
}
