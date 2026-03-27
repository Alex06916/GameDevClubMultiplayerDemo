using System;
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

    private void Awake() {
        Instance = this;

        for (int i = 0; i < columnButtons.Length; i++) {
            Button columnButton = columnButtons[i];
            columnButton.onClick.AddListener(() => { OnColumnButtonClicked(this, new OnColumnButtonClickedEventArgs(i)); });
        }
    }
}