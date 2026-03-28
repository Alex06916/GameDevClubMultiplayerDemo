using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompleteGameBoardUI : MonoBehaviour {
    public static CompleteGameBoardUI Instance { get; private set; }

    public event EventHandler<OnColumnButtonClickedEventArgs> OnColumnButtonClicked;
    public class OnColumnButtonClickedEventArgs : EventArgs {
        public int column;
        
        public OnColumnButtonClickedEventArgs(int column) {
            this.column = column;
        }
    }

    [SerializeField] private GameObject startScreen;
    [SerializeField] private Button[] columnButtons;
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startClientButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private TextMeshProUGUI playerTurnText;
    [SerializeField] private TextMeshProUGUI playerWonText;
    [SerializeField] private Color player1Color;
    [SerializeField] private Color player2Color;
    [SerializeField] private Color yourTurnColor;
    [SerializeField] private Color notYourTurnColor;

    private void Awake() {
        Instance = this;

        for (int i = 0; i < columnButtons.Length; i++) {
            int columnNumber = i;  // This reassignment is necessary because the lambda function uses a reference to the value instead
            Button columnButton = columnButtons[columnNumber];

            columnButton.onClick.AddListener(() => {
                OnColumnButtonClicked?.Invoke(this, new OnColumnButtonClickedEventArgs(columnNumber));
            });
        }

        startHostButton.onClick.AddListener(() => { 
            CompleteConnectFourGameLogic.Instance.StartHost();
            startScreen.SetActive(false);
        });
        startClientButton.onClick.AddListener(() => {
            CompleteConnectFourGameLogic.Instance.StartClient();
            startScreen.SetActive(false);
        });
        exitButton.onClick.AddListener(() => { Application.Quit(); });
    }

    public void SetGridTile(int row, int column, CompleteConnectFourGameLogic.BoardTileStatus playerTile) {
        Transform columnObject = columnButtons[column].transform;
        GameObject gridTile = columnObject.GetChild(CompleteConnectFourGameLogic.NUM_ROWS - 1 - row).gameObject;

        if (gridTile.TryGetComponent(out Image gridTileImage)) {
            switch (playerTile) {
                case CompleteConnectFourGameLogic.BoardTileStatus.Player1:
                    gridTileImage.color = player1Color;
                    break;
                case CompleteConnectFourGameLogic.BoardTileStatus.Player2:
                    gridTileImage.color = player2Color;
                    break;
                default:
                    Debug.LogError("Unknown player type");
                    break;
            }
        }
    }

    public void SetPlayerTurnText(CompleteConnectFourGameLogic.BoardTileStatus playerTile, bool isLocalPlayersTurn) {
        switch (playerTile) {
            case CompleteConnectFourGameLogic.BoardTileStatus.Player1:
                playerTurnText.text = "Host's turn";
                break;
            case CompleteConnectFourGameLogic.BoardTileStatus.Player2:
                playerTurnText.text = "Client's turn";
                break;
            default:
                Debug.LogError("Unknown player type");
                break;
        }

        if (isLocalPlayersTurn) {
            playerTurnText.color = yourTurnColor;
        } else {
            playerTurnText.color = notYourTurnColor;
        }
    }

    public void SetPlayerWonText(CompleteConnectFourGameLogic.BoardTileStatus playerWon) {
        playerWonText.gameObject.SetActive(true);
        if (playerWon == CompleteConnectFourGameLogic.BoardTileStatus.Player1) {
            playerWonText.text = "Host wins!";
        } else {
            playerWonText.text = "Client wins!";
        }
    }
}
