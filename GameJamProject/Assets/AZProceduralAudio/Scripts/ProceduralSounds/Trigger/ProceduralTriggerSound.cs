using UnityEngine;

public class ProceduralTriggerSound : SimpleProceduralSound {
	public AudioClip Sound;

	// Use this for initialization
	void Start () {
		Type = PROCEDURAL_SOUND_TYPE.TRIGGER;
	}

	protected override AudioClip DetermineSound ()
	{
		return Sound;
	}

	public override void Play(){
		Debug.Log ("Playing TriggerSound: " + gameObject.name);
		if (Active) {
			base.Play();
		}
	}
		
	public override void Stop(){
		timePlaying = 0;
		base.Stop();
	}

	public override void UpdateSound () {
		IsPlaying = Source.isPlaying;
		base.UpdateSound ();
	}
}
