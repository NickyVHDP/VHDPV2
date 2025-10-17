#nullable enable
using System;
using System.Collections;
using UnityEngine;

namespace VHDPV2.Player
{
    public sealed class DashController : MonoBehaviour
    {
        [SerializeField] private float dashCooldown = 3f;
        [SerializeField] private float dashDuration = 0.2f;
        [SerializeField] private float dashSpeedMultiplier = 3f;
        [SerializeField] private float invulnerabilityDuration = 0.2f;
        [SerializeField] private AnimationCurve dashEase = AnimationCurve.Linear(0, 1, 1, 1);

        private float _cooldownTimer;
        private bool _isDashing;
        private Coroutine? _dashRoutine;

        public bool IsDashing => _isDashing;
        public float CurrentSpeedMultiplier { get; private set; } = 1f;

        public event Action? DashStarted;
        public event Action? DashEnded;

        private void Update()
        {
            if (_cooldownTimer > 0f)
            {
                _cooldownTimer -= Time.deltaTime;
            }
        }

        public bool TryDash(Vector2 direction)
        {
            if (_isDashing || _cooldownTimer > 0f || direction.sqrMagnitude <= float.Epsilon)
            {
                return false;
            }

            _dashRoutine = StartCoroutine(DashRoutine());
            return true;
        }

        private IEnumerator DashRoutine()
        {
            _isDashing = true;
            DashStarted?.Invoke();
            _cooldownTimer = dashCooldown;
            float elapsed = 0f;
            CurrentSpeedMultiplier = dashSpeedMultiplier;
            var damageReceiver = GetComponent<Damage.DamageReceiver>();
            if (damageReceiver != null)
            {
                damageReceiver.SetInvulnerable(true);
            }

            while (elapsed < dashDuration)
            {
                elapsed += Time.deltaTime;
                float normalized = Mathf.Clamp01(elapsed / dashDuration);
                CurrentSpeedMultiplier = Mathf.Lerp(dashSpeedMultiplier, 1f, dashEase.Evaluate(normalized));
                yield return null;
            }

            CurrentSpeedMultiplier = 1f;
            if (damageReceiver != null)
            {
                yield return new WaitForSeconds(invulnerabilityDuration);
                damageReceiver.SetInvulnerable(false);
            }

            _isDashing = false;
            DashEnded?.Invoke();
        }

        private void OnDisable()
        {
            if (_dashRoutine != null)
            {
                StopCoroutine(_dashRoutine);
                _dashRoutine = null;
            }

            _isDashing = false;
            CurrentSpeedMultiplier = 1f;
        }
    }
}
