using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeBase : MonoBehaviour
{
    [SerializeField] private float lifeTime = 2f;
    private float currentLifeTime = 0f;

	private void Update()
	{
		currentLifeTime += Time.deltaTime;
		if (currentLifeTime >= lifeTime)
			Destroy(gameObject);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Destroy(gameObject);
	}
}
