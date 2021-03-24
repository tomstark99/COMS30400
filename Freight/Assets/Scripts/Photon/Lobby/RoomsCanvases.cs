using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomsCanvases : MonoBehaviour
{
    [SerializeField]
    private CreateOrJoinRoomCanvas createOrJoinRoomCanvas;
    public CreateOrJoinRoomCanvas CreateOrJoinRoomCanvas { get { return createOrJoinRoomCanvas; } }

    [SerializeField]
    private CurrentRoomCanvas currentRoomCanvas;
    public CurrentRoomCanvas CurrentRoomCanvas { get { return currentRoomCanvas; } }

    [SerializeField]
    private OptionsCanvas optionsCanvas;
    public OptionsCanvas OptionsCanvas { get { return optionsCanvas; } }

    private void Awake()
    {
        FirstInitalise();
    }
    
    // initalises the canvases so you can have direct reference to them later
    private void FirstInitalise()
    {
        CreateOrJoinRoomCanvas.Initialise(this);
        CurrentRoomCanvas.Initialise(this);
        OptionsCanvas.Initialise(this);
    }

}
