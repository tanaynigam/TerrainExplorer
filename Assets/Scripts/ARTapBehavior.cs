using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ARTapBehavior : MonoBehaviour
{

    [Header("AR Objects")]
    public GameObject TerrainParent;
    public GameObject PlaneIndicatorQuad;
    public GameObject InfoObject;

    [Header("RayCast config")]
    public float maxRayDistance = 3000.0f;
    public LayerMask collisionLayer = 1 << 10;

    [Header("Object Scaling")]
    public float TerrainMaxScale;
    public float TerrainMinScale;

    public float TerrainEnlargedScale;

    public float PlaneIndicatorQuadMaxScale;
    public float PlaneIndicatorQuadMinScale;

    public float PlaneIndicatorQuadScalingTime;
    public float TerrainScalingTime;

    public float TerrainMaxScalingTime;

    [Header("Info Object")]
    public Text InfoText;
    public float InfoTextLocalScale;
    public float LatitudeAdjustmemt;
    public float LongitudeAdjustment;
    public float AltitudeAdjustment;

    [Header("Points of Interests")]
    public GameObject POIInterestObjectParent;

    [Header("Character Speed")]
    public float speed;
    public float CharacterHeight;
    //State Tracking
    public int ARState;
    private bool AnimatingQuadIndicator = false;

    private Vector3 TerrainARPosition;
    private List<GameObject> TaggedObjects;

    private Vector3 UNIT_SCALE = new Vector3(1, 1, 1);


    void Start()
    {
        ARState = 0;
        TaggedObjects = new List<GameObject>();
    }

    void Update()
    {
        switch (ARState)
        {
            case 0:
                PlaneIndicatorQuadBehaviour();
                break;
            case 1:
                InfoPopUp();
                break;
            case 2:
                POIPopUp();
                break;
            case 3:
                // CountTaps();
                EnterTerrainTapped();
                break;
            case 4:
                MaintainAboveGround();
                break;
            default:
                break;
        }
    }

    #region State 0 Behaviour

    void PlaneIndicatorQuadBehaviour()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxRayDistance))
        {
            if (hit.collider.tag == "Plane")
            {
                if (!PlaneIndicatorQuad.activeSelf)
                {
                    if (!AnimatingQuadIndicator)
                    {
                        PlaneIndicatorQuad.SetActive(true);
                        StartCoroutine(EnlargePlaneIndicatorQuad());
                    }
                }
                PlaneIndicatorQuad.transform.position = Vector3.Lerp(PlaneIndicatorQuad.transform.position, hit.point, Time.deltaTime * 5f);
            }
        }
        else
        {
            if (PlaneIndicatorQuad.activeSelf)
            {
                if (!AnimatingQuadIndicator)
                {
                    StartCoroutine(ShrinkPlaneIndicatorQuad());
                }
            }
        }

        // if (Input.touchCount > 0)
        if (Input.GetMouseButtonDown(0))
        {
            Ray rayTouch = Camera.main.ScreenPointToRay(Input.mousePosition);
            // Ray rayTouch = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hitTouch;
            if (Physics.Raycast(rayTouch, out hitTouch, maxRayDistance))
            {
                // Debug.Log(hit.collider.gameObject);
                if (hitTouch.collider.tag == "QuadIndicator")
                {
                    TerrainParent.SetActive(true);
                    TerrainParent.transform.position = hitTouch.point;
                    TerrainARPosition = TerrainParent.transform.position;
                    Debug.Log(TerrainARPosition);
                    StartCoroutine(EnlargeTerrainOnPlane());
                    PlaneIndicatorQuad.SetActive(false);
                    StartCoroutine(SwitchState());
                    // ARState = 1;
                }
            }
        }
    }

    IEnumerator SwitchState()
    {
        yield return new WaitForSeconds(1);
        PlaneIndicatorQuad.SetActive(false);
        gameObject.GetComponentInChildren<UIBehaviour>().MainPanel.SetActive(true);
        gameObject.GetComponentInChildren<UIBehaviour>().LocationInfoButtonToggled();
    }

    #endregion

    #region State 1 Nehaviour

    void InfoPopUp()
    {
        // if (Input.touchCount > 0)
        if (Input.GetMouseButtonDown(0))
        {
            Ray rayTouch = Camera.main.ScreenPointToRay(Input.mousePosition);
            // Ray rayTouch = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hitTouch;
            if (Physics.Raycast(rayTouch, out hitTouch, maxRayDistance))
            {
                if (hitTouch.collider.tag == "Terrain")
                {
                    InfoObject.SetActive(true);
                    InfoObject.transform.position = hitTouch.point;
                    InfoObject.transform.SetParent(hitTouch.collider.transform);
                    InfoObject.transform.localScale = UNIT_SCALE * InfoTextLocalScale;
                    float latitude = LatitudeAdjustmemt + (InfoObject.transform.localPosition.x / 1843);
                    float longitude = LongitudeAdjustment + (InfoObject.transform.localPosition.z / 1855);
                    float altitude = AltitudeAdjustment + InfoObject.transform.localPosition.y;
                    // InfoObject.transform.SetParent(null);

                    InfoText.text = "Latitude : " + latitude + "\nLongitude: " + longitude + "\nAltitude: " + altitude + "m";
                }
            }
        }
    }

    #endregion

    #region State 2 Behaviour

    void POIPopUp()
    {
        // if (Input.touchCount > 0)
        if (Input.GetMouseButtonDown(0))
        {
            Ray rayTouch = Camera.main.ScreenPointToRay(Input.mousePosition);
            // Ray rayTouch = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hitTouch;
            if (Physics.Raycast(rayTouch, out hitTouch, maxRayDistance))
            {
                // Debug.Log(hitTouch.collider.name);
                if (hitTouch.collider.tag == "POI")
                {
                    int POIIndex = hitTouch.collider.gameObject.GetComponent<PointOfInterest>().POIIndex;
                    gameObject.GetComponent<UIBehaviour>().OpenPOI(POIIndex);
                }
            }
        }
    }

    #endregion

    #region  State 3 Behaviour

    void CountTaps()
    {
        if (Input.touchCount > 0)
        // if(Input.GetMouseButtonDown(0))
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.tapCount == 2)
                {
                    // Ray rayTouch = Camera.main.ScreenPointToRay(Input.mousePosition);
                    Ray rayTouch = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                    RaycastHit hitTouch;
                    if (Physics.Raycast(rayTouch, out hitTouch, maxRayDistance))
                    {
                        if (hitTouch.collider.tag == "Terrain")
                        {
                            StartCoroutine(EnterTerrain(hitTouch.point));
                        }
                    }
                }
            }

        }
    }

    void EnterTerrainTapped()
    {
        // if (Input.touchCount > 0)
        if (Input.GetMouseButtonDown(0))
        {
            Ray rayTouch = Camera.main.ScreenPointToRay(Input.mousePosition);
            // Ray rayTouch = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hitTouch;
            if (Physics.Raycast(rayTouch, out hitTouch, maxRayDistance))
            {
                if (hitTouch.collider.tag == "Terrain")
                {
                    StartCoroutine(EnterTerrain(hitTouch.point));
                }
            }
        }
    }

    IEnumerator EnterTerrain(Vector3 touchPosition)
    {
        gameObject.GetComponentInChildren<UIBehaviour>().ActivateNavigationUI();

        GameObject indicator = new GameObject();
        indicator.transform.position = touchPosition;
        indicator.transform.SetParent(TerrainParent.transform);

        // Vector3 originalScale = UNIT_SCALE * TerrainMaxScale;
        Vector3 destinationScale = UNIT_SCALE * TerrainEnlargedScale;
        
        TerrainParent.transform.localScale = destinationScale;
        // float currentTime = 0.0f;

        // do
        // {
        //     TerrainParent.transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / TerrainMaxScalingTime);
        //     currentTime += Time.deltaTime;
        //     yield return null;
        // } while (currentTime <= TerrainMaxScalingTime);



        // Vector3 originalPosition = TerrainParent.transform.position;
        Vector3 destinationPosition = TerrainParent.transform.position - indicator.transform.position + Camera.main.transform.position;

        TerrainParent.transform.position = destinationPosition;
        // currentTime = 0.0f;

        // do
        // {
        //     TerrainParent.transform.position = Vector3.Lerp(originalPosition, destinationPosition, currentTime / TerrainMaxScalingTime);
        //     currentTime += Time.deltaTime;
        //     yield return null;
        // } while (currentTime <= TerrainMaxScalingTime);

        Destroy(indicator);

        ARState = 4;
        yield return null;
    }

    #endregion

    #region State 4 Behaviour
    void MaintainAboveGround()
    {
        Vector3 RayCastPoint = Camera.main.transform.position + new Vector3(0, 1000, 0);
        
        if(TerrainParent.transform.position.y > Camera.main.transform.position.y)
            TerrainParent.transform.position = new Vector3(TerrainParent.transform.position.x, Camera.main.transform.position.y - CharacterHeight, TerrainParent.transform.position.z);
        
        // Debug.Log(RayCastPoint);
        Ray ray = new Ray(RayCastPoint, -Vector3.up);
        RaycastHit hit;
        

        if (Physics.Raycast(ray, out hit, maxRayDistance))
        {
            // Debug.DrawLine (transform.position, hit.point, Color.cyan);
            // Debug.Log(hit.collider.name);
            if (hit.collider.tag == "Terrain")
            {
                float diff = Camera.main.transform.position.y - hit.point.y - CharacterHeight;
                TerrainParent.transform.position = new Vector3(TerrainParent.transform.position.x, TerrainParent.transform.position.y + diff, TerrainParent.transform.position.z);
            }
            else
            {
                TaggedObjects.Add(hit.collider.gameObject);
                hit.collider.gameObject.layer = 2;
            }
        }
    }

    public IEnumerator ExitTerrain()
    {
        ARState = 3;

        // Vector3 originalPosition = TerrainParent.transform.position;
        Vector3 destinationPosition = TerrainARPosition;
        // Debug.Log(TerrainARPosition);

        TerrainParent.transform.position = destinationPosition;
        // float currentTime = 0.0f;

        // do
        // {
        //     TerrainParent.transform.position = Vector3.Lerp(originalPosition, destinationPosition, currentTime / TerrainMaxScalingTime);
        //     currentTime += Time.deltaTime;
        //     yield return null;
        // } while (currentTime <= TerrainMaxScalingTime);

        // Vector3 originalScale = UNIT_SCALE * TerrainEnlargedScale;

        Vector3 destinationScale = UNIT_SCALE * TerrainMaxScale;
        TerrainParent.transform.localScale = destinationScale;

        // currentTime = 0.0f;

        // do
        // {
        //     TerrainParent.transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / TerrainMaxScalingTime);
        //     currentTime += Time.deltaTime;
        //     yield return null;
        // } while (currentTime <= TerrainMaxScalingTime);

        foreach(GameObject tagged in TaggedObjects)
        {
            tagged.layer = 0;
        }

        TaggedObjects = new List<GameObject>();
        yield return null;
    }

    private bool MovingForward = false;
    private bool MovingBackward = false;

    public void MoveForwardButtonDown()
    {
        MovingForward = true;
    }

    public void MoveForwardButtonUp()
    {
        MovingForward = false;
    }

    public void MoveForwardUpdate()
    {
        if(MovingForward)
        TerrainParent.transform.Translate(-Camera.main.transform.forward * (speed) * Time.deltaTime);
    }

    public void MoveBackwardButtonDown()
    {
        MovingBackward = true;
    }

    public void MoveBackwardButtonUp()
    {
        MovingBackward = false;
    }

    public void MoveBackwardUpdate()
    {
        if(MovingBackward)
        TerrainParent.transform.Translate(Camera.main.transform.forward * (speed) * Time.deltaTime);
    }

    #endregion


    #region Scale Animations
    IEnumerator EnlargePlaneIndicatorQuad()
    {
        AnimatingQuadIndicator = true;
        PlaneIndicatorQuad.SetActive(true);
        Vector3 originalScale = UNIT_SCALE * PlaneIndicatorQuadMinScale;
        Vector3 destinationScale = UNIT_SCALE * PlaneIndicatorQuadMaxScale;

        float currentTime = 0.0f;

        do
        {
            PlaneIndicatorQuad.transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / PlaneIndicatorQuadScalingTime);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= PlaneIndicatorQuadScalingTime);

        AnimatingQuadIndicator = false;
    }

    IEnumerator ShrinkPlaneIndicatorQuad()
    {
        AnimatingQuadIndicator = true;
        Vector3 originalScale = UNIT_SCALE * PlaneIndicatorQuadMaxScale;
        Vector3 destinationScale = UNIT_SCALE * PlaneIndicatorQuadMinScale;

        float currentTime = 0.0f;

        do
        {
            PlaneIndicatorQuad.transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / PlaneIndicatorQuadScalingTime);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= PlaneIndicatorQuadScalingTime);

        PlaneIndicatorQuad.SetActive(false);
        AnimatingQuadIndicator = false;
    }

    IEnumerator EnlargeTerrainOnPlane()
    {
        // SquarePatch.SetActive(true);
        Vector3 originalScale = UNIT_SCALE * TerrainMinScale;
        Vector3 destinationScale = UNIT_SCALE * TerrainMaxScale;

        float currentTime = 0.0f;

        do
        {
            TerrainParent.transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / TerrainScalingTime);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= TerrainScalingTime);
    }
    #endregion
}
