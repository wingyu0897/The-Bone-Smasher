using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
	private int damage;
	private ulong clientId;

	public void SetDamage(int damage)
	{
		this.damage = damage;
	}

	public void SetOwner(ulong ownerClientId)
	{
		clientId = ownerClientId;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.attachedRigidbody == null) return;

		if (collision.attachedRigidbody.TryGetComponent<Health>(out Health health))
		{
			health.TakeDamage(damage, clientId);
		}
	}
}
