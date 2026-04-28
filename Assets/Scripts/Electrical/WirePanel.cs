using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class WirePanel : MonoBehaviour
{
    [SerializeField] private List<Wire> wiresInPanel = new List<Wire>();
    [SerializeField] private List<Color> completeColors = new List<Color>();
    [SerializeField] private List<Color> incompleteColors = new List<Color>();
    [SerializeField] private EndCord endCord;

    private bool wiresRearranged = false;
    private bool wiresDestroyed = false;

    public bool WiresCompleted => wiresCompleted;
    private bool wiresCompleted = false;

    private void OnEnable()
    {
        WireLoad();
    }

    private void Update()
    {
        if(CheckForWiresCut() == true && wiresRearranged == false)
        {
            wiresDestroyed = true;
            RearrangeEndPoints();
            wiresRearranged = true;
        }

        if(wiresDestroyed == true && CheckForWiresCompleted() == true)
        {
            wiresCompleted = true;
        }

    }

    private void WireLoad()
    {
        for(int i = 0; i < wiresInPanel.Count; i++)
        {
            wiresInPanel[i].WireComplete();
            wiresInPanel[i].wire.gameObject.GetComponent<Image>().color = completeColors[i];
            wiresInPanel[i].wireStartPos.gameObject.GetComponent<Image>().color = completeColors[i];
            wiresInPanel[i].wireEndPos.gameObject.GetComponent<Image>().color= completeColors[i];
        }
    }

    private bool CheckForWiresCut()
    {
        foreach(Wire wire in wiresInPanel)
        {
            if(wire.wireCompleted == true)
            {
                return false;
            }
        }
        return true;
    }

    private bool CheckForWiresCompleted()
    {
        foreach(Wire wire in wiresInPanel)
        {
            if(wire.wireCompleted == false)
            {
                return false;
            }
        }
        return true;
    }

    private void RearrangeEndPoints()
    {
        RectTransform[] endpoints = new RectTransform[wiresInPanel.Count];
        for(int i = 0; i < endpoints.Length; i++)
        {
            endpoints[i] = wiresInPanel[i].wireEndPos;
        }

        for(int i = 0; i < 20; i++)
        {
            int rng1 = Random.Range(0, endpoints.Length);
            RectTransform endPoint1 = endpoints[rng1];
            int rng2 = Random.Range(0, endpoints.Length);
            RectTransform endPoint2 = endpoints[rng2];

            int determineSwap = Random.Range(1, 2);
            if (determineSwap == 1)
            {
                Vector2 temp = endPoint1.anchoredPosition;
                endPoint1.anchoredPosition = endPoint2.anchoredPosition;
                endPoint2.anchoredPosition = temp;
            }
        }

        for(int i = 0; i < endpoints.Length; i++)
        {
            
            endpoints[i].gameObject.GetComponent<Image>().color = incompleteColors[i];
        }

        
    }


}
