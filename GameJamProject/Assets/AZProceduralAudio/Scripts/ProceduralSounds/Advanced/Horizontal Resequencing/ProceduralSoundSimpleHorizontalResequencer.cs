using UnityEngine;
using UnityUtils;

// Allows for Resequencing of Individual Tracks
// Trigger / Timer hybrid: On current Sound Finished, play next. Similar to LoopSound. 
// Additionally, PSHR plays sounds based on probabilities. All probabilities in track options MUST SUM to 1.0f.
public class ProceduralSoundSimpleHorizontalResequencer : ProceduralSound, FadableSound {
	[SerializeField]
	public SoundProbabilityDictionary Tracks = new SoundProbabilityDictionary ();
	public ProceduralSound Current;

	public bool Fadable {
		get;
		set;
	}

	void Start(){
		Type = PROCEDURAL_SOUND_TYPE.ADVANCED;
	}

	public override void Play(){
		IsPlaying = true;
		Debug.Log ("Playing HORIZONTAL RESEQUENCER PROCEDURAL SOUND");
		Debug.Log ("OBJECT NAME: " + gameObject.name);
		Current.Play ();
	}

	public void PlayNext(){
		Debug.Log ("PlayNEXT");
		float total_probability = 0.0f;
		foreach (string key in Tracks.Keys) {
			if (key == "") {
				continue;
			}
			Debug.Log ("Track in HR: " + Tracks [key].name);
			total_probability += Tracks[key].Probability;
		}
		if (total_probability != 1.0f) {
			Debug.LogError ("Total probability for Horizontal Resequencing Tracks DOES NOT SUM TO 1.0f!");
			return;
		}

		float current_value = 0.0f;
		float number = Random.Range (0.0f, 1.0f);
		Debug.Log (number);
		foreach (string key in Tracks.Keys) {
			if (Utilities.FloatIsBetween (number, current_value, current_value + Tracks [key].Probability)) {
				Current = Tracks[key].Sound;
				Current.Play ();
				Debug.Log ("HR playing: " + Current.name);
				return;
			}
			current_value += Tracks [key].Probability;
		}
	}
		
	public override void Stop(){
		IsPlaying = false;
		Current.Stop ();
	}

	public override void UpdateSound(){
		if (Current == null)
			return;
		if (!Current.IsPlaying) {
			PlayNext ();
		}
	}
}

[System.Serializable]
public class SoundProbabilityDictionary : SerializableDictionary<string,ProceduralSoundProbabilityAssociation>{}

[UnityEditor.CustomPropertyDrawer(typeof(SoundProbabilityDictionary))]
public class SoundProbabilityDictionaryDrawer : SerializableDictionaryDrawer<string,ProceduralSoundProbabilityAssociation> { }