using UnityEngine;

public class ModelAnimationController : MonoBehaviour
{
    public Animation animationComponent;
    private int currentAnimationIndex = 0;
    private AnimationClip[] animationClips;

    void Start()
    {
        if (animationComponent == null)
        {
            animationComponent = GetComponent<Animation>();
        }

        if (animationComponent != null)
        {
            int clipCount = animationComponent.GetClipCount();
            animationClips = new AnimationClip[clipCount];
            int i = 0;

            foreach (AnimationState state in animationComponent)
            {
                animationClips[i] = state.clip;
                i++;
            }
        }
        else
        {
            Debug.LogError("Keine Animation-Komponente gefunden!");
        }
    }

    public void PlayNextAnimation()
    {
        if (animationClips != null && animationClips.Length > 0)
        {
            currentAnimationIndex = (currentAnimationIndex + 1) % animationClips.Length;
            AnimationClip clip = animationClips[currentAnimationIndex];
            if (clip != null)
            {
                animationComponent.clip = clip;
                animationComponent.Play();
                Debug.Log("Playing Next Animation: " + clip.name);
            }
            else
            {
                Debug.LogError("Animation Clip ist null bei Index: " + currentAnimationIndex);
            }
        }
    }

    public void PlayPreviousAnimation()
    {
        if (animationClips != null && animationClips.Length > 0)
        {
            currentAnimationIndex = (currentAnimationIndex - 1 + animationClips.Length) % animationClips.Length;
            AnimationClip clip = animationClips[currentAnimationIndex];
            if (clip != null)
            {
                animationComponent.clip = clip;
                animationComponent.Play();
                Debug.Log("Playing Previous Animation: " + clip.name);
            }
            else
            {
                Debug.LogError("Animation Clip ist null bei Index: " + currentAnimationIndex);
            }
        }
    }
}
