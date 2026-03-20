using UnityEngine;

[System.Serializable]
public class InteractionState
{
    public GameObject[] showObjects;
    public GameObject[] hideObjects;

    public void SetInteractionState()
    {
        foreach (var showObject in showObjects)
            showObject.SetActive(true);

        foreach (var hideObject in hideObjects)
            hideObject.SetActive(false);
    }
}