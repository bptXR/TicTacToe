using System.Collections;
using UnityEngine;

public class ClickTrigger : MonoBehaviour
{
	private TicTacToeAI _ai;

	public int myCoordX;
	public int myCoordY;
	public bool canClick;
	public bool filledByPlayer;
	public bool filledByAI;

	private void Awake()
	{
		_ai = FindObjectOfType<TicTacToeAI>();
	}

	private void Start(){

		_ai.onGameStarted.AddListener(AddReference);
		_ai.onGameStarted.AddListener(() => SetInputEnabled(true));
		_ai.onPlayerWin.AddListener((win) => SetInputEnabled(false));
	}

	private void SetInputEnabled(bool val){
		canClick = val;
	}

	private void AddReference()
	{
		_ai.RegisterTransform(myCoordX, myCoordY, this);
		canClick = true;
	}

	private void OnMouseDown()
	{
		if (!canClick || !_ai.isPlayerTurn && _ai.currentRound <= 9) return;
		_ai.PlayerSelects(myCoordX, myCoordY);
		canClick = false;
		filledByPlayer = true;
		if(_ai.currentRound < 9) _ai.CalculateMove();
	}
}
