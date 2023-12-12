using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RecordUI : MonoBehaviour
{
    private TextMeshProUGUI recordText;

    public ulong clientID;
	public int rank;
	public string username;
	public int score;

	private void Awake()
	{
		recordText = GetComponent<TextMeshProUGUI>();
	}

	public void SetOwner(ulong ownerID)
	{
		clientID = ownerID;
	}

	public void SetText(int rank, string username, int score)
	{
		this.rank = rank;
		this.username = username;
		this.score = score;
		recordText.SetText($"{rank.ToString()} . {username} [ {score.ToString()} ]"); 
	}
}
