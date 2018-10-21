using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TriggerSoundGenerator : MonoBehaviour {
	//public GameObject TriggerSoundPrefab;
	public List<string> Keys;
	public int index = 0;

	public void Awake(){
		index = 0;
	}

	public void GenerateSound(){
		AZProceduralAudioManager.Instance.Play (NextKey());
	}

	public string NextKey(){
		string _key = Keys [index];
		index++;
		if (index > Keys.Count - 1)
			index = 0;
		return _key;
	}
}

[CustomEditor(typeof(TriggerSoundGenerator))]
public class TSGeneratorEditor: Editor{
	public override void OnInspectorGUI() {
		if (GUILayout.Button ("Spawn"))
			(target as TriggerSoundGenerator).GenerateSound ();
		base.OnInspectorGUI();
	}
}
