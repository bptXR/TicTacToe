using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public enum TicTacToeState
{
    None,
    Cross,
    Circle
}

[System.Serializable]
public class WinnerEvent : UnityEvent<int>
{
}

public class TicTacToeAI : MonoBehaviour
{
    TicTacToeState[,] _boardState;

    int _aiLevel;
    public bool isPlayerTurn;
    public int maxRounds = 9;
    public int currentRound = 1;
    private bool _hasWinner;

    [SerializeField] private TicTacToeState aiState = TicTacToeState.Cross;
    [SerializeField] private int gridSize = 3;
    [SerializeField] private TicTacToeState playerState = TicTacToeState.Circle;
    [SerializeField] private GameObject xPrefab;
    [SerializeField] private GameObject oPrefab;
    [SerializeField] private GameObject[] grid;
    public UnityEvent onGameStarted;

    //Call This event with the player number to denote the winner
    public WinnerEvent onPlayerWin;

    private ClickTrigger[,] _triggers;

    private void Awake()
    {
        if (onPlayerWin == null)
        {
            onPlayerWin = new WinnerEvent();
        }
    }

    public void StartAI(int aiLevel)
    {
        _aiLevel = aiLevel;
        StartGame();
    }

    public void RegisterTransform(int myCoordX, int myCoordY, ClickTrigger clickTrigger)
    {
        _triggers[myCoordX, myCoordY] = clickTrigger;
    }

    private void StartGame()
    {
        _triggers = new ClickTrigger[3, 3];
        isPlayerTurn = true;
        onGameStarted.Invoke();
    }

    public void PlayerSelects(int coordX, int coordY)
    {
        SetVisual(coordX, coordY, playerState);
        isPlayerTurn = false;
        currentRound++;
        
        print("Player has selected");
    }

    private void AiSelects(int coordX, int coordY)
    {
        SetVisual(coordX, coordY, aiState);
        isPlayerTurn = true;
        currentRound++;
        
        print("AI has selected");
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
            bool foundEmptySpot = false;

            while (!foundEmptySpot)
            {
                int randomNumber = Random.Range(0, 9);

                var gridField = grid[randomNumber].GetComponent<ClickTrigger>();

                if (gridField.canClick)
                {
                    AiSelects(gridField.myCoordX, gridField.myCoordY);
                    gridField.canClick = false;
                    foundEmptySpot = true;
                }
            }
        }
        else
        {
            AiSelects(2, 2);
        }
    }
}