#nullable enable
using System;
using UnityEngine;
using VHDPV2.Core;
using VHDPV2.Pickups;
using VHDPV2.Systems;

namespace VHDPV2.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerStats baseStats = new();
        [SerializeField] private DashController? dashController;
        [SerializeField] private PickupCollector? pickupCollector;

        private Rigidbody2D? _body;
        private StatCollection? _runtimeStats;
        private Vector2 _movementInput;

        public event Action<Vector2>? OnMove;

        public StatCollection Stats => _runtimeStats ??= baseStats.BuildStatCollection();

        private void Awake()
        {
            _body = GetComponent<Rigidbody2D>();
            dashController ??= GetComponent<DashController>();
            pickupCollector ??= GetComponent<PickupCollector>();
        }

        private void OnEnable()
        {
            pickupCollector?.SetMagnetMultiplier(Stats.GetValue(StatType.Magnet, 1f));
        }

        private void Update()
        {
            ReadInput();
            HandleDash();
        }

        private void FixedUpdate()
        {
            if (_body == null)
            {
                return;
            }

            float speed = Stats.GetValue(StatType.MoveSpeed, baseStats.MoveSpeed);
            Vector2 velocity = _movementInput.normalized * speed;
            if (dashController != null && dashController.IsDashing)
            {
                velocity *= dashController.CurrentSpeedMultiplier;
            }

            _body.velocity = velocity;
            OnMove?.Invoke(velocity);
        }

        private void ReadInput()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            _movementInput = new Vector2(horizontal, vertical);
            _movementInput = Vector2.ClampMagnitude(_movementInput, 1f);
        }

        private void HandleDash()
        {
            if (dashController == null)
            {
                return;
            }

            if (Input.GetButtonDown("Jump"))
            {
                dashController.TryDash(_movementInput);
            }
        }
    }
}
