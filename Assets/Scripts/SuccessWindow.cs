using UnityEngine;
using TMPro;

public class SuccessWindow : MonoBehaviour
{
	[SerializeField] private SphereComparisonSystem sphereComparisonSystem;
	[SerializeField] private TextMeshProUGUI _scoreText;

	private void OnEnable()
	{
		_scoreText.text = $"Score: {sphereComparisonSystem.GetScore()}%";
	}
}
