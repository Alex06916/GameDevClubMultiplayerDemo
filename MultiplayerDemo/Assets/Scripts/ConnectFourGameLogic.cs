using UnityEngine;

public class ConnectFourGameLogic : MonoBehaviour {
    public static ConnectFourGameLogic Instance { get; private set; }

    public enum BoardTileStatus {
        None,
        Player1,
        Player2
    }

    public const int NUM_COLUMNS = 7;
    public const int NUM_ROWS = 6;
    public const int AMOUNT_TO_WIN = 4;

    private BoardTileStatus currentPlayer = BoardTileStatus.Player1;
    private BoardTileStatus[,] board;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        board = new BoardTileStatus[NUM_ROWS, NUM_COLUMNS];

        GameBoardUI.Instance.OnColumnButtonClicked += CompleteGameBoardUI_OnColumnButtonClicked;
    }

    private void CompleteGameBoardUI_OnColumnButtonClicked(object sender, GameBoardUI.OnColumnButtonClickedEventArgs e) {
        int columnClicked = e.column;

        if (CanDropTileInColumn(columnClicked) && currentPlayer != BoardTileStatus.None) {
            DropTile(columnClicked);
        }
    }

    private bool CanDropTileInColumn(int columnNumber) {
        return board[NUM_ROWS - 1, columnNumber] == BoardTileStatus.None;
    }

    private void DropTile(int columnNumber) {
        int row;
        for (row = 0; row < NUM_ROWS; row++) {
            if (board[row, columnNumber] == BoardTileStatus.None) {
                board[row, columnNumber] = currentPlayer;
                break;
            }
        }

        GameBoardUI.Instance.SetGridTile(row, columnNumber, currentPlayer);

        BoardTileStatus playerWonStatus = DidAPlayerWin(row, columnNumber);
        if (playerWonStatus == BoardTileStatus.None) {
            TogglePlayerTurn();
        } else {
            GameBoardUI.Instance.SetPlayerWonText(playerWonStatus);
            currentPlayer = BoardTileStatus.None;
        }
    }

    private void TogglePlayerTurn() {
        if (currentPlayer == BoardTileStatus.Player1) {
            currentPlayer = BoardTileStatus.Player2;
        } else {
            currentPlayer = BoardTileStatus.Player1;
        }

        SetPlayerTurnText();
    }

    private void SetPlayerTurnText() {
        GameBoardUI.Instance.SetPlayerTurnText(currentPlayer);
    }

    private BoardTileStatus DidAPlayerWin(int startingRow, int startingColumn) {
        BoardTileStatus testPlayer = board[startingRow, startingColumn];
        // Test hoizontal space
        int amountLeft = CountTilesInDirection(startingRow, startingColumn, -1, 0, testPlayer);
        int amountRight = CountTilesInDirection(startingRow, startingColumn, 1, 0, testPlayer);

        if (amountLeft + amountRight >= AMOUNT_TO_WIN - 1) {
            return testPlayer;
        }

        int amountUp = CountTilesInDirection(startingRow, startingColumn, 0, 1, testPlayer);
        int amountDown = CountTilesInDirection(startingRow, startingColumn, 0, -1, testPlayer);

        if (amountUp + amountDown >= AMOUNT_TO_WIN - 1) {
            return testPlayer;
        }

        int amountUpLeft = CountTilesInDirection(startingRow, startingColumn, -1, 1, testPlayer);
        int amountDownRight = CountTilesInDirection(startingRow, startingColumn, 1, -1, testPlayer);

        if (amountUpLeft + amountDownRight >= AMOUNT_TO_WIN - 1) {
            return testPlayer;
        }

        int amountUpRight = CountTilesInDirection(startingRow, startingColumn, 1, 1, testPlayer);
        int amountDownLeft = CountTilesInDirection(startingRow, startingColumn, -1, -1, testPlayer);

        if (amountUpRight + amountDownLeft >= AMOUNT_TO_WIN - 1) {
            return testPlayer;
        }

        return BoardTileStatus.None;
    }

    private int CountTilesInDirection(int startingRow, int startingColumn, int horizontalDirection, int verticalDirection, BoardTileStatus checkTile) {
        int checkRow = startingRow + verticalDirection;
        int checkColumn = startingColumn + horizontalDirection;
        int tilesInDirection = 0;

        while (checkRow >= 0 && checkRow < NUM_ROWS && checkColumn >= 0 && checkColumn < NUM_COLUMNS) {
            if (board[checkRow, checkColumn] == checkTile) {
                tilesInDirection++;
            } else {
                return tilesInDirection;
            }

            checkRow += verticalDirection;
            checkColumn += horizontalDirection;
        }

        return tilesInDirection;
    }
}
