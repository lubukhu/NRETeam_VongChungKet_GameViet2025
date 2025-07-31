using UnityEngine;
using Obvious.Soap;

public class CardBack : MonoBehaviour
{
    [SerializeField] private GameObject targetGameObject;
    [SerializeField] private float delayTime = 1f;
    
    public void HideThenShow()
    {
        
        if (!targetGameObject.activeSelf)
        {
            targetGameObject.SetActive(true);
        }
        
        targetGameObject.SetActive(false);
        
        Invoke("ShowGameObject", delayTime);
    }

    private void ShowGameObject()
    {
        if (targetGameObject != null)
        {
            targetGameObject.SetActive(true);
        }
    }
}