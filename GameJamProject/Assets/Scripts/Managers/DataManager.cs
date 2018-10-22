using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq; 

namespace Jam
{
    [Serializable]
    public enum Areas
    {
        One,
        Two,
        Three,
        Four
    }

    [Serializable]
    public enum Category
    {
        Animals,
        VideoGames,
        Fitness,
        Television,
        Science,
        Education,
        Sports,
        Cooking,
        Movies,
        Reading
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
        public PhraseReadData[] WrittenData;

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

    [Serializable]
    public class PhraseReadData
    {
        public string Category;
        public string Group;
        public string Text; 
    }


    public class DataManager : MonoBehaviour
    {
        private string WrittenDataPath = "WrittenData";

        [HideInInspector]
        public PhraseData[] phrases;

        private PhraseReadData[] readData; 

        private void Awake()
        {
            readData = PhraseDataContainer.Load(WrittenDataPath).WrittenData;
            ConvertReadFormat(readData);


            // Testing 
            //if (phrases == null)
            //    Debug.Log("Null phrases");
            //else
            //{
            //    foreach (PhraseData phrase in phrases)
            //    {
            //        Debug.Log("Category: " + phrase.Category);
            //        Debug.Log("Type: " + phrase.Group);
            //        Debug.Log("Text: " + phrase.Text);
            //        Debug.Log("\n");
            //    }
            //}

            //if (readData == null)
            //    Debug.Log("Null phrases");
            //else
            //{
            //    foreach (PhraseReadData phrase in readData)
            //    {
            //        Debug.Log("Category: " + phrase.Category);
            //        Debug.Log("Type: " + phrase.Group);
            //        Debug.Log("Text: " + phrase.Text);
            //        Debug.Log("\n");
            //    }
            //}

        }

        private void ConvertReadFormat(PhraseReadData[] readData)
        {
            phrases = new PhraseData[readData.Length]; 
            for(int iData = 0; iData < readData.Length; ++iData)
            {
                PhraseReadData data = readData[iData];
                Category cat;
                cat = (Category)Enum.Parse(typeof(Category), data.Category, true);

                Group newGroup;
                newGroup = (Group)Enum.Parse(typeof(Group), data.Group, true);

                //if(cat == Category.WrongParse || newGroup == Group.WrongParse)
                //{
                //    Debug.Log("Error in data gen"); 
                //}

                phrases[iData] = new PhraseData(); 
                phrases[iData].Category = cat;
                phrases[iData].Group = newGroup;
                phrases[iData].Text = data.Text; 
            }
        }

        public PersonTrait PullPersonTrait(Category category, Group group)
        {
            PersonTrait personTrait = new PersonTrait();

            System.Random rnd = new System.Random();
            PhraseData[] randomizedPhrases = phrases.OrderBy(x => rnd.Next()).ToArray();

            foreach (PhraseData data in randomizedPhrases)
            {
                if(data.Category == category && data.Group == group)
                {
                    personTrait.Phrase = data.Text;
                    personTrait.Category = data.Category;
                    personTrait.groupType = data.Group;
                    break; 
                    //if(exclusionList != null)
                    //{
                    //    if(exclusionList.Contains(personTrait))
                    //    {
                    //        continue; 
                    //    }
                    //}
                    //else
                    //{
                    //    break; 
                    //}
                }
            }

            return personTrait; 
        }
    }
}
