using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpot : MonoBehaviour
{
    public int myX;
    public int myY;

    private GameObject myMarker;
    private Animator myAnimator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayMarker(GameObject markerObj) {
        myMarker = Instantiate(markerObj, gameObject.transform);
        myAnimator = myMarker.GetComponent<Animator>();
    }

    public void DisplayError() {
        myAnimator.SetTrigger("TriggerError");
    }

    public void DisplayWin() {
        myAnimator.SetTrigger("TriggerWin");
    }
}
