using System;
using UnityEngine;

public static class Utilities
{
	#region Math
	public static float MapFloat(float value, float min1, float max1, float min2, float max2){
		return (((value - min1) / (max1 - min1)) * (max2 - min2)) + min2;
	}

	public static bool FloatIsBetween(float number, float a, float b){
		if ((number < a && number > b)||(number < b && number > a))
			return true;
		return false;
	}
	#endregion
	#region Audio
	public static AudioSource HierarchySearchForAudioSource(GameObject gameObject){
		return (gameObject.GetComponent<AudioSource> () != null) ? gameObject.GetComponent<AudioSource> () : 
			HierarchySearchForAudioSource (gameObject.transform.parent.gameObject);
	}
	#endregion
}


