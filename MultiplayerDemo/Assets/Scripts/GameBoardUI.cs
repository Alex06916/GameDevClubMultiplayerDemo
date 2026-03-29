using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameBoardUI : MonoBehaviour {
    public static GameBoardUI Instance { get; private set; }

    public event EventHandler<OnColumnButtonClickedEventArgs> OnColumnButtonClicked;
    public class OnColumnButtonClickedEventArgs : EventArgs {
        public int column;
        
        public OnColumnButtonClickedEventArgs(int column) {
            this.column = column;
        }
    }

    [SerializeField] private Button[] columnButtons;
    [SerializeField] private Button exitButton;
    [SerializeField] private TextMeshProUGUI playerTurnText;
    [SerializeField] private TextMeshProUGUI playerWonText;
    [SerializeField] private Color player1TileColor;
    [SerializeField] private Color player2TileColor;

    private void Awake() {
        Instance = this;

        for (int i = 0; i < columnButtons.Length; i++) {
            int columnNumber = i;  // This reassignment is necessary because the lambda function uses a reference to the value instead
            Button columnButton = columnButtons[columnNumber];

            columnButton.onClick.AddListener(() => {
                OnColumnButtonClicked?.Invoke(this, new OnColumnButtonClickedEventArgs(columnNumber));
            });
        }

        exitButton.onClick.AddListener(() => { Application.Quit(); });
    }

    public void SetGridTile(int row, int column, ConnectFourGameLogic.BoardTileStatus playerTile) {
        Transform columnObject = columnButtons[column].transform;
        GameObject gridTile = columnObject.GetChild(ConnectFourGameLogic.NUM_ROWS - 1 - row).gameObject;

        if (gridTile.TryGetComponent(out Image gridTileImage)) {
            switch (playerTile) {
                case ConnectFourGameLogic.BoardTileStatus.Player1:
                    gridTileImage.color = player1TileColor;
                    break;
                case ConnectFourGameLogic.BoardTileStatus.Player2:
                    gridTileImage.color = player2TileColor;
                    break;
                default:
                    Debug.LogError("Unknown player type");
                    break;
            }
        }
    }

    public void SetPlayerTurnText(ConnectFourGameLogic.BoardTileStatus playerTile) {
        switch (playerTile) {
            case ConnectFourGameLogic.BoardTileStatus.Player1:
                playerTurnText.text = "Host's turn";
                break;
            case ConnectFourGameLogic.BoardTileStatus.Player2:
                playerTurnText.text = "Client's turn";
                break;
            default:
                Debug.LogError("Unknown player type");
                break;
        }
    }

    public void SetPlayerWonText(ConnectFourGameLogic.BoardTileStatus playerWon) {
        playerWonText.gameObject.SetActive(true);
        if (playerWon == ConnectFourGameLogic.BoardTileStatus.Player1) {
            playerWonText.text = "Host wins!";
        } else {
            playerWonText.text = "Client wins!";
        }
    }
}
