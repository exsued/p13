using System.Linq;
using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public enum Emotions
{
    Despair,
    transientState,
    neutral
}
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(VideoPlayer))]

public class PlayerCam : MonoBehaviour
{
    //Camera bob
    public float walkingBobbingSpeed = 7f;
    public float runningBobbingSpeed = 14f;
    public float bobbingAmount = 0.05f;
    float defaultPosY = 0;
    float defaultPosX = 0;
    float timer = 0;

    //Emotions
    Emotions grayEmotions = Emotions.neutral;

    //MouseLook
    public bool YLockRotate = false;
    public bool XLockRotate = false;
    public float sensitive = 15f;
    public float Ymax = 60f;
    public float Ymin = 60f;
    public float Xmax = 190f;
    public float Xmin = 60f;
    public bool isLookingAt { get; private set; }

    Camera cam;
    public float defCameraVOF = 60f;
    public float minCameraVOF = 50f;
    public float maxCameraVOF = 75f;
    float FOVForOneDegree = 0f;

    Transform player;
    float x = 0;
    float y = 0;
    Quaternion origRotation;
    Quaternion playerOrigRotation;
    
    //Enemy gallutinations
    VideoPlayer fractalEffects;
    public float minFractalsAlpha = 0.1f;
    public float maxFractalsAlpha = 0.6f;
    float alphaForOneDegree = 0f;
    public float yBug, xBug;


    //Camera shake
    public enum ShakeMode { OnlyX, OnlyY, OnlyZ, XY, XZ, XYZ };

    private static Transform tr;
    public static float Power => i_Power;
    private static float elapsed, i_Duration, i_Power, percentComplete;
    private static ShakeMode i_Mode;
    private static Vector3 originalPos;

    public bool CursorActived
    {
        get => Cursor.lockState != CursorLockMode.Confined;
        set
        {
            Cursor.visible = value;
            if (value == true)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Confined;
        }
    }
    private void Start()
    {
        CursorActived = false;
        cam = GetComponent<Camera>();
        CameraBobStart();
        FractalsEffectStart();

        player = transform.root;
        playerOrigRotation = player.localRotation;
        origRotation = transform.localRotation;

        percentComplete = 1;
        tr = GetComponent<Transform>();
    }
    void Update()
    {
        CameraShakeEffect();
        if(percentComplete == 1)
            CameraBobUpdate();
        MouseLook();
        if(XLockRotate)
            x = player.eulerAngles.y;

        //EnemyEffectFunc();
    }
    IEnumerator PlayerEmotions()
    {
        switch (grayEmotions)
        {
            case Emotions.Despair:
                break;
            case Emotions.neutral:
                break;
        }
        yield break;
    }
    public void Shake(float duration, float power)
    {
        if (percentComplete == 1) originalPos = tr.localPosition;
        i_Mode = ShakeMode.XYZ;
        elapsed = 0;
        i_Duration = duration;
        i_Power = power;
    }
    public void AdditiveShake(float duration, float power)
    {
        if (percentComplete == 1) originalPos = tr.localPosition;
        i_Mode = ShakeMode.XYZ;
        elapsed = 0;
        i_Duration = duration;
        i_Power += power;
    }
    public void Shake(float duration, float power, ShakeMode mode)
    {
        if (percentComplete == 1) originalPos = tr.localPosition;
        i_Mode = mode;
        elapsed = 0;
        i_Duration = duration;
        i_Power = power;
    }
    void CameraShakeEffect()
    {
        if (elapsed < i_Duration)
        {
            elapsed += Time.deltaTime;
            percentComplete = elapsed / i_Duration;
            percentComplete = Mathf.Clamp01(percentComplete);
            Vector3 rnd = Random.insideUnitSphere * i_Power * (1f - percentComplete);
            switch (i_Mode)
            {
                case ShakeMode.XYZ:
                    tr.localPosition = originalPos + rnd;
                    break;
                case ShakeMode.OnlyX:
                    tr.localPosition = originalPos + new Vector3(rnd.x, 0, 0);
                    break;
                case ShakeMode.OnlyY:
                    tr.localPosition = originalPos + new Vector3(0, rnd.y, 0);
                    break;
                case ShakeMode.OnlyZ:
                    tr.localPosition = originalPos + new Vector3(0, 0, rnd.z);
                    break;
                case ShakeMode.XY:
                    tr.localPosition = originalPos + new Vector3(rnd.x, rnd.y, 0);
                    break;
                case ShakeMode.XZ:
                    tr.localPosition = originalPos + new Vector3(rnd.x, 0, rnd.z);
                    break;
            }
        }
        else
        {
            i_Power = 0f;
            i_Duration = 0f;
        }
    }
    void FractalsEffectStart()
    {
        FOVForOneDegree = (maxCameraVOF - minCameraVOF) / 180.0f;
        alphaForOneDegree = (maxFractalsAlpha - minFractalsAlpha) / 180.0f;
        fractalEffects = GetComponent<VideoPlayer>();
    }
    void CameraBobStart()
    {
        defaultPosY = transform.localPosition.y;
        defaultPosX = transform.localPosition.x;
    }
    void CameraBobUpdate()
    {
        if (Mathf.Abs(Player.instance.moveDirection.x) > 0.1f || Mathf.Abs(Player.instance.moveDirection.z) > 0.1f)
        {
            var curVelocity = Player.instance.controller.velocity.magnitude;
            //Player is moving
            switch (Player.instance.states)
            {
                case PlayerPosState.Walk:
                    timer += Time.deltaTime * walkingBobbingSpeed * curVelocity / Player.instance.walkSpeed;
                    break;
                case PlayerPosState.Run:
                    timer += Time.deltaTime * runningBobbingSpeed * curVelocity / Player.instance.runSpeed;
                    break;
            }
            transform.localPosition = new Vector3(defaultPosX + Mathf.Sin(timer * 0.5f) * bobbingAmount, defaultPosY + Mathf.Abs(Mathf.Cos(timer * 0.5f)) * bobbingAmount, transform.localPosition.z);
        }
        else
        {
            //Idle
            timer = 0;
            transform.localPosition = 
                new Vector3(
                Mathf.Lerp(transform.localPosition.x, defaultPosX, Time.deltaTime * walkingBobbingSpeed), 
                Mathf.Lerp(transform.localPosition.y, defaultPosY, Time.deltaTime * walkingBobbingSpeed),
                transform.localPosition.z);
        }
    }
    void EnemyEffectFunc()
    {
        if (ControllerNPC.spotted)
        {
            if (!fractalEffects.isPlaying)
                fractalEffects.Play();
            var angle = ControllerNPC.NPConScene.Min(x => Vector3.Angle(transform.forward, x.transform.position - transform.position));
            var fractalAlphaformule = alphaForOneDegree * (Mathf.Clamp(defCameraVOF - angle, 0f, 180f)) + minFractalsAlpha;
            var FOVformule = FOVForOneDegree * (angle) + minCameraVOF;
            fractalEffects.targetCameraAlpha = fractalAlphaformule;
            cam.fieldOfView = FOVformule;
        }
        else
        {
            if (fractalEffects.isPlaying)
                fractalEffects.Stop();
            fractalEffects.targetCameraAlpha = 0.0f;
            cam.fieldOfView = defCameraVOF;
        }
    }
    void MouseLook()
    {
        if (!XLockRotate)
        {
            x += Input.GetAxis("Mouse X") * sensitive;
            x = ClampAngle(x, Xmin, Xmax);
            Quaternion xQuaternion = Quaternion.AngleAxis(x, Vector3.up);
            player.localRotation = playerOrigRotation * xQuaternion;
        }
        if (!YLockRotate)
        {
            y += Input.GetAxis("Mouse Y") * sensitive;
            y = ClampAngle(y, Ymin, Ymax);
            Quaternion yQuaternion = Quaternion.AngleAxis(y, -Vector3.right);
            transform.localRotation = origRotation * yQuaternion;
        }
    }
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
    public void StartLookAt(Quaternion playerRotation, float yAngle, float lerpSpeed = 10f, float epsilon = 0.1f)
    {
        StartCoroutine(LookAt(playerRotation, yAngle, true, true, lerpSpeed, epsilon));
    }
    public void StartLookAtY(float yAngle, float lerpSpeed = 10f, float epsilon = 0.1f)
    {
        StartCoroutine(LookAt(Quaternion.identity, yAngle, false, true, lerpSpeed, epsilon));
    }
    public void StartLookAtX(Quaternion rotation, float lerpSpeed = 10f, float epsilon = 0.1f)
    {
        StartCoroutine(LookAt(rotation, 0f, true, false, lerpSpeed, epsilon));
    }

    IEnumerator LookAt(Quaternion playerRotation, float yAngle, bool checkX, bool checkY, float lerpSpeed = 10f, float epsilon = 0.1f)
    {
        if (isLookingAt)
            yield break;
        XLockRotate = YLockRotate = true;
        isLookingAt = true;
        while (isLookingAt)
        {
            var yAng = Quaternion.Euler(yAngle, 0f, 0f);

            if (checkX)
                player.localRotation = Quaternion.Slerp(player.localRotation, playerRotation, Time.deltaTime * lerpSpeed);
            if (checkY)
                transform.localRotation = Quaternion.Slerp(transform.localRotation, yAng, Time.deltaTime * lerpSpeed);
           yield return new WaitForEndOfFrame();
        }
        isLookingAt = false;
        XLockRotate = YLockRotate = false;
    }
    public void StopLookAt()
    {
        isLookingAt = false;
    }
}
