using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public abstract class ProceduralSound : MonoBehaviour
{
    public bool Active = false;

    [Range(0.0f, 1.0f)]
    public float Volume; // 0 <-- x --> 1

    [Range(0.0f, 1.0f)]
    public float FadeInSpeed = 0.1f;

    [Range(0.0f, 1.0f)]
    public float FadeOutSpeed = 0.1f;

    protected PROCEDURAL_SOUND_TYPE Type;
    public INITIALIZATION_STATE Init = INITIALIZATION_STATE.UNINITIALIZED;
    public ProceduralSound Parent;

    public bool IsPlaying
    {
        get;
        set;
    }

    // Use this for initialization
    void Start()
    {
        Init = INITIALIZATION_STATE.UNINITIALIZED;
        // Playing with the idea of automatic detection of new sounds in the Scene Audio Ecosystem
        //		if (AZProceduralAudioManager.Instance.Sounds[id] == null)
        //			AZProceduralAudioManager.Instance.Sounds.Add(id, this);
    }

    public virtual void Play()
    {
        // Implemented in Subclasses
        Debug.Log("ProceduralSound " + gameObject.name + " was Played.");
    }

    public virtual void Stop()
    {
        // Implemented in Subclasses
        Debug.Log("ProceduralSound " + gameObject.name + " was Stopped.");
    }

    public IEnumerator FadeOut()
    {
        while (Volume > 0.0f)
        {
            Mathf.Lerp(Volume, 0.0f, FadeOutSpeed);
            if (FadeOutSpeed <= 0)
            {
                Debug.LogError("Infinite Coroutine Loop Exception!: Fade Out Speed is Invalid!");
                break;
            }
            if (Volume < 0.1f)
                break;
            yield return null;
        }
    }

    public IEnumerator FadeIn()
    {
        while (Volume < 1.0f)
        {
            Mathf.Lerp(Volume, 1.0f, FadeInSpeed);
            if (FadeInSpeed <= 0)
            {
                Debug.LogError("Infinite Coroutine Loop Exception!: Fade In Speed is Invalid!");
                break;
            }
            if (Volume > 0.9)
                break;
            yield return null;
        }
    }

    public float GetVolume()
    {
        return (Parent != null) ? Volume * Parent.GetVolume() : Volume;
    }

    public abstract void UpdateSound();

    protected enum PROCEDURAL_SOUND_TYPE
    {
        TIMER,
        TRIGGER,
        ADVANCED
    }

    public enum INITIALIZATION_STATE
    {
        INITIALIZED,
        UNINITIALIZED
    }
}

#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(ProceduralSound), true)]
public class ProceduralSoundEditor : Editor
{
    public override void OnInspectorGUI()
    {
        bool isPlaying = (((ProceduralSound)target).IsPlaying);
        EditorGUILayout.LabelField("Playing?: " +
            ((isPlaying) ? "YES" : "NO"));
        if (isPlaying)
        {
            if (GUILayout.Button("Stop"))
                (target as ProceduralSound).Stop();
        }
        else
        {
            if (GUILayout.Button("Play"))
                (target as ProceduralSound).Play();
        }

        EditorGUILayout.LabelField("Initialized?: " + (((ProceduralSound)target).Init == ProceduralSound.INITIALIZATION_STATE.INITIALIZED ? "YES" : "NO"));
        base.OnInspectorGUI();
    }
}
#endif