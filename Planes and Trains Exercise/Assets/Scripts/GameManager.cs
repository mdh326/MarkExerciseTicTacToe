using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //tictactoe Grid: 0 = empty; 1 = X (plane); 2 = O (train)
    //Each array is treated as x and y respectively, starting in bottom left. Therefore top middle is gridCon[1][2]
    private int[][] gridItems = new int[][] {
         new int[] {0,0,0},
         new int[] {0,0,0},
         new int[] {0,0,0}
     };

    private GridSpot[][] gridObj = new GridSpot[][] {
         new GridSpot[3],
         new GridSpot[3],
         new GridSpot[3]
     };

    private int currPlayer = 1;
    public GameObject[] PlayerMaker;
    public Animator PlaneTurnAnimator;
    public Animator TrainTurnAnimator;

    //Input Variables
    private Ray inputRay;
    private bool touchRegistered = false;
    private bool touchAnimPlayed = false;
    private float touchWait = 0.1f;
    private float touchTimer = 0;

    //Victory Variables
    private bool winnerFound = false;
    private bool gameOver = false;
    public Animator VictoryScreenAnimator;
    public Text VictoryText;
    public Image VictoryImage;
    public Sprite[] VictorySprites;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0) && !gameOver && Input.touchCount <= 0) {
            inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(inputRay.origin, inputRay.direction*15f, Color.red);
            RaycastHit2D hit = Physics2D.Raycast(inputRay.origin, inputRay.direction);

            if(hit.collider != null) {

                UpdateGrid(hit.transform.GetComponent<GridSpot>());

            }
        }

         if (Input.touchCount > 0 && !gameOver && !touchRegistered) {
            //stop error visual from registering multiple hits
            touchRegistered = true;
            touchTimer = touchWait;

            Touch touch = Input.GetTouch(0);
            inputRay = Camera.main.ScreenPointToRay(touch.position);
            Debug.DrawRay(inputRay.origin, inputRay.direction*15f, Color.red);
            RaycastHit2D hit = Physics2D.Raycast(inputRay.origin, inputRay.direction);

            if(hit.collider != null) {
                UpdateGrid(hit.transform.GetComponent<GridSpot>());

            }
        }
        
        //wait a bit before registering brand new touch and giving appropriate visual feedback
        if (touchRegistered) {
            touchTimer -= Time.deltaTime;
            if (touchTimer <= 0f) {
                touchRegistered = false;
                touchAnimPlayed = false;
            }
        }

    }


    void UpdateGrid(GridSpot GridspotScr) {
        int xPos = GridspotScr.myX;
        int yPos = GridspotScr.myY;

        //Check the grid position is not empty
        if (gridItems[xPos][yPos] == 0) {
            gridItems[xPos][yPos] = currPlayer;

            gridObj[xPos][yPos] = GridspotScr;
            GridspotScr.DisplayMarker(PlayerMaker[currPlayer-1]);

            if(touchRegistered){
                touchAnimPlayed = true;
            }

            CheckVictoryState();

            if (!gameOver){
                ChangeTurn();
            }

        // if space is already occupied
        } else {

            if (!touchAnimPlayed){
                GridspotScr.DisplayError();
            }

        }
    }


    void CheckVictoryState() {
        //check for win
        for (int VicX = 0; VicX <= 2; VicX++) {
            for (int VicY = 0; VicY <= 2; VicY++) {

                //check if all values are inside Grid
                if(VicY - 1 >= 0 && VicY + 1 <= 2) {
                    //check vertical
                    if (gridItems[VicX][VicY] != 0 && gridItems[VicX][VicY] == gridItems[VicX][VicY - 1] && gridItems[VicX][VicY] == gridItems[VicX][VicY + 1]) {
                       // Debug.Log("Winner is P" + gridItems[VicX][VicY]);

                        gridObj[VicX][VicY - 1].DisplayWin();
                        gridObj[VicX][VicY].DisplayWin();
                        gridObj[VicX][VicY + 1].DisplayWin();

                        winnerFound = true;
                        GameOver(gridItems[VicX][VicY]);
                    }
                }

                //check if all values are inside Grid
                if(VicX - 1 >= 0 && VicX + 1 <= 2) {
                    //check horizontal
                    if (gridItems[VicX][VicY] != 0 && gridItems[VicX][VicY] == gridItems[VicX - 1][VicY] && gridItems[VicX][VicY] == gridItems[VicX + 1][VicY]) {
                        //Debug.Log("Winner is P" + gridItems[VicX][VicY]);

                        gridObj[VicX - 1][VicY].DisplayWin();
                        gridObj[VicX][VicY].DisplayWin();
                        gridObj[VicX + 1][VicY].DisplayWin();

                        winnerFound = true;
                        GameOver(gridItems[VicX][VicY]);
                    }
                }

                //check if we're testing center of grid
                if(VicX == 1 && VicY == 1) {
                    //check diagonal 1 /
                    if (gridItems[VicX][VicY] != 0 && gridItems[VicX][VicY] == gridItems[VicX - 1][VicY - 1] && gridItems[VicX][VicY] == gridItems[VicX + 1][VicY + 1]) {
                        //Debug.Log("Winner is P" + gridItems[VicX][VicY]);

                        gridObj[VicX - 1][VicY - 1].DisplayWin();
                        gridObj[VicX][VicY].DisplayWin();
                        gridObj[VicX + 1][VicY + 1].DisplayWin();

                        winnerFound = true;
                        GameOver(gridItems[VicX][VicY]);
                    }

                    //check diagonal 2 \
                    if (gridItems[VicX][VicY] != 0 && gridItems[VicX][VicY] == gridItems[VicX - 1][VicY + 1] && gridItems[VicX][VicY] == gridItems[VicX + 1][VicY - 1]) {
                        //Debug.Log("Winner is P" + gridItems[VicX][VicY]);

                        gridObj[VicX - 1][VicY + 1].DisplayWin();
                        gridObj[VicX][VicY].DisplayWin();
                        gridObj[VicX + 1][VicY - 1].DisplayWin();

                        winnerFound = true;
                        GameOver(gridItems[VicX][VicY]);
                    }
                }
            }
        }
        
        //check for game over
        if (!winnerFound) {
            int spotsRemaining = 9;

            for (int VicX = 0; VicX <= 2; VicX++) {
                for (int VicY = 0; VicY <= 2; VicY++){
                    //how many grid spaces are still empty
                    if(gridItems[VicX][VicY] != 0) {

                        spotsRemaining--;

                    }
                }
            }

            if(spotsRemaining <= 0) {
                //Debug.Log("Game Over");
                GameOver(0);
            }
        }
    }


    void ChangeTurn() {
        if (currPlayer == 1) {
            currPlayer = 2;
            PlaneTurnAnimator.SetTrigger("TriggerLeft");
            TrainTurnAnimator.SetTrigger("TriggerLeft");
        } else {
            currPlayer = 1;
            PlaneTurnAnimator.SetTrigger("TriggerRight");
            TrainTurnAnimator.SetTrigger("TriggerRight");
        }
    }


    void GameOver(int winnerVal) {
        gameOver = true;

        if (currPlayer == 1) {
            PlaneTurnAnimator.SetTrigger("TriggerLeft");
        } else {
            TrainTurnAnimator.SetTrigger("TriggerRight");
        }

        if(winnerVal == 1) {
            VictoryText.text = "PLANE WINS";
        } else if (winnerVal == 2) {
            VictoryText.text = "TRAIN WINS";
        } else {
            VictoryText.text = "TIE";
        }

        VictoryImage.sprite = VictorySprites[winnerVal];

        VictoryScreenAnimator.SetTrigger("GameOver");

    }
}
