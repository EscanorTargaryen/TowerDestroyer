using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameObject target;
    public static GameObject maria;
    public static GameObject mutant;
    private GameObject Camp;
    private bool canPlace = true;
    public int logs;
    public TMP_Text logsText;
    public TMP_Text timerText;
    List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();
    public bool CampButtonSelected = false;
    public int campCost = 1;
    public GameObject GameUI;
    public static GameManager instance;
    public static GameObject Tree;

    public Button CampButton;
    public GraphicRaycaster GraphicRaycaster;

    public static List<GameObject> mariaList = new List<GameObject>();
    
    private void Awake()
    {
        instance = this;
        maria = (GameObject)Resources.Load("prefab/Maria", typeof(GameObject));
        mutant = (GameObject)Resources.Load("prefab/Mutant", typeof(GameObject));
        Tree = (GameObject)Resources.Load("prefab/Tree", typeof(GameObject));
        Camp = (GameObject)Resources.Load("prefab/Camp", typeof(GameObject));
        GameUI.SetActive(false);
    }

 

    private void spawnTree()
    {
        if (PlanAnchor.playing)
        {
            Vector3 pos = PlanAnchor.instance.selectedPlane.center;

            pos.x += Random.Range(-0.5f, 0.5f);
            pos.z += Random.Range(-0.5f, 0.5f);

            Instantiate(Tree, pos, Quaternion.identity);
        }
    }

    private float timer;
    private float timePassed;

    void Update()
    {
        if (!GameOverScreen.GameOver)
        {
            
            TimeSpan t = TimeSpan.FromSeconds( timer );
            timer += Time.deltaTime;

            string timerString = string.Format("{0:D2}h:{1:D2}m:{2:D2}s", 
                t.Hours, 
                t.Minutes, 
                t.Seconds);

            timerText.text = timerString;
            
            ForceSelectGameObject();

            timePassed += Time.deltaTime;
            if (timePassed > 5)
            {
                spawnTree();

                timePassed = 0;
            }

            if (logs >= campCost && canPlace)
            {
                CampButton.interactable = true;
            }
            else
            {
                CampButton.interactable = false;
                CampButtonSelected = false;
            }

            if (Input.touchCount > 0)
            {
                PointerEventData ped = new PointerEventData(null);

                ped.position = Input.GetTouch(0).position;

                List<RaycastResult> results = new List<RaycastResult>();

                GraphicRaycaster.Raycast(ped, results);

                if (results.Count > 0)
                {
                    return;
                }

                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                    {
                        GameObject obj = hit.collider.gameObject;
                        // PlanAnchor.instance.text.text = hit.collider.tag;

                        if (hit.collider.tag.Equals("Tree"))
                        {
                            Animator animator = obj.GetComponent<Animator>();
                            TreeScript s = obj.GetComponent<TreeScript>();
                            if (s.canTake)
                            {
                                animator.SetTrigger("Decap");
                                s.canTake = false;
                                logs++;
                                UpdateLogs();
                                StartCoroutine(removeObj(obj, 2));
                                return;
                            }
                        }
                    }
                }

                if (PlanAnchor.playing && canPlace && logs >= campCost && CampButton.IsInteractable() &&
                    EventSystem.current.currentSelectedGameObject == CampButton.gameObject)
                {
                    if (target == null)
                    {
                        target = GameObject.FindWithTag("Target");
                    }


                    if (
                        PlanAnchor.instance.m_RaycastManager.Raycast(Input.GetTouch(0).position, m_Hits))
                    {
                        HandleRaycast(m_Hits[Random.Range(0, m_Hits.Count)]);
                    }
                }
            }
        }
    }

    private void ForceSelectGameObject()
    {
        if (CampButtonSelected)
            EventSystem.current.SetSelectedGameObject(CampButton.gameObject);
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private IEnumerator removeObj(GameObject gameObject, float secs)
    {
        yield return new WaitForSeconds(secs);
        Destroy(gameObject);
    }

    void HandleRaycast(ARRaycastHit hit)
    {
        if ((hit.hitType & TrackableType.Planes) != 0)
        {
            Vector3 pos = hit.pose.position;

            pos.y += 0.15f;

            GameObject m = Instantiate(Camp, pos, hit.pose.rotation);
            logs -= campCost;
            UpdateLogs();
            canPlace = false;
            StartCoroutine(wait());
        }
    }

    private void UpdateLogs()
    {
        logsText.text = logs + "";
    }

    public IEnumerator wait()
    {
        yield return new WaitForSeconds(5f);
        canPlace = true;
    }

    public void campcClick()
    {
        CampButtonSelected = !CampButtonSelected;
    }
}