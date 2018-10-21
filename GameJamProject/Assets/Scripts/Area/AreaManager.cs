using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Jam
{
    public struct AreaData
    {
        public Areas name;
        public Category[] categories;
        public Group[] groupTypes;
        public List<string> phrases;
    }

    public class AreaManager : MonoBehaviour
    {

        public AreaData[] ConstructArea( PhraseData [] baseData)
        {

            AreaData[] results = new AreaData[4];



            




            return results;
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}