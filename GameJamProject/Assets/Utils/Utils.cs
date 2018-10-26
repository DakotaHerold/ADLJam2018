using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace UnityUtils {
    public static class Utilities {
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
		
        public static bool SHOW_DEBUG_BUTTONS = true;
        public static bool InRange(float limit_a, float limit_b, float point) {
            if (point > limit_a && point < limit_b)
                return true;
            if (point > limit_b && point < limit_a)
                return true;
            return false;
        }
        public static Vector3 AveragePosition(List<GameObject> list) {
            Vector3 pos = Vector3.zero;
            foreach (GameObject g in list)
                pos += g.transform.position; // SUMMING POSITIONS
            pos /= list.Count;               // NORMALIZE BY SIZE OF LIST
            return pos;
        }
        public static T ParseAsEnum<T>(this string value)
        // where T : enum
        {
            if (string.IsNullOrEmpty(value)) {
                throw new ArgumentNullException
                    ("Can't parse an empty string");
            }

            Type enumType = typeof(T);
            if (!enumType.IsEnum) {
                throw new InvalidOperationException
                    ("Here's why you need enum constraints!!!");
            }

            // warning, can throw
            return (T)Enum.Parse(enumType, value);
        }

        public static void AddRange<T>(this Queue<T> queue, IEnumerable<T> enu) {
            foreach (T obj in enu)
                queue.Enqueue(obj);
        }
    }
}
namespace AZUnityExtensions
{
    namespace UnityTypeExtensions {
        public static class ColorExtensions {
            public static Color Copy(this Color col) {
                return new Color(col.r, col.g, col.b, col.a);
            }
            public static Color Copy(this Color col, float a) {
                return new Color(col.r, col.g, col.b, a);
            }
            public static Color Copy(this Color col, float r, float g, float b) {
                return new Color(r, g, b, col.a);
            }
        }
    }
    namespace UnityCollections {
        #region GENERICS    
        [System.Serializable] public class SerializableGameObjectQueue : SerializableQueue<GameObject> { }
        [UnityEditor.CustomPropertyDrawer(typeof(SerializableGameObjectQueue))]
        public class SerializableGameObjectQueueDrawer : SerializableQueueDrawer<GameObject> { }

        [System.Serializable] public class SerializeableNamedGameObjectDictionary : SerializableDictionary<string, GameObject> { }
        [UnityEditor.CustomPropertyDrawer(typeof(SerializeableNamedGameObjectDictionary))]
        public class SerializableNamedGameObjectDictionaryDrawer : SerializableDictionaryDrawer<string, GameObject> { }

        [System.Serializable] public class SerializeableNamedColorListDictionary : SerializableDictionary<string, List<string>> { }
        [UnityEditor.CustomPropertyDrawer(typeof(SerializeableNamedColorListDictionary))]
        public class SerializableNamedColorListDictionaryDrawer : SerializableDictionaryDrawer<string, List<string>> { }
        #endregion
    }
}