using Cinemachine;
using System;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private TextMeshPro _nameText;
    [SerializeField] private CinemachineVirtualCamera _followCam;
    
    public static event Action<Player> OnPlayerSpawned;
    public static event Action<Player> OnPlayerDespawned;

    public Health HealthCompo { get; private set; }

    private NetworkVariable<FixedString32Bytes> _username = new NetworkVariable<FixedString32Bytes>();

	private void Awake()
	{
		HealthCompo = GetComponent<Health>();
        HealthCompo.OnDie += HandleDie;
	}

	private void HandleDie(Health health)
	{
        ulong killerID = health.LastHitDealerID;
        Destroy(gameObject);
	}

    public override void OnNetworkSpawn()
    {
        _username.OnValueChanged += HandleNameChanged;
        HandleNameChanged("", _username.Value);
        if(IsOwner)
        {
            _followCam.Priority = 15;
        }

        if (IsServer)
		{
            OnPlayerSpawned?.Invoke(this);
		}
    }

	public override void OnNetworkDespawn()
	{
        _username.OnValueChanged -= HandleNameChanged;
        if (IsServer)
		{
            OnPlayerDespawned?.Invoke(this);
		}
	}

	private void HandleNameChanged(FixedString32Bytes prev, FixedString32Bytes newValue)
    {
        _nameText.text = newValue.ToString();
    }

    public void SetUserName(string username)
    {
        _username.Value = username;
    }
}
