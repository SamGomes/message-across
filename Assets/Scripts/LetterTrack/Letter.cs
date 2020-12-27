using System.Collections;
using Mirror;
using UnityEngine;

public class Letter : NetworkBehaviour {

    public float speed;
    public char letterText;

    private bool isLocked;

    void Awake() {
        isLocked = false;
        //speed = 10.5f;
    }
	
	void FixedUpdate () {
        if (!isLocked)
        {
            float translation = Time.deltaTime * speed;
            transform.Translate(translation, 0, 0);
        }
    }

    [ClientRpc]
    public void Lock()
    {
        isLocked = true;
    }
    [ClientRpc]
    public void Unlock()
    {
        isLocked = false;
    }
    
    public bool IsLocked()
    {
        return isLocked;
    }
    
    
    private IEnumerator AnimateLetter(Vector3 position)
    {
        yield return Globals.LerpAnimation(this.gameObject, position, 1.0f);
        Destroy(this.gameObject);
    }

    [ClientRpc]
    public void AnimateAndDestroy(Vector3 position)
    {
        StartCoroutine(AnimateLetter(position));
    }
}
