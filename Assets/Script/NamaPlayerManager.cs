using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NamaPlayerManager : MonoBehaviour
{
    [SerializeField] private TargetKunci targetKunci;
    public InputField playerNameInputField;

    public void SetPlayerName()
    {
        string playerName = playerNameInputField.text.Trim();
        if (!string.IsNullOrEmpty(playerName))
        {
            targetKunci.NamaPlayer = playerName;
        }
    }
}

