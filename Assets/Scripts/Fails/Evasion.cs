//using UnityEngine;

//public class PlayerCombatMechanics : MonoBehaviour
//{
//    [Header("Roll Settings")]
//    public float rollSpeed = 8f;
//    public float rollDuration = 0.5f;
//    public float rollCooldown = 0.8f;

//    [Header("Parry Settings")]
//    public float parryDuration = 0.25f;
//    public float parryCooldown = 1f;
//    public float parryGuardDuration = 1.5f; // Máximo tiempo en posición de guardia

//    // Referencias
//    private PlayerMovement playerMovement;
//    private Rigidbody2D rb;
//    private Animator animator;

//    // Estados de Roll
//    private bool isRolling = false;
//    private float rollCooldownTimer = 0f;
//    private float rollEndTime = 0f;
//    private int rollDirection = 1;

//    // Estados de Parry
//    private bool isParrying = false;
//    private bool isGuarding = false;
//    private float parryEndTime = 0f;
//    private float parryCooldownTimer = 0f;
//    private float guardEndTime = 0f;

//    private void Awake()
//    {
//        playerMovement = GetComponent<PlayerMovement>();
//        rb = GetComponent<Rigidbody2D>();
//        animator = GetComponent<Animator>();
//    }

//    private void Update()
//    {
//        // Manejo de cooldowns
//        rollCooldownTimer -= Time.deltaTime;
//        parryCooldownTimer -= Time.deltaTime;

//        // Si está en roll, verifica si debe terminar
//        if (isRolling && Time.time >= rollEndTime)
//        {
//            EndRoll();
//        }

//        // Si está en parry, verifica si debe terminar
//        if (isParrying && Time.time >= parryEndTime)
//        {
//            EndParry();
//        }

//        // Si está en guardia, verifica si debe terminar por timeout
//        if (isGuarding && Time.time >= guardEndTime)
//        {
//            EndGuard();
//        }

//        // No permitir más acciones durante roll
//        if (isRolling)
//            return;

//        // Lógica de Roll (Shift)
//        if (Input.GetKeyDown(KeyCode.LeftShift) && rollCooldownTimer <= 0 && !isParrying && !isGuarding)
//        {
//            StartRoll();
//        }

//        // Lógica de Parry (clic derecho)
//        if (Input.GetMouseButtonDown(1) && parryCooldownTimer <= 0 && !isRolling)
//        {
//            StartParry();
//        }
//        // Mantener guardia mientras se mantiene pulsado
//        else if (isGuarding)
//        {
//            // Finalizar guardia al soltar el botón
//            if (Input.GetMouseButtonUp(1))
//            {
//                EndGuard();
//            }
//        }
//    }

//    private void StartRoll()
//    {
//        isRolling = true;
//        rollEndTime = Time.time + rollDuration;
//        rollCooldownTimer = rollCooldown;

//        // Determinar dirección del roll
//        float horizontalInput = Input.GetAxis("Horizontal");
//        rollDirection = (int)(horizontalInput != 0 ? Mathf.Sign(horizontalInput) : (playerMovement.IsFacingRight() ? 1 : -1));

//        // Iniciar estado de Roll en PlayerMovement
//        playerMovement.ChangeState(PlayerMovement.PlayerState.Roll);
//    }

//    private void EndRoll()
//    {
//        isRolling = false;

//        // Restaurar estado en PlayerMovement
//        playerMovement.ResetState();
//    }

//    private void StartParry()
//    {
//        isParrying = true;
//        isGuarding = true;
//        parryEndTime = Time.time + parryDuration;
//        guardEndTime = Time.time + parryGuardDuration;
//        parryCooldownTimer = parryCooldown;

//        // Iniciar estado de Parry en PlayerMovement
//        playerMovement.ChangeState(PlayerMovement.PlayerState.Parry);
//    }

//    private void EndParry()
//    {
//        isParrying = false;
//        // Nota: No termina la guardia aquí, solo la fase activa del parry
//    }

//    private void EndGuard()
//    {
//        isGuarding = false;

//        // Restaurar estado en PlayerMovement
//        playerMovement.ResetState();
//    }

//    private void FixedUpdate()
//    {
//        // Aplicar movimiento durante el roll
//        if (isRolling)
//        {
//            rb.linearVelocity = new Vector2(rollDirection * rollSpeed, rb.linearVelocity.y);
//        }
//    }
//}