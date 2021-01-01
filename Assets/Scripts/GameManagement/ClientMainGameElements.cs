using Mirror;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//wrapper class for broadcasted GameManager calls
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

    
    [ClientRpc]
    public void PlayAudioClip(int managerIndex, string clipPath)
    {
        Globals.audioManagers[managerIndex].PlayClip(clipPath);
    }
    
    [ClientRpc]
    public void StopCurrentAudioClip(int managerIndex)
    {
        Globals.audioManagers[managerIndex].StopCurrentClip();
    }
    
    [ClientRpc]
    public void PlayInfiniteAudioClip(int managerIndex, string introClipPath, string loopClipPath)
    {
        Globals.audioManagers[managerIndex].PlayInfiniteClip(introClipPath, loopClipPath);
    }

    
}