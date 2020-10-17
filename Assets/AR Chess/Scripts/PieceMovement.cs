using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class PieceMovement : MonoBehaviour {

    // Use this for initialization
    // this is the piece that is selected

    // 0=white 1=black turn
    public int playerTurn=0;

    //this is the piece that is selected by clicking on it
	public GameObject selectedPiece;
    public GameObject lastSelectedPiece;

    // the time variable for the movement
	public float timeToMove=0.5f;
    
    //the event system of the game--> need to be disabled when pieces are moving
    public EventSystem mod;

    public GameObject[] squares;

    //when a piece is captured it goes here
    public Transform whitecaptures_pos;
    public Transform blackcaptures_pos;

    //register the number of pieces
    public int nbWhitecaptures;
    public int nbBlackcaptures;

    //all pieces of the set
    public Piece[] allPieces;
    public Piece[] whitePieces, blackPieces;

    //images to set turn
    public Text turnWhiteText,turnBlackText;

    public Piece kingPieceW;
    public Piece kingPieceB;

    public float th = 0.008f;

    void Start ()
	{
        //get the squares
        squares = GameObject.FindGameObjectsWithTag("chessSquare");

        //get all the pieces
        GameObject[] go=GameObject.FindGameObjectsWithTag("Piece");
        allPieces = new Piece[go.Length];
        whitePieces=new Piece[go.Length/2];
        blackPieces= new Piece[go.Length/2];
        
        int jj = 0;
        int kk = 0;
        //determine white and black pieces
        for (int ii=0;ii<go.Length;ii++)
        {
            allPieces[ii] = go[ii].GetComponent<Piece>();


            if (allPieces[ii].color==0)
            {
                if (allPieces[ii].pieceTp == PieceType.king)
                {
                    kingPieceW = allPieces[ii];
                }

                whitePieces[jj] = allPieces[ii];
                jj++;
            }
            if (allPieces[ii].color == 1)
            {
                if (allPieces[ii].pieceTp == PieceType.king)
                {
                    kingPieceB = allPieces[ii];
                }
                blackPieces[kk] = allPieces[ii];
                kk++;
            }
        }
        disablePieces(1);
    }


    //events to enable/disable colliders
    public void enablePieces(int col)
    {
        if (col == 0)
        {
            for (int ii = 0; ii < whitePieces.Length; ii++)
            {
                whitePieces[ii].GetComponent<Collider>().enabled = true;

            }
            
        }
        else
        {
            for (int ii = 0; ii < whitePieces.Length; ii++)
            {
                blackPieces[ii].GetComponent<Collider>().enabled = true;

            }
            

        }
    }
    public void disablePieces(int col)
    {
        if (col == 0)
        {
            for (int ii = 0; ii < whitePieces.Length; ii++)
            {
                whitePieces[ii].GetComponent<Collider>().enabled = false;

            }
            
        }
        else
        {
            for (int ii = 0; ii < whitePieces.Length; ii++)
            {
                blackPieces[ii].GetComponent<Collider>().enabled = false;

            }
           
        }
    }



    //call a corrutine to start movement
    public void movePiece (GameObject groundPoint, Piece pc)
	{
		Debug.Log("move piece = "+selectedPiece.name+ " to "+ groundPoint.name);
		StartCoroutine( moveToObjective(groundPoint.transform,pc));
              
    }


    //this calls the second corrutine to move the piece away from the board 
    public void capturePieceCorrutine(Piece piece)
    {
        
        StartCoroutine(capturePiece(piece));

        Debug.Log("capture piece");
        
    }

    IEnumerator moveToObjective (Transform tf, Piece capturedPiece)
	{
        //disable event system
        mod.enabled = false;

        float elapsed = 0;
        Vector3 origin = selectedPiece.transform.position;

		// linear movement
		while (elapsed <= timeToMove) 
		{
			selectedPiece.transform.position = (tf.position - origin) * elapsed / timeToMove + origin;
			elapsed += Time.fixedDeltaTime;

			yield return new WaitForFixedUpdate ();
		}

        lastSelectedPiece = selectedPiece;
        // deactivate the piece
        selectedPiece.transform.GetChild (0).GetComponent<Renderer> ().enabled = false;
		selectedPiece.transform.position = tf.position;
		selectedPiece = null;

        
       
        //CAPTURNG LOGIC
        if(capturedPiece!=null)
        {
            capturePieceCorrutine(capturedPiece);
        }
        else
        {
            //enable event system

            mod.enabled = true;
            resetSquares();
            changePlayer();
        }


    }
    public void changePlayer()
    {
        if (playerTurn == 0)
        {
            playerTurn = 1;
            turnBlackText.enabled = true;
        }
        else
        {
            playerTurn = 0;
            turnWhiteText.enabled = true;
        }

        enablePieces(playerTurn);
        disablePieces(1-playerTurn);
    }

    public void disableTexts()
    {
        turnWhiteText.enabled = false;
        turnBlackText.enabled = false;
    }

    public void resetSquares()
    {
        foreach (GameObject sq in squares)
        {
            sq.GetComponent<ChessSquare>().onExitSquare();
        }
    }


    IEnumerator capturePiece(Piece piece)
    {
        //disable event system
        mod.enabled = false;

        float elapsed2 = 0;

        Vector3 destination;
        Vector3 origin =piece.transform.position;

        if (piece.color == 0)
        {
            destination = whitecaptures_pos.position + ((float)nbWhitecaptures /30 )* whitecaptures_pos.right;
        }
        else
        {
            destination = blackcaptures_pos.position + ((float)nbBlackcaptures /30) * blackcaptures_pos.right;  
        }


        // linear movement
        while (elapsed2 <= timeToMove)
        {
            piece.transform.position = (destination-origin) * elapsed2 / timeToMove + origin;

            elapsed2 += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        //enable event system
        mod.enabled = true;

        //increment  captures
        if (piece.color == 0)
        {
            nbWhitecaptures++;
        }
        else
        {
            nbBlackcaptures++;
        }

        piece.captured = true;

        if (piece.pieceTp == PieceType.king && piece.captured == true)
        {
            Debug.Log("Checkmate!");
            Application.Quit();
        }
        else
        {
            resetSquares();
            changePlayer();
        }
    }

    //verify if a check is being produced generally at the end of a turn
    public bool CheckCheck(int col, Transform tf)
    {
        bool check = false;

        Piece[] pieces;
  
        if (col == 0)
        {
            pieces = blackPieces;
        }
        else
        {
            pieces = whitePieces;
        }


        foreach (Piece pc in pieces)
        {
            /////////////
            //  PAWN
            ////////////

            if (pc.pieceTp == PieceType.pawn)
            {
                float distToKing = (pc.transform.position - tf.position).magnitude;
                float angle = 0;
                //check posible directions of the pawn refered to the king
                if (pc.color == 0)
                {
                    angle = Vector3.Angle(transform.forward, -(pc.transform.position - tf.position));
                    if (Mathf.Abs(distToKing - pc.distance * Mathf.Sqrt(2)) < th && (Mathf.Abs(angle - 45) < 0.5f) && pc.captured == false)
                    {
                        check = true;
                    }

                }
                else if (pc.color == 1)
                {
                    angle = Vector3.Angle(transform.forward, (pc.transform.position - tf.position));
                    if (Mathf.Abs(distToKing - pc.distance * Mathf.Sqrt(2)) < th && (Mathf.Abs(angle - 135) < 0.5f) && pc.captured == false)
                    {
                        check = true;
                    }
                }
                //movement is posible if can move forward and is not blocked 
                // or if there is a opponent piece at the diagonal square
            }
            /////////////
            //  KNIGHT
            ////////////
            else if (pc.pieceTp == PieceType.knight)
            {
                float distToKing = (pc.transform.position - tf.position).magnitude;


                if (Mathf.Abs(distToKing - pc.distance ) < th && pc.captured == false)
                {

                    check = true;
                }
            }
            /////////////
            //  BISHOP
            ////////////
            else if (pc.pieceTp == PieceType.bishop)
            {
                float angle =Vector3.Angle(transform.forward, (pc.transform.position - tf.position));

                //diagonal movement
                //Debug.Log(angle);

                if ((Mathf.Abs(angle - 45) < 0.5f
                    || Mathf.Abs(angle - 135) < 0.5f
                    || Mathf.Abs(angle - 225) < 0.5f
                    || Mathf.Abs(angle - 315) < 0.5f)&& pc.captured == false && RaycastIsBlocked(pc, tf) == false)
                {

                    check = true;
                }
            }
            /////////////
            //  ROOK
            ////////////
            else if (pc.pieceTp == PieceType.rock)
            {
                float angle = Vector3.Angle(transform.forward, (pc.transform.position - tf.position));

                //linear movement
                //Debug.Log(angle);

                if ((Mathf.Abs(angle - 0) < 0.5f
                    || Mathf.Abs(angle - 90) < 0.5f
                    || Mathf.Abs(angle - 180) < 0.5f
                    || Mathf.Abs(angle - 270) < 0.5f) && pc.captured == false && RaycastIsBlocked(pc, tf) == false)
                {

                    check = true;
                }
            }

            ////////////
            // QUEEN
            ///////////
            else if (pc.pieceTp == PieceType.queen)
            {
                float angle = Vector3.Angle(transform.forward, (pc.transform.position - tf.position));

                //linear movement
                //Debug.Log(angle);

                if ((Mathf.Abs(angle - 0) < 0.5f
                    || Mathf.Abs(angle - 90) < 0.5f
                    || Mathf.Abs(angle - 180) < 0.5f
                    || Mathf.Abs(angle - 270) < 0.5f
                    || Mathf.Abs(angle - 45) < 0.5f
                    || Mathf.Abs(angle - 135) < 0.5f
                    || Mathf.Abs(angle - 225) < 0.5f
                    || Mathf.Abs(angle - 315) < 0.5f) && pc.captured == false && RaycastIsBlocked(pc, tf) == false)
                {

                    check = true;
                }
            }

            ////////////
            // KING
            ///////////
            else if (pc.pieceTp == PieceType.king)
            {
                float distToKing = (pc.transform.position - tf.position).magnitude;
                
                if ( (Mathf.Abs(distToKing - pc.distance * Mathf.Sqrt(2)) < th || Mathf.Abs(distToKing - pc.distance)<th ) && pc.captured == false)
                {
                    check = true;
                }   
            }
        }

        if (check == true)
        {
            Debug.Log("Check!");
        }
        return check;
    }



    public bool RaycastIsBlocked(Piece pc, Transform tf)
    {
        bool blocked = false;

        Vector3 height = 0.01f * transform.up;

        RaycastHit hit;

        //this is the theretical distance between points:

        Vector3 direction = tf.position - pc.transform.position;
        float thDist = (direction).magnitude;

        //need to enable colliders to check the raycasts of the opposite pieces
        enablePieces(1 - playerTurn);

        // Does the ray intersect any objects excluding the player layer?
        if (Physics.Raycast(pc.transform.position + height, direction, out hit, Mathf.Infinity))
        {
            //Debug.Log("Hit_dist="+hit.distance+" dist="+thDist);
            if (hit.distance>thDist)
            {
                blocked = false;
            }
            else
            {
                blocked = true;
            }
        }

        //disable colliders again
        disablePieces(1 - playerTurn);

        return blocked;
    }

}
