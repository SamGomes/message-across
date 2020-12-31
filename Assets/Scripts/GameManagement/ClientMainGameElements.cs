using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class ClientMainGameElements : NetworkBehaviour
{
    public Text countdownText;
    public GameObject emoji;

    //--------- client visual effects ------------
    [ClientRpc]
    public void DisplayCountdownText(bool active, string text)
    {
        countdownText.gameObject.SetActive(active);
        countdownText.text = text;
    }
    
    [ClientRpc]
    public void SetEmojiAnim(string animName)
    {
        emoji.GetComponent<Animator>().Play(animName);
    }
    
    [ClientRpc]
    public void StartEmojiAnim()
    {
        emoji.GetComponent<Animator>().speed = 1;
    }

    [ClientRpc]
    public void StopEmojiAnim()
    {
        emoji.GetComponent<Animator>().speed = 0;
    }
}