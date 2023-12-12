using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform barTrm;

    public void HandleHealthChanged(int oldHealth, int newHealth, float ratio)
	{
        ratio = Mathf.Clamp(ratio, 0, 1f);
        barTrm.localScale = new Vector3(ratio, 1, 1); 
	}
}
