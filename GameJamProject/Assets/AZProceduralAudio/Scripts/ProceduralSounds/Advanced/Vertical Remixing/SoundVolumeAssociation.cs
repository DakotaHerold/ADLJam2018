using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SoundVolumeAssociation : MonoBehaviour{
	// Audio File associated with the probability
	[SerializeField]
	public ProceduralSound Sound;
	[SerializeField]
	public float Volume;
}