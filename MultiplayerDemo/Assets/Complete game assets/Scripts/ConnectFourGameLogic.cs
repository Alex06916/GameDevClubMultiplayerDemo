using UnityEngine;

public class ConnectFourGameLogic : MonoBehaviour {
    private enum BoardTileStatus {
        None,
        Red,
        Yellow
    }

    private BoardTileStatus[,] board;
    private const int NUM_ROWS= 7;
    private const int NUM_COLUMNS = 6;

    private void Start() {
        board = new BoardTileStatus[NUM_ROWS, NUM_COLUMNS];

        GameBoardUI.Instance.OnColumnButtonClicked += GameBoardUI_OnColumnButtonClicked;
    }

    private void GameBoardUI_OnColumnButtonClicked(object sender, GameBoardUI.OnColumnButtonClickedEventArgs e) {
        int columnClicked = e.column;
    }
}