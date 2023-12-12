using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float dashCooltime = 2f;
    [SerializeField] private float dashSpeed = 5f;
    private float lastDashTime = 0;
    private bool isDashing = false;
    private Vector2 dashDir;

    private Vector2 _movementInput;
    private Rigidbody2D _rigidbody2D;
    private PlayerAnimation _playerAnimation;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _playerAnimation = transform.Find("Visual").GetComponent<PlayerAnimation>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        _inputReader.MovementEvent += HandleMovement;
        _inputReader.DashEvent += HandleDash;
    }

	public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        _inputReader.MovementEvent -= HandleMovement;
        _inputReader.DashEvent -= HandleDash;
    }

    private void HandleMovement(Vector2 movementInput)
    {
        _movementInput = movementInput;
    }

	private void HandleDash()
	{
        if (lastDashTime + dashCooltime > Time.time) return;
        lastDashTime = Time.time;
        if (_movementInput.sqrMagnitude > 0.02f)
		{
            dashDir = _movementInput.normalized;
		}
        else
		{
            dashDir = new Vector2(1f, 0);
		}
        StartCoroutine(DashingCo());
    }

    private IEnumerator DashingCo()
	{
        isDashing = true;
        yield return new WaitForSeconds(0.1f);
        isDashing = false;
	}

    private void FixedUpdate()
    {
        _playerAnimation.SetMove(_rigidbody2D.velocity.magnitude > 0.1f);
        _playerAnimation.FlipController( _rigidbody2D.velocity.x );

        if (!IsOwner) return;

        if (isDashing)
		{
            _rigidbody2D.velocity = dashDir * dashSpeed;
		}
        else
		{
            _rigidbody2D.velocity = _movementInput * _movementSpeed;
		}
    }
}
