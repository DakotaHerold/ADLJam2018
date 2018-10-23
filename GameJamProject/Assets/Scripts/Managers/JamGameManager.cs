using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jam
{
    public class JamGameManager : MonoBehaviour {

        //public enum GAME_STATE
        //{
        //    MENU,
        //    RUNNING,
        //    RESTART
        //}

        //private GAME_STATE gameState; 

        private DataManager dataManager;
        private AreaManager areaManager;
        private GameCanvasManager gameCanvasManager;
        private AZProceduralAudioManager audioManager;
        private TriggerSoundGenerator soundGenerator; 

        // Use this for initialization
        void Start() {
            //audioManager = FindObjectOfType<AZProceduralAudioManager>();
            //soundGenerator = FindObjectOfType<TriggerSoundGenerator>(); 
            dataManager = GetComponent<DataManager>();
            areaManager = GetComponent<AreaManager>();
            gameCanvasManager = FindObjectOfType<GameCanvasManager>();
            //gameState = GAME_STATE.MENU;
            gameCanvasManager.gameManager = this; 
            InitializeGame(); 
        }

        public void InitializeGame()
        {
            areaManager.finishedAreas = areaManager.ConstructArea(dataManager.phrases);
            areaManager.ConstructPeople(areaManager.NumPeople, dataManager.phrases);
            gameCanvasManager.SpawnPeople(areaManager.finishedPeople);
        }

        // Update is called once per frame
        void Update() {

        }

        public void UpdatePersonText(List<PersonTrait> traits)
        {
            gameCanvasManager.ShowPersonText(traits); 
        }

        public void ClearInfoBox()
        {
            gameCanvasManager.ClearInfoBox(); 
        }

        public void PlayPersonPickupSound()
        {
            //soundGenerator.GenerateSound(); 
            //audioManager.Play("pick1");
        }
    }
}
