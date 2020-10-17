using UnityEngine;
using System.Collections;

//this enum holds the value/type of the piece
public enum PieceType
{       rock,
        pawn,
        bishop,
        king,
        queen,
        knight
}

// this script allows to hold the piece values, considering its posible movement
public class Piece : MonoBehaviour {

    // Use this for initialization
    public PieceType pieceTp;

   // the renderer is changed to create a chosing effect
	public Renderer groundRend;

	// this it the scrpt that manages the piece movement
	PieceMovement piecemovScript;

    //capured parameter
    public bool captured;

    // square references to calculate distance movement
    public Transform destinationSq, originSq;
           
    //the distance obtained from the two variables above
    public float distance;

    //the threshold used to determine wheter the piece is in possible movement
    public float th = 0.008f;

    //the color of the piece
    public int color;
           

    void Start ()
	{
        //distance allowed for the movement
        if (destinationSq!=null && originSq!=null)
        {
            distance = (destinationSq.position - originSq.position).magnitude;
        }
        else
        {
            distance = 0;
        }
        
        //the movement script
        piecemovScript = GameObject.FindGameObjectWithTag("pieceMovement").GetComponent<PieceMovement>();

    }
	

	public void onPieceExit ()
	{
		if (piecemovScript.selectedPiece != gameObject) 
		{
			groundRend.enabled=false;
		}
	}

	//this is the function that allows piece selection   0 = white  1 = black
	public void onPieceClick (int lastSelectedColor)
	{
        piecemovScript.resetSquares();

        Piece capturedPiece = null;

        //if there wasn't a selected gameobject
        if (piecemovScript.selectedPiece == null)
        {
            piecemovScript.selectedPiece = transform.gameObject;
            groundRend.enabled = true;
            //stop execution in this case
            
        }

        // set lastest piece render to false (unselect) only of there is one selected and is not the same as clicked
        else
        {
        
            piecemovScript.selectedPiece.transform.GetChild(0).GetComponent<Renderer>().enabled = false;
            piecemovScript.selectedPiece = transform.gameObject;
            groundRend.enabled = true;


        }

        piecemovScript.disableTexts();
        ////////////////////////////////////////////////////
        //the square selection mechanics is implemented here
        ////////////////////////////////////////////////////

        /////////////
        //  PAWN
        ////////////
        if (pieceTp == PieceType.pawn)
        {
            // loop accross the squares
            foreach (GameObject sq in piecemovScript.squares)
            {
               
                float distToSquare = (transform.position - sq.transform.position).magnitude;
                float angle = 0;

                //check movement of the pawn in function of the color
                if (color == 0)
                {
                    angle = Vector3.Angle(transform.forward, -(sq.transform.position - piecemovScript.selectedPiece.transform.position));
                    
                }
                else if (color == 1)
                {
                    angle = Vector3.Angle(transform.forward, (sq.transform.position - piecemovScript.selectedPiece.transform.position));
                    
                }

                //movement is posible if can move forward and is not blocked 
                // or if there is a opponent piece at the diagonal square

                capturedPiece = GetOccupiedDiffCol(sq);

                if (((Mathf.Abs(distToSquare - distance) < th && angle < 0.5f) && (capturedPiece == null && IsOccupied_sameCol(sq) == false) ||
                    (Mathf.Abs(distToSquare - distance * Mathf.Sqrt(2)) < th && (Mathf.Abs(angle - 45) < 0.5f ))
                    && capturedPiece != null
                    ) && IsBlocked(sq) == false && captured==false)
                {
                    //the square is activated and can be selected 
                    //set the captured piece
                   
                    sq.GetComponent<ChessSquare>().active(capturedPiece);
                }

                }
        }

        /////////////
        //  knight
        ////////////

        else if (pieceTp == PieceType.knight)
        {
            // loop accross the squares
            foreach (GameObject sq in piecemovScript.squares)
            {
                float distToSquare = (transform.position - sq.transform.position).magnitude;

                //Debug.Log(distToSquare);
                capturedPiece = GetOccupiedDiffCol(sq);
                if ((Mathf.Abs(distToSquare - distance) < th) && IsOccupied_sameCol(sq) == false && IsBlocked(sq) == false && captured == false)
                {
                    sq.GetComponent<ChessSquare>().active(capturedPiece);

                }
            }
        }

        /////////////
        //  bishop
        ////////////
        else if (pieceTp == PieceType.bishop)
        {
            foreach (GameObject sq in piecemovScript.squares)
            {
                //check if a piece is on the square
                

                float angle = Vector3.Angle(transform.forward, -(sq.transform.position - piecemovScript.selectedPiece.transform.position));
                capturedPiece = GetOccupiedDiffCol(sq);

                if ((Mathf.Abs(angle - 45) < 0.5f
                    || Mathf.Abs(angle - 135) < 0.5f
                    || Mathf.Abs(angle - 225) < 0.5f
                    || Mathf.Abs(angle - 315) < 0.5f) && IsOccupied_sameCol(sq) == false && IsBlocked(sq) == false && captured == false)
                {
                    sq.GetComponent<ChessSquare>().active(capturedPiece);

                }

            }
        }
        /////////////
        //  ROOK
        ////////////
        else if (pieceTp == PieceType.rock)
        {
            foreach (GameObject sq in piecemovScript.squares)
            {
                float angle = Vector3.Angle(transform.forward, (sq.transform.position - piecemovScript.selectedPiece.transform.position));
                capturedPiece = GetOccupiedDiffCol(sq);

                if ((angle < 0.5f || Mathf.Abs(angle - 90) < 0.5f || Mathf.Abs(angle + 90) < 0.5f || Mathf.Abs(angle - 180) < 0.5f) && IsOccupied_sameCol(sq) == false && IsBlocked(sq) == false && captured == false)
                {
                    sq.GetComponent<ChessSquare>().active(capturedPiece);

                }

            }
        }

        ////////////
        // QUEEN
        ///////////
        else if (pieceTp == PieceType.queen)
        {
            foreach (GameObject sq in piecemovScript.squares)
            {
                float angle = Vector3.Angle(transform.forward, (sq.transform.position - piecemovScript.selectedPiece.transform.position));
                capturedPiece = GetOccupiedDiffCol(sq);
                if ((Mathf.Abs(angle - 45) < 0.5f
                     || Mathf.Abs(angle - 135) < 0.5f
                     || Mathf.Abs(angle - 225) < 0.5f
                     || Mathf.Abs(angle - 315) < 0.5f
                     || angle < 0.5f || Mathf.Abs(angle - 90) < 0.5f || Mathf.Abs(angle + 90) < 0.5f || Mathf.Abs(angle - 180) < 0.5f)&& IsOccupied_sameCol(sq) == false && IsBlocked(sq) == false && captured == false)
                {
                    sq.GetComponent<ChessSquare>().active(capturedPiece);

                }

            }
        }

        ////////////
        // king
        ///////////
        else if (pieceTp == PieceType.king)
        {
            foreach (GameObject sq in piecemovScript.squares)
            {
                capturedPiece = GetOccupiedDiffCol(sq);
                float distToSquare = (transform.position - sq.transform.position).magnitude;

                if ((Mathf.Abs(distToSquare - distance) < th || Mathf.Abs(distToSquare - distance * Mathf.Sqrt(2)) < th) && IsOccupied_sameCol(sq) == false
                    && IsBlocked(sq) == false
                    && piecemovScript.CheckCheck(color, sq.transform)==false && captured == false)
                {
                    sq.GetComponent<ChessSquare>().active(capturedPiece);

                }

            }
        }


    }


    //OCCOUPIED  FUNCTIONS
    public bool  IsOccupied_sameCol(GameObject sq)
    {
        bool occupied = false;

        foreach (Piece pc in piecemovScript.allPieces)
        {
                occupied = occupied || ((pc.transform.position - sq.transform.position).magnitude < th && color == pc.color);
        }

        return occupied;
    }


    public Piece GetOccupiedDiffCol(GameObject sq)
    {
        Piece occupied = null;

        foreach (Piece pc in piecemovScript.allPieces)
        {
            if(((pc.transform.position - sq.transform.position).magnitude < th && (1 - color) == pc.color))
            {
                occupied = pc;
            }
                        
        }

        return occupied;
    }

    public bool IsBlocked(GameObject sq)
    {
        bool blocked = false;

        Vector3 height = 0.01f * transform.up;

        RaycastHit hit;

        //this is the theretical distance between points:

        Vector3 direction = piecemovScript.selectedPiece.transform.position- sq.transform.position;
        float thDist = (direction).magnitude;

        //need to enable colliders to check the raycasts of the opposite pieces
        piecemovScript.enablePieces(1-piecemovScript.playerTurn);
        
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(sq.transform.position+height, direction , out hit, Mathf.Infinity))
        {
            if(Mathf.Abs(thDist-hit.distance)<0.1f)
            {
                blocked = false;
            }
            else
            {
                blocked = true;
            }
        }

        //disable colliders again
        piecemovScript.disablePieces(1 - piecemovScript.playerTurn);
        
        return blocked;
    }

    // this part of the code detects collision between pieces
    void OnTriggerEnter (Collider col)
	{
		if (col.gameObject.tag != "Untagged") {
			Debug.Log ("Collision Has occur with: "+ col.gameObject.name);         

        }

	}

   




}
