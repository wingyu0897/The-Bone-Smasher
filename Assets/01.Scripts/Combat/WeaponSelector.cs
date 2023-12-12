using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WeaponSelector : NetworkBehaviour
{
	private static List<WeaponSelector> selectors = new List<WeaponSelector>();
    [SerializeField] private Attacker attacker;
    [SerializeField] private List<Weapon> weapons;
	private Weapon selected;
	public Weapon Selected => selected;

	public override void OnNetworkSpawn()
	{
		if (IsServer)
		{
			selected = weapons[Random.Range(0, weapons.Count)];
			selected.gameObject.SetActive(true);
			attacker.SetWeapon(selected);
			SetVisualClientRpc(selected.gameObject.name);

			selectors.Add(this);
		}
		else
		{
			SetVisualOnServerRpc();
		}
	}

	public override void OnNetworkDespawn()
	{
		if (!IsServer) return;

		selectors.Remove(this);
	}

	[ServerRpc(RequireOwnership = false)]
	public void SetVisualOnServerRpc()
	{
		//selectors = FindObjectsOfType<WeaponSelector>().ToList();
		SetVisualOn();
	}

	private static void SetVisualOn()
	{
		foreach (var selector in selectors)
		{
			selector.SetVisualClientRpc(selector.Selected.gameObject.name);
		}
	}
	 
	[ClientRpc]
	public void SetVisualClientRpc(string name)
	{
		Debug.Log($"Finding {name}...");
		selected = weapons.Find(x => x.gameObject.name == name);
		selected.gameObject.SetActive(true);
		attacker.SetWeapon(selected);
		if (attacker.weaponAnimator == null)
			attacker.weaponAnimator = selected.transform.Find("Visual").GetComponent<WeaponAnimation>();
	}
}
