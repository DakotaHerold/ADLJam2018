using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralSoundProbabilityAssociation : MonoBehaviour {
	// Audio File associated with the probability
	public ProceduralSound Sound;

	// 0 to 1: 1 is maximum probability for Clip Appearance
	[Range(0.0f, 1.0f)]
	public float Probability;
}
