using UnityEngine;

public class ClickTrigger : MonoBehaviour
{
	private TicTacToeAI _ai;

	[SerializeField] private int myCoordX;
	[SerializeField] private int myCoordY;
	[SerializeField] private bool canClick;

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
		if (!canClick || !_ai.isPlayerTurn) return;
		_ai.PlayerSelects(myCoordX, myCoordY);
		canClick = false;
		_ai.CalculateMove();
	}
}
