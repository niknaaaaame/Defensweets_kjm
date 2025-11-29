using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class GameStateUI : MonoBehaviour
{
    [SerializeField] private string prefix = "STATE : ";

    private TextMeshProUGUI stateText;

    private void Awake()
    {
        stateText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (stateText == null)
            return;

        if (GameManager.Instance == null)
        {
            stateText.text = prefix + "(NO GM)";
            return;
        }

        GameState state = GameManager.Instance.CurrentState;
        string display = state switch
        {
            GameState.Ready => "READY",
            GameState.Wave => "WAVE",
            GameState.Result => "RESULT",
            _ => state.ToString().ToUpper()
        };

        stateText.text = $"{prefix}{display}";
    }
}
