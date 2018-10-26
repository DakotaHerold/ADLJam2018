#if UNITY_EDITOR
using UnityEditor;

#endif
[System.Serializable]
public class NameIndexedProceduralSoundDictionary : SerializableDictionary<string, ProceduralSound> { }
#if UNITY_EDITOR
[System.Serializable]
[CustomPropertyDrawer(typeof(NameIndexedProceduralSoundDictionary))]
public class NameIndexedProceduralSoundDictionaryDrawer : SerializableDictionaryDrawer<string, ProceduralSound> { }
#endif