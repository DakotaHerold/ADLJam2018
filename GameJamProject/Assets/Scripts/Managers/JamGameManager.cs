using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jam
{
    public class JamGameManager : MonoBehaviour {

        public enum GAME_STATE
        {
            MENU,
            RUNNING,
            RESTART
        }

        private GAME_STATE gameState; 

        private DataManager dataManager;
        private AreaManager areaManager;
        private GameCanvasManager gameCanvasManager; 

        // Use this for initialization
        void Start() {
            dataManager = GetComponent<DataManager>();
            areaManager = GetComponent<AreaManager>();
            gameCanvasManager = FindObjectOfType<GameCanvasManager>();
            gameState = GAME_STATE.MENU;
        }

        public void InitializeGame()
        {
            //areaManager.finishedAreas = areaManager.ConstructArea(dataManager.phrases);
            //areaManager.finishedPersonTraits = areaManager.ConstructPeople(areaManager.NumPeople, dataManager.phrases); 
            //gameCanvasManager.SpawnPeople(aream)
        }

        // Update is called once per frame
        void Update() {

        }
    }
}
