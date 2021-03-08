using UnityEngine;
using System.Collections;

public enum PlayerPosState
{
    Crouch,
    Walk,
    Run
}
[RequireComponent(typeof(CharacterController))]

public class Player : MonoBehaviour
{
    public static Player instance = null;

    //Eyes
    public Light nightVision = null;

    //Movement
    public CharacterController controller { get; private set; }
    public Vector3 moveDirection { get; private set; }
    public PlayerPosState states { get; private set; } = PlayerPosState.Walk;

    public float walkSpeed = 5f,
                 runSpeed = 8f,
                 crouchSpeed = 3f;
    public LayerMask groundMask;
    public LayerMask ceilingMask;
    public LayerMask interactableMask;

    public float maxInteractDistance = 4f;
    public float StandHeight => standHeight;
    public float CrouchHeight => crouchHeight;

    private float standHeight, crouchHeight, stepLength;
    private float timer;
    private FMOD.Studio.EventInstance foosteps;

    RaycastHit stepHit;
    RaycastHit interactHit;

    public Transform Feet;

    public PlayerCam alignCamera { get; private set; }

    public Item UsableItem = null;

    private void OnDisable()
    {
        states = PlayerPosState.Walk;
        moveDirection = Vector3.zero;
        controllerHeight = standHeight;
    }
    private void OnEnable()
    {
        states = PlayerPosState.Walk;
        if(alignCamera != null)
        alignCamera.XLockRotate = false;
    }
    public float controllerHeight
    {
        get => controller.height;
        set
        {
            controller.center = Vector3.up * value / 2f;
            //controller.Move(Vector3.up * (value - controller.height));
            controller.height = value;
        }
    }

    private void PlayFootstep()
    {
        if (stepHit.transform == null)
            return;
        string soundPath = "event:/PlayerSteps/Player";
        switch (states)
        {
            case PlayerPosState.Walk:
                soundPath += "Walk";
                break;
            case PlayerPosState.Run:
                soundPath += "Run";
                break;
            case PlayerPosState.Crouch:
                soundPath = "event:/PlayerCrouchSteps";
                break;
        }
        switch(stepHit.transform.tag)
        {
            case "Grass":
                soundPath += "Grass";
                break;
            case "Untagged":
                soundPath += "Coridor";
                break;
            case "Concrete":
                soundPath += "Concrete";
                break;
            case "Water":
                soundPath += "Water";
                break;
        }
        foosteps = FMODUnity.RuntimeManager.CreateInstance(soundPath);
        foosteps.start();
        foosteps.release();
    }

    private void TryPlayFootstep(float curSpeed)
    {
        if (timer < Time.time)
        {
            PlayFootstep();
            timer = Time.time + stepLength / curSpeed;
        }
    }
    private void Start()
    {
        instance = this;
        controller = GetComponent<CharacterController>();
        standHeight = controllerHeight;
        crouchHeight = controllerHeight / 2f;
        stepLength = walkSpeed * 0.8f;  //Ширина шага

        alignCamera = GetComponentInChildren<PlayerCam>();
    }
    private bool CanCrouch()
    {
        RaycastHit hit2;
        Physics.Raycast(Feet.position, Vector3.down, out hit2, standHeight, groundMask);

        return !(hit2.transform == null || hit2.point.y - transform.position.y > crouchHeight);
    }
    private bool CanStand()
    {
        var r = !Physics.Raycast(transform.position, Vector3.up, standHeight, ceilingMask);
        return r;
    }
    private bool IsGrounded()
    {
        return Physics.Raycast(Feet.position, Vector3.down, out stepHit, controllerHeight, groundMask);
    }
    private void Update()
    {
        MoveUpdate();
        InteractUpdate();
    }
    private void CheckPosState()
    {
        if (Input.GetButton("Crouch") && CanCrouch())
            states = PlayerPosState.Crouch;
        else
        {
            if (states == PlayerPosState.Crouch)
                if (!CanStand()) return;
            if (Input.GetButton("Run"))
                states = PlayerPosState.Run;
            else
                states = PlayerPosState.Walk;
        }
    }
    private void MoveUpdate()
    {
        CheckPosState();
        //Falling system
        IsGrounded();
        controller.Move(Physics.gravity * Time.deltaTime);

        var motion = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        moveDirection = transform.TransformDirection(motion).normalized;    //From world to local space

        float curSpeed = 0f;
        switch (states)
        {
            case PlayerPosState.Walk:
                curSpeed = walkSpeed;
                controllerHeight = standHeight;
                break;
            case PlayerPosState.Crouch:
                curSpeed = crouchSpeed;
                controllerHeight = crouchHeight;
                break;
            case PlayerPosState.Run:
                curSpeed = runSpeed;
                controllerHeight = standHeight;
                break;
        }
        controller.Move(moveDirection * curSpeed * Time.deltaTime);
        var curVel = controller.velocity.magnitude;
        if (curVel > curSpeed / 2f)
            TryPlayFootstep(curSpeed * curVel / curSpeed);
    }
    private void InteractUpdate()
    {
        var camTrans = alignCamera.transform;
        if (Physics.Raycast(camTrans.position, camTrans.forward, out interactHit, maxInteractDistance, interactableMask))
        {
            var interactable = interactHit.transform.GetComponent<Interactable>();
            if (interactable == null)
                return;
            if (Input.GetKeyDown(KeyCode.F))
                interactable.Interact();
        }
    }

    public void GetElectricDamage(Transform source, float force = 1f)
    {
        alignCamera.Shake(0.1f, 0.3f);
        controller.Move((transform.position - source.position).normalized * force);
    }
    public IEnumerator TranslateAtPosition(Transform point, float lerpSpeed = 10f, float epsilon = 0.1f)
    {
        while (Vector3.Distance(transform.position, point.position) > epsilon)
        {
            transform.position = Vector3.Lerp(transform.position, point.position, Time.deltaTime * lerpSpeed);
            yield return new WaitForEndOfFrame();
        }
        transform.position = point.position;
    }
    public IEnumerator TranslateAtPosition(Vector3 pos, float lerpSpeed = 10f, float epsilon = 0.1f)
    {
        while (Vector3.Distance(transform.position, pos) > epsilon)
        {
            transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * lerpSpeed);
            yield return new WaitForEndOfFrame();
        }
        transform.position = pos;
    }
}
