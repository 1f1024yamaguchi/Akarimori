using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Refs")]
    public Transform cam;

    [Header("Move")]
    public float moveSpeed = 4f;
    public float sprintSpeed = 7f;
    public float mouseSensitivity = 1.5f;
    public float gravity = -20f;
    public float jumpHeight = 1.2f;

    [Header("Interaction")]
    public float interactDistance = 2f;
    public LayerMask interactMask;

    [Header("Head Bob")]
    public bool headBob = true;
    public float bobFrequency = 1.8f;
    public float bobWalkAmplitude = 0.03f;
    public float bobSprintAmplitude = 0.055f;
    public float bobSmooth = 10f;

    [Header("Stamina")]
    public float staminaMax = 100f;
    public float staminaRegenPerSec = 16f;
    public float staminaDrainPerSec = 25f;
    public float staminaRegenDelay = 0.6f;

    [Header("UI")]
    public CanvasGroup interactHintGroup;
    public TextMeshProUGUI interactHintText;
    public Image staminaFill;

    // jump
    public float coyoteTime = 0.12f;
    public float jumpBufferTime = 0.12f;

    CharacterController cc;
    PlayerInput pi;
    InputAction moveA, lookA, jumpA, sprintA, interactA;

    float pitch;
    Vector3 vel;
    float coyoteCounter, jumpBufferCounter;
    float baseStepOffset;
    float stamina;
    float staminaRegenClock;

    // shake
    Vector3 camDefaultLocalPos;
    float bobTimer;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        pi = GetComponent<PlayerInput>();
        baseStepOffset = cc.stepOffset;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        stamina = staminaMax;
        if (cam != null) camDefaultLocalPos = cam.localPosition;

        if (interactHintGroup) interactHintGroup.alpha = 0f;
    }

    void OnEnable()
    {
        var map = pi.actions;
        moveA = map["Move"];
        lookA = map["Look"];
        jumpA = map["Jump"];
        sprintA = map["Sprint"];
        interactA = map["Interact"];
    }

    void Update()
    {
        float dt = Time.deltaTime;

        // --- camera ---
        Vector2 look = lookA.ReadValue<Vector2>() * mouseSensitivity;
        pitch = Mathf.Clamp(pitch - look.y, -85f, 85f);
        if (cam) cam.localEulerAngles = new Vector3(pitch, 0f, 0f);
        transform.Rotate(0f, look.x, 0f);

        // --- move ---
        Vector2 m = moveA.ReadValue<Vector2>();
        Vector3 dir = (transform.forward * m.y + transform.right * m.x).normalized;
        bool grounded = cc.isGrounded;
        bool movingPlanar = m.sqrMagnitude > 0.01f;

        if (grounded)
        {
            coyoteCounter = coyoteTime;
            if (vel.y < 0f) vel.y = -2f;
        }
        else coyoteCounter -= dt;

        if (jumpA.WasPressedThisFrame()) jumpBufferCounter = jumpBufferTime;
        else jumpBufferCounter -= dt;

        if (coyoteCounter > 0f && jumpBufferCounter > 0f)
        {
            vel.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            coyoteCounter = 0f;
            jumpBufferCounter = 0f;
            grounded = false;
        }

        vel.y += gravity * dt;

        // --- rush ---
        bool wantsSprint = sprintA.IsPressed();
        bool canSprint = stamina > 0.01f;
        bool sprinting = wantsSprint && canSprint && movingPlanar && grounded;

        float speed = sprinting ? sprintSpeed : moveSpeed;

        if (sprinting)
        {
            stamina = Mathf.Max(0f, stamina - staminaDrainPerSec * dt);
            staminaRegenClock = 0f;
        }
        else
        {
            staminaRegenClock += dt;
            if (staminaRegenClock >= staminaRegenDelay)
                stamina = Mathf.Min(staminaMax, stamina + staminaRegenPerSec * dt);
        }
        if (staminaFill) staminaFill.fillAmount = stamina / staminaMax;

        // --- shake ---
        if (headBob && cam)
        {
            float amp = sprinting ? bobSprintAmplitude : bobWalkAmplitude;
            if (movingPlanar && grounded)
            {
                bobTimer += dt * bobFrequency * (sprinting ? 1.5f : 1f);
                // Minecraft
                Vector3 offset = new Vector3(
                    Mathf.Sin(bobTimer * 2f) * amp * 0.5f,
                    Mathf.Abs(Mathf.Sin(bobTimer)) * amp,
                    0f);
                cam.localPosition = Vector3.Lerp(cam.localPosition, camDefaultLocalPos + offset, dt * bobSmooth);
            }
            else
            {
                bobTimer = 0f;
                cam.localPosition = Vector3.Lerp(cam.localPosition, camDefaultLocalPos, dt * bobSmooth);
            }
        }

        Vector3 motion = dir * speed;
        motion.y += vel.y;
        cc.stepOffset = grounded ? baseStepOffset : 0f;
        cc.Move(motion * dt);

        // interact
        bool hasTarget = false;
        RaycastHit hit;
        Ignitable ign = null;
        if (Physics.SphereCast(cam.position, 0.2f, cam.forward, out hit, interactDistance, interactMask))
        {
            ign = hit.collider.GetComponentInParent<Ignitable>();
            if (ign != null && !ign.isLit)
            {
                hasTarget = true;
                if (interactA.WasPressedThisFrame()) ign.Ignite();
            }
        }
        UpdateInteractHint(hasTarget, dt);
    }

    void UpdateInteractHint(bool show, float dt)
    {
        if (!interactHintGroup) return;
        float target = show ? 1f : 0f;
        interactHintGroup.alpha = Mathf.MoveTowards(interactHintGroup.alpha, target, dt * 8f);
        interactHintGroup.blocksRaycasts = false;
        if (show && interactHintText)
            interactHintText.text = "「Eキーで松明を点灯」";
    }
}
