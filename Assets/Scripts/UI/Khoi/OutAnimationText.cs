using UnityEngine;

public class OutAnimationText : MonoBehaviour
{
    public Animator animator;
    public void DisableAnimator()
    {
        if (animator != null)
        {
            animator.enabled = false;
        }
        else
        {
            Debug.LogError("Lỗi chưa gắng animator");
        }
    }
}
