using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// GUIDE TO CREATING PROPERTY DRAWERS IN INSPECTOR
[UnityEditor.CustomPropertyDrawer(typeof(SerializableDictionary <int, float> ))]
[System.Serializable] public class SerializableTrackToVolumeLinks: SerializableDictionary<int, 
																				  float> {}
 
[UnityEditor.CustomPropertyDrawer(typeof(SerializableTrackToVolumeLinks))]
public class SerializableTrackVolumeDrawer : SerializableDictionaryDrawer<int, float> { }