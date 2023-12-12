using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RankboardBehaviour : NetworkBehaviour
{
	[SerializeField] private RecordUI recordPrefab;
	[SerializeField] private RectTransform recordParentTrm;

	private NetworkList<RankboardEntityState> rankList;

	private List<RecordUI> rankUIList = new List<RecordUI>();

	private void Awake()
	{
		rankList = new NetworkList<RankboardEntityState>();
	}

	public override void OnNetworkSpawn()
	{
		if (IsClient)
		{
			rankList.OnListChanged += HandleRankListChanged;
			foreach (RankboardEntityState re in rankList)
			{
				HandleRankListChanged(new NetworkListEvent<RankboardEntityState>
				{
					Type = NetworkListEvent<RankboardEntityState>.EventType.Add,
					Value = re
				});
			}
		}

		if (IsServer)
		{
			ServerSingleton.Instance.NetServer.OnUserJoin += HandleUserJoin;
			ServerSingleton.Instance.NetServer.OnUserLeft += HandleUserLeft;
		}
	}

	public override void OnNetworkDespawn()
	{
		if (IsClient)
		{
			rankList.OnListChanged -= HandleRankListChanged;
		}

		if (IsServer)
		{
			ServerSingleton.Instance.NetServer.OnUserJoin -= HandleUserJoin;
			ServerSingleton.Instance.NetServer.OnUserLeft -= HandleUserLeft;
		}
	}

	private void HandleUserJoin(ulong clientID, UserData userData)
	{
		RankboardEntityState re = new RankboardEntityState() { clientID = clientID, playerName = userData.username, score = 0 };
		rankList.Add(re);
	}

	private void HandleUserLeft(ulong clientID, UserData userData)
	{
		foreach (RankboardEntityState re in rankList)
		{
			if (re.clientID == clientID)
			{
				try
				{
					rankList.Remove(re);
				}
				catch (Exception e)
				{
					Debug.LogError($"{re.playerName} [ {re.clientID} ] : 삭제중 오류 발생 {e.Message}");
				}
				break;
			}
		}
	}

	private void HandleRankListChanged(NetworkListEvent<RankboardEntityState> evt)
	{
		switch (evt.Type)
		{
			case NetworkListEvent<RankboardEntityState>.EventType.Add:
				AddUIToList(evt.Value);
				break;
			case NetworkListEvent<RankboardEntityState>.EventType.Remove:
				RemoveFromUIList(evt.Value.clientID);
				break;
			case NetworkListEvent<RankboardEntityState>.EventType.Value:
				AdjustScoreToUIList(evt.Value);
				break;
		}
	}

	private void AdjustScoreToUIList(RankboardEntityState value)
	{
		RecordUI record = rankUIList.Find(x => x.clientID == value.clientID);
		if (record != null)
		{
			record.SetText(1, value.playerName.ToString(), value.score);
		}
		rankUIList.Sort((RecordUI a, RecordUI b) => b.score.CompareTo(a.score));
		for (int i = 0; i < rankUIList.Count; ++i)
		{
			rankUIList[i].transform.parent = null;
			rankUIList[i].transform.SetParent(recordParentTrm);
			rankUIList[i].SetText(i + 1, rankUIList[i].username, rankUIList[i].score);
		}
		// 값을 받아서 해당 UI를 찾아서 (올바른 클라이언트 ID) score를 갱신한다.
		// 선택: 갱신 후에는 UIList를 정렬하고
		// 정렬된 순서에 맞춰서 실제 UI의 순서도 변경한다.
		// RemoveFromParent => Add
	}

	private void AddUIToList(RankboardEntityState value)
	{
		RecordUI re = rankUIList.Find(x => x.clientID == value.clientID);
		if (re == null)
		{
			RecordUI newUI = Instantiate(recordPrefab, recordParentTrm);
			newUI.SetOwner(value.clientID);
			newUI.SetText(1, value.playerName.ToString(), value.score);
			rankUIList.Add(newUI);
		}
	}

	public void HandleChangeScore(ulong clientID, int score)
	{
		for (int i = 0; i < rankList.Count; ++i)
		{
			if (rankList[i].clientID == clientID)
			{
				var oldItem = rankList[i];

				rankList[i] = new RankboardEntityState
				{
					clientID = clientID,
					playerName = oldItem.playerName,
					score = score
				};
				break;
			}
		}
	}

	public void HandleAddScore(ulong clientID, int score)
	{
		for (int i = 0; i < rankList.Count; ++i)
		{
			if (rankList[i].clientID == clientID)
			{
				var oldItem = rankList[i];

				rankList[i] = new RankboardEntityState
				{
					clientID = clientID,
					playerName = oldItem.playerName,
					score = oldItem.score + score
				};
				break;
			}
		}
	}

	private void RemoveFromUIList(ulong clientID)
	{
		RecordUI record = rankUIList.Find(x => x.clientID == clientID);
		if (record != null)
		{
			rankUIList.Remove(record);
			Destroy(record.gameObject);
		}
	}
}
