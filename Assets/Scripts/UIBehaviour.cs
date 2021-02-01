using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBehaviour : MonoBehaviour
{
    [Header("Panels")]
    public GameObject MainPanel;
    public GameObject NavigationPanel;
    public GameObject InstructionsPanel;

    [Header("Buttons")]
    public GameObject LocationInfoButton;
    public GameObject POIButton;
    public GameObject EnterTerrainButton;
    public GameObject[] NavigationButtons;

    public Sprite ActiveButtonSprite;
    public Sprite InactiveButtonSprite;

    [Header("ImagePanel")]
    public GameObject POIImageObject;
    public GameObject CancelPOIButton;
    public Sprite[] POIImages;

    public void InstructionsContinue()
    {
        InstructionsPanel.SetActive(false);
        MainPanel.SetActive(true);
    }

    public void PopInstructions()
    {
        InstructionsPanel.SetActive(true);
        MainPanel.SetActive(false);
    }

    public void OpenPOI(int index)
    {
        MainPanel.SetActive(false);
        POIImageObject.SetActive(true);
        POIImageObject.GetComponentInChildren<Image>().sprite = POIImages[index];
    }

    public void CancelPOI()
    {
        MainPanel.SetActive(true);
        POIImageObject.SetActive(false);
    }

    public void PlaceAgain()
    {
        gameObject.GetComponentInChildren<ARTapBehavior>().TerrainParent.SetActive(false);
        gameObject.GetComponentInChildren<ARTapBehavior>().PlaneIndicatorQuad.SetActive(false);
        gameObject.GetComponentInChildren<ARTapBehavior>().ARState = 0;
    }

    public void LocationInfoButtonToggled()
    {
        EnterTerrainButton.GetComponentInChildren<Image>().sprite = InactiveButtonSprite;
        POIButton.GetComponentInChildren<Image>().sprite = InactiveButtonSprite;
        LocationInfoButton.GetComponentInChildren<Image>().sprite = ActiveButtonSprite;

        EnterTerrainButton.GetComponentInChildren<Text>().color = Color.black;
        POIButton.GetComponentInChildren<Text>().color = Color.black;
        LocationInfoButton.GetComponentInChildren<Text>().color = Color.white;

        gameObject.GetComponentInChildren<ARTapBehavior>().ARState = 1;

        gameObject.GetComponentInChildren<ARTapBehavior>().POIInterestObjectParent.SetActive(false);
        gameObject.GetComponentInChildren<ARTapBehavior>().InfoObject.SetActive(false);
    }

    public void POIButtonToggled()
    {
        EnterTerrainButton.GetComponentInChildren<Image>().sprite = InactiveButtonSprite;
        POIButton.GetComponentInChildren<Image>().sprite = ActiveButtonSprite;
        LocationInfoButton.GetComponentInChildren<Image>().sprite = InactiveButtonSprite;

        EnterTerrainButton.GetComponentInChildren<Text>().color = Color.black;
        POIButton.GetComponentInChildren<Text>().color = Color.white;
        LocationInfoButton.GetComponentInChildren<Text>().color = Color.black;

        gameObject.GetComponentInChildren<ARTapBehavior>().ARState = 2;

        gameObject.GetComponentInChildren<ARTapBehavior>().POIInterestObjectParent.SetActive(true);
        gameObject.GetComponentInChildren<ARTapBehavior>().InfoObject.SetActive(false);
    }

    public void EnterTerrainButtonToggled()
    {
        EnterTerrainButton.GetComponentInChildren<Image>().sprite = ActiveButtonSprite;
        POIButton.GetComponentInChildren<Image>().sprite = InactiveButtonSprite;
        LocationInfoButton.GetComponentInChildren<Image>().sprite = InactiveButtonSprite;

        EnterTerrainButton.GetComponentInChildren<Text>().color = Color.white;
        POIButton.GetComponentInChildren<Text>().color = Color.black;
        LocationInfoButton.GetComponentInChildren<Text>().color = Color.black;

        gameObject.GetComponentInChildren<ARTapBehavior>().ARState = 3;

        gameObject.GetComponentInChildren<ARTapBehavior>().POIInterestObjectParent.SetActive(false);
        gameObject.GetComponentInChildren<ARTapBehavior>().InfoObject.SetActive(false);
    }


    public void ActivateNavigationUI()
    {
        MainPanel.SetActive(false);
        NavigationPanel.SetActive(true);
    }

    public void ExitTerrainButtonClicked()
    {
        StartCoroutine(gameObject.GetComponentInChildren<ARTapBehavior>().ExitTerrain());
        NavigationPanel.SetActive(false);
        MainPanel.SetActive(true);
    }
}
