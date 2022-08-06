using TMPro;
using UnityEngine;

public class EndMessage : MonoBehaviour
{

	[SerializeField]
	private TMP_Text playerMessage = null;

	public void OnGameEnded(int winner)
	{
		playerMessage.text = winner == -1 ? "Tie" : winner == 1 ? "AI wins" : "Player wins";
	}
}
