using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class VisualDialog : MonoBehaviour
{
    public enum Character { Player, NPC };

    [System.Serializable]
    public class DialogData
    {
        public Character character;
        public string name;
        [TextArea(5, 6)]
        public string dialog;
        public List<string> boldSentences;
    }

    [Header("Player Name")]
    public string PlayerName;

    [Header("Active Dialog")]
    [TextArea(5, 3)]
    public string activeDialog;
    public bool typewriting;
    public float delay;

    [Header("Visual Setting")]
    public GameObject ParentObject;
    public Image dialogPanel;
    public Image playerImage;
    public Image npcImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogText;

    [Header("Dialog Setting")]
    public bool AutoStartDialog;
    public bool AutoFinishDialog;
    public List<DialogData> playerDialogs = new List<DialogData>();

    [Header("Event Dialog Setting")]
    public UnityEvent StartDialogEvent;
    public UnityEvent FinishDialogEvent;

    string currentText = "";
    private int currentDialogIndex = 0;
    private Coroutine DialogCoroutine;
    private bool isDialogRunning = false;

    public void StartDialog()
    {
        currentDialogIndex = 0;
        ActivateDialog(playerDialogs[currentDialogIndex]);
        StartDialogEvent?.Invoke();
    }

    void Start()
    {
        if (AutoStartDialog) StartDialog();
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Return)) && isDialogRunning)
        {
            if (typewriting && DialogCoroutine != null)
            {
                StopCoroutine(DialogCoroutine);
                dialogText.text = activeDialog;
                typewriting = false;
            }
            else
            {
                typewriting = true;
                NextDialog();
            }
        }
    }

    void NextDialog()
    {
        isDialogRunning = false;
        currentDialogIndex++;
        currentText = "";
        dialogText.text = "";

        if (currentDialogIndex < playerDialogs.Count)
        {
            ActivateDialog(playerDialogs[currentDialogIndex]);
        }
        else
        {
            if (AutoFinishDialog)
            {
                SetChildStatus(ParentObject, false);
            }
            FinishDialogEvent?.Invoke();
        }
    }

    void ActivateDialog(DialogData dialogData)
    {
        string player = PlayerName;
        SetChildStatus(ParentObject, true);
        if (dialogData.character == Character.Player)
        {
            playerImage.gameObject.SetActive(true);
            npcImage.gameObject.SetActive(false);
        }
        else if (dialogData.character == Character.NPC)
        {
            playerImage.gameObject.SetActive(false);
            npcImage.gameObject.SetActive(true);
        }

        nameText.text = dialogData.name;
        if (dialogData.name == "<name>")
        {
            nameText.text = player;
        }

        activeDialog = dialogData.dialog;
        string editedString = activeDialog;
        if (activeDialog.Contains("<name>"))
        {
            editedString = EditString(activeDialog, "<name>", player);
        }

        editedString = ApplyBoldFormatting(editedString, dialogData.boldSentences);
        activeDialog = editedString; // Update activeDialog with the formatted string

        if (typewriting)
        {
            DialogCoroutine = StartCoroutine(TypeText(editedString));
        }
        else
        {
            dialogText.text = editedString;
        }
        isDialogRunning = true;
    }

    string ApplyBoldFormatting(string dialog, List<string> boldSentences)
    {
        foreach (string sentence in boldSentences)
        {
            dialog = dialog.Replace(sentence, $"<b>{sentence}</b>");
        }
        return dialog;
    }

    IEnumerator TypeText(string fullString)
    {
        for (int i = 0; i < fullString.Length; i++)
        {
            currentText += fullString[i];
            dialogText.text = currentText;
            yield return new WaitForSeconds(delay);
        }
        typewriting = false;
    }

    string EditString(string originalString, string targetWord, string replacementWord)
    {
        string[] words = originalString.Split(' ');

        for (int i = 0; i < words.Length; i++)
        {
            if (words[i] == targetWord)
            {
                words[i] = replacementWord;
                break;
            }
        }

        return string.Join(" ", words);
    }

    public void SetChildStatus(GameObject parentObject, bool aValue)
    {
        Transform[] childTransforms = parentObject.GetComponentsInChildren<Transform>(true);

        foreach (Transform childTransform in childTransforms)
        {
            if (childTransform.gameObject != parentObject)
            {
                childTransform.gameObject.SetActive(aValue);
            }
        }
    }
}