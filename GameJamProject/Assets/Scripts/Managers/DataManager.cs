using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Jam
{
    [Serializable]
    public enum Areas
    {
        Red,
        Blue,
        White,
        Yellow
    }

    [Serializable]
    public enum Category
    {
        Animals,
        VideoGames
    }

    [Serializable]
    public enum Group
    {
        Individual,
        Group,
        NotGroup,
        NotIndividual
    }

    [Serializable]
    public class PhraseDataContainer
    {
        // NOTE: This must be the same name as the google sheet name!
        public PhraseData[] WrittenData;

        public static PhraseDataContainer Load(string PhraseDataPath)
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, PhraseDataPath + ".json");
            //Debug.Log("Loading from path: " + filePath); 

            var serialized = File.ReadAllText(filePath);
            return JsonUtility.FromJson<PhraseDataContainer>(serialized);
        }
    }

    [Serializable]
    public class PhraseData
    {
        public Category Category;
        public Group Group;
        public string Text;
    }


    public class DataManager : MonoBehaviour
    {
        private string WrittenDataPath = "WrittenData";

        private void Start()
        {
            PhraseData[] phrases = PhraseDataContainer.Load(WrittenDataPath).WrittenData;
            //if (phrases == null)
            //    Debug.Log("Null phrases");
            //else
            //{
            //    foreach(PhraseData phrase in phrases)
            //    {
            //        Debug.Log("Category: " + phrase.Category);
            //        Debug.Log("Type: " + phrase.Group);
            //        Debug.Log("Text: " + phrase.Text);
            //        Debug.Log("\n"); 
            //    }
            //}
            
        }

    }
}
