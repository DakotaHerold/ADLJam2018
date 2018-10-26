#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable] public class SerializableTrackToVolumeLinks : SerializableDictionary<int, float> { }

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SerializableTrackToVolumeLinks))]
public class SerializableTrackVolumeDrawer : SerializableDictionaryDrawer<int, float> { }
#endif