using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RespawnManager : NetworkBehaviour
{
	[SerializeField] private RankboardBehaviour rankboard;

	public override void OnNetworkSpawn()
	{
		if (!IsServer) return;

		Player.OnPlayerDespawned += HandlePlayerDespawn;
	}

	public override void OnNetworkDespawn()
	{
		Player.OnPlayerDespawned -= HandlePlayerDespawn;
	}

	private void HandlePlayerDespawn(Player player)
	{
		ulong killerID = player.HealthCompo.LastHitDealerID;
		UserData killerUserData = ServerSingleton.Instance.NetServer.GetUserDataByClientID(killerID);
		UserData diedUserData = ServerSingleton.Instance.NetServer.GetUserDataByClientID(player.OwnerClientId);

		if (diedUserData != null)
		{
			Debug.Log($"{diedUserData.username}´ÔÀÌ Á×¾ú½À´Ï´Ù. (By {killerUserData.username})");
		}

		if (IsServer)
			rankboard.HandleAddScore(killerID, 1);
		StartCoroutine(DelayRespawn(player.OwnerClientId));
	}

	IEnumerator DelayRespawn(ulong clientID)
	{
		yield return new WaitForSeconds(3f);

		ServerSingleton.Instance.NetServer.RespawnPlayer(clientID);
	}
}
