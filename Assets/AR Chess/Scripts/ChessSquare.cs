using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChessSquare : MonoBehaviour {

	// Use this for initialization

	// this it the scrpt that manages the piece movement
	PieceMovement piecemovScript;

	// this is the over material used when focusing on the square
	public Material overMat;

    // pointer functions is used to call the pepare to click
    public bool canMove;
    public Piece pieceToCapture;

	void Start ()
	{
		piecemovScript=GameObject.FindGameObjectWithTag("pieceMovement").GetComponent<PieceMovement>();
        gameObject.GetComponent<Renderer>().material = overMat;
        gameObject.GetComponent<Renderer>().enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
	
	}


	//*****************
	// call to the movement function in this case
	//*******************
	public void onClickSquare ()
	{
        if (canMove)
        {
            
            if(pieceToCapture!=null)
            {
                piecemovScript.movePiece(gameObject, pieceToCapture);
            }
            else
            {
                piecemovScript.movePiece(gameObject, null);
            }
        }

	}

    public void active(Piece pc)
    {
        gameObject.GetComponent<Renderer>().enabled =true;
        canMove = true;
        pieceToCapture = pc;

    }

	public void onExitSquare ()
	{
        gameObject.GetComponent<Renderer>().enabled = false;
        canMove = false;
        pieceToCapture = null;
        
    }
}
