using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 
/// </summary>
[System.Serializable]
public class AZProceduralAudioManager : MonoBehaviour {	
	[SerializeField]
	public static AZProceduralAudioManager Instance;

	[SerializeField]
	public NameIndexedProceduralSoundDictionary Sounds;

	private bool active = true;
	public bool Active {
		get { return active; }
		set { 
			active = value; 
			foreach (string _key in Sounds.Keys) {
				ProceduralSound _sound = Sounds[_key];
				if(_sound != null)
					_sound.Active = value;
			}
		}
	}

	public void Start(){
		if (Instance != null)
			Destroy (Instance.gameObject);
		Instance = this;

		if (Sounds == null) {
			Debug.LogWarning ("Sound Dictionary initially null!");
			Sounds = new NameIndexedProceduralSoundDictionary ();
		}
	}

	public void Add(string _key, ProceduralSound _newSound) {
		Sounds.Add(_key, _newSound);
	}
		
	public void Remove(string _key) {
		Sounds.Remove(_key);
	}

	public void Destroy(){
		foreach (string _key in Sounds.Keys) {
			Sounds.Remove(_key);
		}
	}

	public void Play(string key, float volume){
		if (!Active) {
			Debug.LogError ("Unavailability Exception: You may NOT play sounds through AZAM\n when AZAM is disabled.\n");
		} else {
			Sounds [key].Volume = volume;
			Sounds [key].Play ();
		}
	}

	public void Play(string key){
		if (!Active) {
			Debug.LogError ("Unavailability Exception: You may NOT play sounds through AZAM\n when AZAM is disabled.\n");
		} 
		else if (Sounds.ContainsKey (key)) {
			Sounds [key].Play ();
		} 
		else {
            Debug.Log("Key: " + key);
			Debug.LogError ("Unassigned Exception: Sound is not contained in the AudioManager's collection.\n");
		}
	}


	public void Stop(string key){
		if (!Active) {
			Debug.LogError ("Unavailability Exception: You may NOT stop sounds through AZAM\n when AZAM is disabled.\n");
		}

		Sounds [key].Stop ();
	}

	public void Update(){
		foreach (string _key in Sounds.Keys) {
			if (!Sounds.Keys.Contains (_key))
				continue;
			if (string.IsNullOrEmpty (_key))
				continue;
			if(Sounds[_key]!=null)
				Sounds[_key].UpdateSound ();
		}
	}
}

[CustomEditor(typeof(AZProceduralAudioManager))]
public class AZProceduralAudioManagerEditor : Editor 
{
	string soundName = "";
	bool defaultGUI = true;
	public override void OnInspectorGUI() {
		defaultGUI = (GUILayout.Button ("Default Inspector")) ? !defaultGUI : defaultGUI;
		if (defaultGUI == true)
			base.OnInspectorGUI();
		
		bool active = (GUILayout.Button ("Toggle Active")) ? !((AZProceduralAudioManager)target).Active : ((AZProceduralAudioManager)target).Active;
		((AZProceduralAudioManager)target).Active = active;

		GUIStyle ActiveTextStyle = new GUIStyle(EditorStyles.label);
		ActiveTextStyle.normal.textColor = (active) ? Color.green : Color.red;
		GUILayout.Label(((active)? "ACTIVE" : "INACTIVE"),ActiveTextStyle);
		GUILayout.Box("", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});
		soundName = EditorGUILayout.TextArea(soundName, GUILayout.MaxHeight(75) );

		if (GUILayout.Button ("Play Sound")) {
			((AZProceduralAudioManager)target).Play (soundName);
		}
	}
}