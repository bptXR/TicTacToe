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
    //Call This event with the player number to denote the winner
    public WinnerEvent onPlayerWin;

    public UnityEvent onGameStarted;
    public bool isPlayerTurn;
    public int maxRounds = 9;
    public int currentRound = 1;

    [SerializeField] private TicTacToeState aiState = TicTacToeState.Cross;
    [SerializeField] private TicTacToeState playerState = TicTacToeState.Circle;
    [SerializeField] private GameObject xPrefab;
    [SerializeField] private GameObject oPrefab;

    private TicTacToeState[,] _boardState;
    private int _aiLevel;
    private int _gridSize = 3;
    private ClickTrigger[,] _triggers;
    public bool playerWin;
    public bool aiWin;

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
        _triggers = new ClickTrigger[3, 3];
        isPlayerTurn = true;
        onGameStarted.Invoke();
    }

    public void PlayerSelects(int coordX, int coordY)
    {
        SetVisual(coordX, coordY, playerState);
        isPlayerTurn = false;
        currentRound++;
        CheckForWin();
    }

    private void AiSelects(int coordX, int coordY)
    {
        if (playerWin) return;
        SetVisual(coordX, coordY, aiState);
        isPlayerTurn = true;
        currentRound++;
        CheckForWin();
    }

    private void SetVisual(int coordX, int coordY, TicTacToeState targetState)
    {
        Instantiate(targetState == TicTacToeState.Circle ? oPrefab : xPrefab,
            _triggers[coordX, coordY].transform.position, Quaternion.identity);
    }

    public void CalculateMove()
    {
        if (_aiLevel == 0)
            CheckGridEasy();
        else
            CheckGridHard();
    }

    private void CheckGridEasy()
    {
        bool foundEmptySpot = false;

        while (!foundEmptySpot)
        {
            int randomRow = Random.Range(0, 3);
            int randomCol = Random.Range(0, 3);

            if (!_triggers[randomRow, randomCol].canClick) continue;
            _triggers[randomRow, randomCol].canClick = false;
            _triggers[randomRow, randomCol].filledByAI = true;
            AiSelects(randomRow, randomCol);
            foundEmptySpot = true;
        }
    }

    private void CheckGridHard()
    {
        // AI Hard Mode
        // AI Check
        // Rows
        for (int row = 0; row < _gridSize; row++)
        {
            int amountFilledAICol = 0;

            for (int col = 0; col < _gridSize; col++)
            {
                if (_triggers[row, col].filledByAI) amountFilledAICol++;
            }

            if (amountFilledAICol == _gridSize - 1)
            {
                for (int col = 0; col < _gridSize; col++)
                {
                    if (!_triggers[row, col].filledByAI && _triggers[row, col].canClick)
                    {
                        _triggers[row, col].canClick = false;
                        _triggers[row, col].filledByAI = true;
                        AiSelects(row, col);
                        return;
                    }
                }
            }
        }

        // Columns
        for (int col = 0; col < _gridSize; col++)
        {
            int amountFilledAIRow = 0;

            for (int row = 0; row < _gridSize; row++)
            {
                if (_triggers[row, col].filledByAI) amountFilledAIRow++;
            }

            if (amountFilledAIRow == _gridSize - 1)
            {
                for (int row = 0; row < _gridSize; row++)
                {
                    if (!_triggers[row, col].filledByAI && _triggers[row, col].canClick)
                    {
                        _triggers[row, col].canClick = false;
                        _triggers[row, col].filledByAI = true;
                        AiSelects(row, col);
                        return;
                    }
                }
            }
        }

        // Diagonal Left to Right
        int amountFilledAIDiaLeft = 0;

        for (int dia = 0; dia < _gridSize; dia++)
        {
            if (_triggers[dia, dia].filledByAI) amountFilledAIDiaLeft++;
        }

        if (amountFilledAIDiaLeft == _gridSize - 1)
        {
            for (int dia = 0; dia < _gridSize; dia++)
            {
                if (!_triggers[dia, dia].filledByAI && _triggers[dia, dia].canClick)
                {
                    _triggers[dia, dia].canClick = false;
                    _triggers[dia, dia].filledByAI = true;
                    AiSelects(dia, dia);
                    return;
                }
            }
        }

        // Diagonal Right to Left
        int amountFilledAIDiaRight = 0;
        int rowIndexAI = 0;

        for (int col = _gridSize - 1; col >= 0; col--)
        {
            if (_triggers[rowIndexAI, col].filledByAI) amountFilledAIDiaRight++;
            rowIndexAI++;
        }

        rowIndexAI = 0;
        if (amountFilledAIDiaRight == _gridSize - 1)
        {
            for (int col = _gridSize - 1; col >= 0; col--)
            {
                if (!_triggers[rowIndexAI, col].filledByAI && _triggers[rowIndexAI, col].canClick)
                {
                    _triggers[rowIndexAI, col].canClick = false;
                    _triggers[rowIndexAI, col].filledByAI = true;
                    AiSelects(rowIndexAI, col);
                    return;
                }

                rowIndexAI++;
            }
        }

        // Player Check
        // Rows
        for (int row = 0; row < _gridSize; row++)
        {
            int amountFilledPlayerCol = 0;

            for (int col = 0; col < _gridSize; col++)
            {
                if (_triggers[row, col].filledByPlayer) amountFilledPlayerCol++;
            }

            if (amountFilledPlayerCol == _gridSize - 1)
            {
                for (int col = 0; col < _gridSize; col++)
                {
                    if (!_triggers[row, col].filledByPlayer && _triggers[row, col].canClick)
                    {
                        _triggers[row, col].canClick = false;
                        _triggers[row, col].filledByAI = true;
                        AiSelects(row, col);
                        return;
                    }
                }
            }
        }

        // Columns
        for (int col = 0; col < _gridSize; col++)
        {
            int amountFilledPlayerRow = 0;

            for (int row = 0; row < _gridSize; row++)
            {
                if (_triggers[row, col].filledByPlayer) amountFilledPlayerRow++;
            }

            if (amountFilledPlayerRow == _gridSize - 1)
            {
                for (int row = 0; row < _gridSize; row++)
                {
                    if (!_triggers[row, col].filledByPlayer && _triggers[row, col].canClick)
                    {
                        _triggers[row, col].canClick = false;
                        _triggers[row, col].filledByAI = true;
                        AiSelects(row, col);
                        return;
                    }
                }
            }
        }

        // Diagonal Left to Right
        int amountFilledPlayerDia = 0;

        for (int dia = 0; dia < _gridSize; dia++)
        {
            if (_triggers[dia, dia].filledByPlayer) amountFilledPlayerDia++;
        }

        if (amountFilledPlayerDia == _gridSize - 1)
        {
            for (int dia = 0; dia < _gridSize; dia++)
            {
                if (!_triggers[dia, dia].filledByPlayer && _triggers[dia, dia].canClick)
                {
                    _triggers[dia, dia].canClick = false;
                    _triggers[dia, dia].filledByAI = true;
                    AiSelects(dia, dia);
                    return;
                }
            }
        }

        // Diagonal Right to Left
        int amountFilledPlayerDiaRight = 0;
        int rowIndexPlayer = 0;

        for (int col = _gridSize - 1; col >= 0; col--)
        {
            if (_triggers[rowIndexPlayer, col].filledByPlayer) amountFilledPlayerDiaRight++;
            rowIndexPlayer++;
        }

        rowIndexPlayer = 0;
        if (amountFilledPlayerDiaRight == _gridSize - 1)
        {
            for (int col = _gridSize - 1; col >= 0; col--)
            {
                if (!_triggers[rowIndexPlayer, col].filledByPlayer && _triggers[rowIndexPlayer, col].canClick)
                {
                    _triggers[rowIndexPlayer, col].canClick = false;
                    _triggers[rowIndexPlayer, col].filledByAI = true;
                    AiSelects(rowIndexPlayer, col);
                    return;
                }

                rowIndexPlayer++;
            }
        }

        // Place in Middle (best position)
        if (_triggers[1, 1].canClick)
        {
            _triggers[1, 1].canClick = false;
            _triggers[1, 1].filledByAI = true;
            AiSelects(1, 1);
        }

        else
        {
            CheckGridEasy();
        }
    }

    private void CheckForWin()
    {
        // Horizontal
        // First Row
        if (_triggers[0, 0].filledByPlayer && _triggers[0, 1].filledByPlayer && _triggers[0, 2].filledByPlayer)
        {
            onPlayerWin.Invoke(0);
            playerWin = true;
        }
        else if (_triggers[0, 0].filledByAI && _triggers[0, 1].filledByAI && _triggers[0, 2].filledByAI)
        {
            onPlayerWin.Invoke(1);
            aiWin = true;
        }

        // Middle Row
        if (_triggers[1, 0].filledByPlayer && _triggers[1, 1].filledByPlayer && _triggers[1, 2].filledByPlayer)
        {
            onPlayerWin.Invoke(0);
            playerWin = true;
        }
        else if (_triggers[1, 0].filledByAI && _triggers[1, 1].filledByAI && _triggers[1, 2].filledByAI)
        {
            onPlayerWin.Invoke(1);
            aiWin = true;
        }

        // Bottom Row
        if (_triggers[2, 0].filledByPlayer && _triggers[2, 1].filledByPlayer && _triggers[2, 2].filledByPlayer)
        {
            onPlayerWin.Invoke(0);
            playerWin = true;
        }
        else if (_triggers[2, 0].filledByAI && _triggers[2, 1].filledByAI && _triggers[2, 2].filledByAI)
        {
            onPlayerWin.Invoke(1);
            aiWin = true;
        }

        // Vertical
        // First Column
        if (_triggers[0, 0].filledByPlayer && _triggers[1, 0].filledByPlayer && _triggers[2, 0].filledByPlayer)
        {
            onPlayerWin.Invoke(0);
            playerWin = true;
        }
        else if (_triggers[0, 0].filledByAI && _triggers[1, 0].filledByAI && _triggers[2, 0].filledByAI)
        {
            onPlayerWin.Invoke(1);
            aiWin = true;
        }

        // Second Column
        if (_triggers[0, 1].filledByPlayer && _triggers[1, 1].filledByPlayer && _triggers[2, 1].filledByPlayer)
        {
            onPlayerWin.Invoke(0);
            playerWin = true;
        }
        else if (_triggers[0, 1].filledByAI && _triggers[1, 1].filledByAI && _triggers[2, 1].filledByAI)
        {
            onPlayerWin.Invoke(1);
            aiWin = true;
        }

        // Third Column
        if (_triggers[0, 2].filledByPlayer && _triggers[1, 2].filledByPlayer && _triggers[2, 2].filledByPlayer)
        {
            onPlayerWin.Invoke(0);
            playerWin = true;
        }
        else if (_triggers[0, 2].filledByAI && _triggers[1, 2].filledByAI && _triggers[2, 2].filledByAI)
        {
            onPlayerWin.Invoke(1);
            aiWin = true;
        }

        // Diagonal
        // Left to Right
        if (_triggers[0, 0].filledByPlayer && _triggers[1, 1].filledByPlayer && _triggers[2, 2].filledByPlayer)
        {
            onPlayerWin.Invoke(0);
            playerWin = true;
        }
        else if (_triggers[0, 0].filledByAI && _triggers[1, 1].filledByAI && _triggers[2, 2].filledByAI)
        {
            onPlayerWin.Invoke(1);
            aiWin = true;
        }

        // Right to Left
        if (_triggers[0, 2].filledByPlayer && _triggers[1, 1].filledByPlayer && _triggers[2, 0].filledByPlayer)
        {
            onPlayerWin.Invoke(0);
            playerWin = true;
        }
        else if (_triggers[0, 2].filledByAI && _triggers[1, 1].filledByAI && _triggers[2, 0].filledByAI)
        {
            onPlayerWin.Invoke(1);
            aiWin = true;
        }

        else if (currentRound == maxRounds + 1 && !playerWin && !aiWin) onPlayerWin.Invoke(-1);
    }
}
