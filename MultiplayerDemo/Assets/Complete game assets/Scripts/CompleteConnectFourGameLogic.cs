using Unity.Netcode;
using UnityEngine;

public class CompleteConnectFourGameLogic : NetworkBehaviour {
    public static CompleteConnectFourGameLogic Instance { get; private set; }

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

        CompleteGameBoardUI.Instance.OnColumnButtonClicked += CompleteGameBoardUI_OnColumnButtonClicked;
    }

    public void StartHost() {
        NetworkManager.StartHost();
    }

    public void StartClient() {
        NetworkManager.StartClient();
    }

    public override void OnNetworkSpawn() {
        SetPlayerTurnText();
    }

    private void CompleteGameBoardUI_OnColumnButtonClicked(object sender, CompleteGameBoardUI.OnColumnButtonClickedEventArgs e) {
        int columnClicked = e.column;

        if (!CompleteTestingOptions.verifyInput || (CanDropTileInColumn(columnClicked) && IsItMyTurn())) {
            DropTileServerRpc(columnClicked);
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void DropTileServerRpc(int columnNumber, RpcParams rpcParams = default) {
        ulong senderClientId = rpcParams.Receive.SenderClientId;
        BoardTileStatus playerTile = GetPlayerTileFromClientId(senderClientId);

        if (CanDropTileInColumn(columnNumber) && IsItPlayersTurn(senderClientId)) {
            DropTileClientRpc(columnNumber, playerTile);
        }
    }

    [Rpc(SendTo.ClientsAndHost, InvokePermission = RpcInvokePermission.Server)]
    private void DropTileClientRpc(int columnNumber, BoardTileStatus playerTile) {
        int row;
        for (row = 0; row < NUM_ROWS; row++) {
            if (board[row, columnNumber] == BoardTileStatus.None) {
                board[row, columnNumber] = playerTile;
                break;
            }
        }

        CompleteGameBoardUI.Instance.SetGridTile(row, columnNumber, playerTile);

        BoardTileStatus playerWonStatus = DidAPlayerWin(row, columnNumber);
        if (playerWonStatus == BoardTileStatus.None) {
            TogglePlayerTurn();
        } else {
            CompleteGameBoardUI.Instance.SetPlayerWonText(playerWonStatus);
            currentPlayer = BoardTileStatus.None;
        }
    }

    private bool CanDropTileInColumn(int columnNumber) {
        return board[NUM_ROWS - 1, columnNumber] == BoardTileStatus.None;
    }

    private void TogglePlayerTurn() {
        if (currentPlayer == BoardTileStatus.Player1) {
            currentPlayer = BoardTileStatus.Player2;
        } else {
            currentPlayer = BoardTileStatus.Player1;
        }

        SetPlayerTurnText();
    }

    private bool IsItPlayersTurn(ulong clientId) {
        return GetPlayerTileFromClientId(clientId) == currentPlayer;
    }

    private bool IsItMyTurn() {
        return IsItPlayersTurn(NetworkManager.LocalClientId);
    }

    private BoardTileStatus GetPlayerTileFromClientId(ulong clientId) {
        if (clientId == NetworkManager.ServerClientId) {
            return BoardTileStatus.Player1;
        } else {
            return BoardTileStatus.Player2;
        }
    }

    private void SetPlayerTurnText() {
        CompleteGameBoardUI.Instance.SetPlayerTurnText(currentPlayer, IsItMyTurn());
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
