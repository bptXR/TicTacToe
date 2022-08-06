using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public enum TicTacToeState{None, Cross, Circle}

[System.Serializable]
public class WinnerEvent : UnityEvent<int>
{
	
}

public class TicTacToeAI : MonoBehaviour
{
	int _aiLevel;

	TicTacToeState[,] _boardState;
	TicTacToeState aiState = TicTacToeState.Cross;

	public bool isPlayerTurn;
	private bool _hasWinner;
	[SerializeField] private int gridSize = 3;
	[SerializeField] private TicTacToeState playerState = TicTacToeState.Circle;
	[SerializeField] private GameObject xPrefab;
	[SerializeField] private GameObject oPrefab;

	public UnityEvent onGameStarted;

	//Call This event with the player number to denote the winner
	public WinnerEvent onPlayerWin;

	private ClickTrigger[,] _triggers;
	
	private void Awake()
	{
		if(onPlayerWin == null){
			onPlayerWin = new WinnerEvent();
		}
	}

	public void StartAI(int aiLevel){
		_aiLevel = aiLevel;
		StartGame();
	}

	public void RegisterTransform(int myCoordX, int myCoordY, ClickTrigger clickTrigger)
	{
		_triggers[myCoordX, myCoordY] = clickTrigger;
	}

	private void StartGame()
	{
		_triggers = new ClickTrigger[3,3];
		isPlayerTurn = true;
		onGameStarted.Invoke();
	}

	public void PlayerSelects(int coordX, int coordY){

		SetVisual(coordX, coordY, playerState);
		isPlayerTurn = false;
		print("Player has selected");
		_hasWinner = CheckForWinner();
		if(_hasWinner) print("Winner");
	}

	private void AiSelects(int coordX, int coordY){

		SetVisual(coordX, coordY, aiState);
		isPlayerTurn = true;
		print("AI has selected");
		_hasWinner = CheckForWinner();
		if(_hasWinner) print("Winner");
	}

	private void SetVisual(int coordX, int coordY, TicTacToeState targetState)
	{
		Instantiate(
			targetState == TicTacToeState.Circle ? oPrefab : xPrefab,
			_triggers[coordX, coordY].transform.position,
			Quaternion.identity
		);
	}
	
	public void CalculateMove()
	{
		if (_aiLevel == 0)
		{
			AiSelects(0, 0);
		}
		else
		{
			AiSelects(2, 2);
		}
	}

	private bool CheckForWinner()
	{
		int i = 0;
		
		//Horizontal
		for (i = 0; i <= 6; i += 3)
		{
			if (!CheckValues(i, i + 1))
				continue;
			if (!CheckValues(i, i + 2))
				continue;
			return true;
		}

		return false;
	}

	private bool CheckValues(int coordX, int coordY)
	{
		return false;
	}
}
