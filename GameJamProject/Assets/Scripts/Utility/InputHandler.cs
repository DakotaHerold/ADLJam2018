using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerInputData
{
    // Player input variables go here
}

public class InputHandler : Singleton<InputHandler>
{
    // Rewired variables
    private IList<Rewired.Player> rewiredPlayers;

    private List<PlayerInputData> players;
    public List<PlayerInputData> Players { get { return players; } }

    // Use this for initialization
    void Awake()
    {
        players = new List<PlayerInputData>();
        rewiredPlayers = Rewired.ReInput.players.Players;

        for (int iPlayer = 0; iPlayer < rewiredPlayers.Count; ++iPlayer)
        {
            players.Add(new PlayerInputData());
        }

    }

    // Update is called once per frame
    void Update()
    {
        for (int iPlayer = 0; iPlayer < rewiredPlayers.Count; ++iPlayer)
        {
            // Update input from rewired for each player here 
            // Example:  players[iPlayer].isJumpDown = rewiredPlayers[iPlayer].GetButtonDown(RewiredConsts.Action.Jump);
        }
    }
}


