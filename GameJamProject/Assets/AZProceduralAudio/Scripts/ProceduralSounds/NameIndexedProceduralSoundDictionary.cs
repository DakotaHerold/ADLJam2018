using UnityEngine;

[System.Serializable]
public class NameIndexedProceduralSoundDictionary : SerializableDictionary<string,ProceduralSound>{}

[System.Serializable]
[UnityEditor.CustomPropertyDrawer(typeof(NameIndexedProceduralSoundDictionary))]
public class NameIndexedProceduralSoundDictionaryDrawer : SerializableDictionaryDrawer<string,ProceduralSound> { }

