using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Allows for Resequencing of Individual Tracks
// Trigger / Timer hybrid: On current Sound Finished
public class ProceduralSoundVerticalRemixer : ProceduralSound, FadableSound {
	// ALL NAMES MUST BE THE SAME AS THE NAME IN THE AZ AUDIO MANAGER

	[SerializeField]
	protected List<SoundVolumeAssociation> TrackVolumes = new List<SoundVolumeAssociation>();

	[SerializeField]
	protected List<VerticalRemixerVolumeState> TrackVolumeStates = new List<VerticalRemixerVolumeState>();

	[Tooltip("Each string is linked to the track volume states in order listed.\n1st string to 0, 2nd to 1...")]
	[SerializeField]
	protected List<string> TrackVolumeStateNames = new List<string>();

	public bool Fadable {
		get;
		set;
	}

	void Start(){
		Type = PROCEDURAL_SOUND_TYPE.ADVANCED;
		Volume = 1.0f;
		foreach (SoundVolumeAssociation association in TrackVolumes) {
			association.Sound.Parent = this;
		}
	}

	public override void UpdateSound(){
		if (IsPlaying) {
			foreach (SoundVolumeAssociation association in TrackVolumes) {
				if (Fadable) {
					float fadeSpeed = (association.Sound.Volume - (association.Volume)>0)? FadeInSpeed
									: (association.Sound.Volume - (association.Volume)<0)? FadeOutSpeed 
								    : 0f;
					association.Sound.Volume = Mathf.Lerp(association.Sound.Volume, association.Volume, fadeSpeed);
				} else {
					association.Sound.Volume = association.Volume;
				}
			}
		}
	}

	public override void Play (){
		IsPlaying = true;
		foreach (SoundVolumeAssociation association in TrackVolumes) {
			association.Sound.Volume = association.Volume;
			association.Sound.Play ();
			ProceduralSound sound = association.Sound;
			Debug.Log ("Vertical Remix Sound Playing: " + sound.name);
		}
	}

	public override void Stop (){
		IsPlaying = false;
	}

	public void SetState(string _stateName){
		if (TrackVolumeStateNames.IndexOf (_stateName) == -1)
			throw new UnityException ("INVALID VR STATE NAME!!!");
		if (TrackVolumeStateNames.IndexOf (_stateName)>TrackVolumeStates.Count)
			throw new UnityException ("VR STATE COUNT IS TOO HIGH! Please remove a state name or add a state!");
		int _stateIndex = TrackVolumeStateNames.IndexOf (_stateName);
		for(int i = 0; i<TrackVolumes.Count; i++){
			TrackVolumes[i].Volume = TrackVolumeStates[_stateIndex].TrackVolumes[i];
		}
	}

	public void Destroy(){
		while(TrackVolumes.Count>0) {
			TrackVolumes.RemoveAt(0);
		}
		TrackVolumes = null;
	}
}

[CustomEditor(typeof(ProceduralSoundVerticalRemixer), true)]
public class ProceduralSoundVREditor : ProceduralSoundEditor 
{
	string stateName = "";
	bool defaultGUI = true;
	public override void OnInspectorGUI() {
		defaultGUI = (GUILayout.Button ("PSVR Default Inspector")) ? !defaultGUI : defaultGUI;
		if (defaultGUI == true)
			base.OnInspectorGUI();
		
		GUIStyle ActiveTextStyle = new GUIStyle(EditorStyles.label);
		ActiveTextStyle.normal.textColor = Color.grey;
		GUILayout.Label("Set the state below. The State is linked to the State objects in order listed in the PSVR object.",ActiveTextStyle);
		GUILayout.Box("", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});
		stateName = EditorGUILayout.TextArea(stateName, GUILayout.MaxHeight(75) );

		if (GUILayout.Button ("Set State to the Text Above")) {
			((ProceduralSoundVerticalRemixer)target).SetState (stateName);
		}
	}
}