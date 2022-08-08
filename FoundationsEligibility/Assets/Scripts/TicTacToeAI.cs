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
    [SerializeField] private TicTacToeState _aiState = TicTacToeState.Cross;
    [SerializeField] private TicTacToeState _playerState = TicTacToeState.Circle;
    [SerializeField] private GameObject _xPrefab;
    [SerializeField] private GameObject _oPrefab;
    
    public WinnerEvent onPlayerWin;
    public UnityEvent onGameStarted;
    public bool isPlayerTurn;
    
    private int _maxRounds = 9;
    private int _currentRound = 0;
    private TicTacToeState[,] _boardState;
    private int _aiLevel;
    private int _gridSize = 3;
    private ClickTrigger[,] _triggers;

    private void Awake()
    {
        if (onPlayerWin == null) onPlayerWin = new WinnerEvent();
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
        _boardState = new TicTacToeState[_gridSize, _gridSize];
        _triggers = new ClickTrigger[_gridSize, _gridSize];
        isPlayerTurn = true;
        onGameStarted.Invoke();
    }

    public void PlayerSelects(int coordX, int coordY)
    {
        SetVisual(coordX, coordY, _playerState);
        _boardState[coordX, coordY] = _playerState;
        _currentRound++;
        if (CheckForWin(_playerState))
        {
            onPlayerWin.Invoke(0);
        }
        else if (_currentRound >= _maxRounds)
        {
            onPlayerWin.Invoke(-1);
        }
        else
        {
            isPlayerTurn = false;
            CalculateMove();
        }
    }

    private void AiSelects(int coordX, int coordY)
    {
        SetVisual(coordX, coordY, _aiState);
        _boardState[coordX, coordY] = _aiState;
        _currentRound++;
        if (CheckForWin(_aiState))
        {
            onPlayerWin.Invoke(1);
        }
        else if (_currentRound >= _maxRounds)
        {
            onPlayerWin.Invoke(-1);
        }
        else
        {
            isPlayerTurn = true;
        }
    }

    private void SetVisual(int coordX, int coordY, TicTacToeState targetState)
    {
        Instantiate(targetState == TicTacToeState.Circle ? _oPrefab : _xPrefab,
            _triggers[coordX, coordY].transform.position, Quaternion.identity);
    }

    private void CalculateMove()
    {
        if (_aiLevel == 0)
            FillFieldsEasy();
        else
            FillFieldsHard();
    }

    private void FillFieldsEasy()
    {
        bool foundEmptySpot = false;

        while (!foundEmptySpot)
        {
            int randomRow = Random.Range(0, 3);
            int randomCol = Random.Range(0, 3);

            if (_boardState[randomRow, randomCol] != TicTacToeState.None) continue;
            AiSelects(randomRow, randomCol);
            foundEmptySpot = true;
        }
    }

    private bool TryFillRows(TicTacToeState actorState)
    {
        for (int row = 0; row < _gridSize; row++)
        {
            int fieldsFilledRow = 0;

            for (int col = 0; col < _gridSize; col++)
            {
                if (_boardState[row, col] == actorState) fieldsFilledRow++;
            }

            if (fieldsFilledRow != _gridSize - 1) continue;

            for (int col = 0; col < _gridSize; col++)
            {
                if (_boardState[row, col] != TicTacToeState.None) continue;

                AiSelects(row, col);

                return true;
            }
        }

        return false;
    }

    private bool TryFillColumns(TicTacToeState actorState)
    {
        for (int col = 0; col < _gridSize; col++)
        {
            int fieldsFilledRow = 0;

            for (int row = 0; row < _gridSize; row++)
            {
                if (_boardState[row, col] == actorState) fieldsFilledRow++;
            }

            if (fieldsFilledRow != _gridSize - 1) continue;

            for (int row = 0; row < _gridSize; row++)
            {
                if (_boardState[row, col] != TicTacToeState.None) continue;
                AiSelects(row, col);

                return true;
            }
        }

        return false;
    }

    private bool TryFillDiagonals(TicTacToeState actorState)
    {
        // Left to Right
        int fieldsFilledDiaLeft = 0;

        for (int dia = 0; dia < _gridSize; dia++)
        {
            if (_boardState[dia, dia] == actorState) fieldsFilledDiaLeft++;
        }

        if (fieldsFilledDiaLeft == _gridSize - 1)
        {
            for (int dia = 0; dia < _gridSize; dia++)
            {
                if (_boardState[dia, dia] != TicTacToeState.None) continue;
                AiSelects(dia, dia);

                return true;
            }
        }

        // Right to Left
        int fieldsFilledDiaRight = 0;
        int rowIndex = 0;

        for (int col = _gridSize - 1; col >= 0; col--)
        {
            if (_boardState[rowIndex, col] == actorState) fieldsFilledDiaRight++;
            rowIndex++;
        }

        rowIndex = 0;

        if (fieldsFilledDiaRight == _gridSize - 1)
        {
            {
                for (int col = _gridSize - 1; col >= 0; col--)
                {
                    if (_boardState[rowIndex, col] == TicTacToeState.None)
                    {
                        AiSelects(rowIndex, col);

                        return true;
                    }

                    rowIndex++;
                }
            }

            return false;
        }

        return false;
    }

    private void FillFieldsHard()
    {
        // AI Hard Mode
        // AI Check
        if (TryFillRows(_aiState)) return;
        if (TryFillColumns(_aiState)) return;
        if (TryFillDiagonals(_aiState)) return;

        // Player Check
        if (TryFillRows(_playerState)) return;
        if (TryFillColumns(_playerState)) return;
        if (TryFillDiagonals(_playerState)) return;

        // Place in Middle (best position)
        if (_boardState[1, 1] == TicTacToeState.None)
        {
            AiSelects(1, 1);
        }
        else
        {
            FillFieldsEasy();
        }
    }

    private bool CheckForWin(TicTacToeState actorState)
    {
        return CheckRows(actorState) || CheckColumns(actorState) || CheckDiagonals(actorState);
    }

    private bool CheckRows(TicTacToeState actorState)
    {
        for (int row = 0; row < _gridSize; row++)
        {
            int fieldsFilledRow = 0;

            for (int col = 0; col < _gridSize; col++)
            {
                if (_boardState[row, col] == actorState) fieldsFilledRow++;
            }

            if (fieldsFilledRow == _gridSize)
            {
                return true;
            }
        }

        return false;
    }

    private bool CheckColumns(TicTacToeState actorState)
    {
        for (int col = 0; col < _gridSize; col++)
        {
            int fieldsFilledCol = 0;

            for (int row = 0; row < _gridSize; row++)
            {
                if (_boardState[row, col] == actorState) fieldsFilledCol++;
            }

            if (fieldsFilledCol == _gridSize)
            {
                return true;
            }
        }

        return false;
    }

    private bool CheckDiagonals(TicTacToeState actorState)
    {
        // Left to Right
        int fieldsFilledDiaLeft = 0;

        for (int dia = 0; dia < _gridSize; dia++)
        {
            if (_boardState[dia, dia] == actorState) fieldsFilledDiaLeft++;
        }

        if (fieldsFilledDiaLeft == _gridSize)
        {
            return true;
        }

        // Right to Left
        int fieldsFilledDiaRight = 0;
        int rowIndex = 0;

        for (int col = _gridSize - 1; col >= 0; col--)
        {
            if (_boardState[rowIndex, col] == actorState) fieldsFilledDiaRight++;
            rowIndex++;
        }

        if (fieldsFilledDiaRight == _gridSize) return true;
        return false;
    }
}