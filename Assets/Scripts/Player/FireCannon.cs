﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireCannon : MonoBehaviour
{
    public ParticleSystem emitter;
    public Text cannonReadout;
    public Text crosshair;
    private float lastAimedRange = 50.0f;
    private float maxAimRange = 500.0f; // avoids dot vanishing

    private void AddArrayIntoListIfUnique(List<RaycastHit> toList, RaycastHit[] fromArray)
    {
        for (int i = 0; i < fromArray.Length; i++)
        {
            if (toList.Contains(fromArray[i]) == false)
            {
                toList.Add(fromArray[i]);
            }
        }

    }

    private void Start()
    {
        crosshair.color = Color.cyan;
    }

    void Update()
    {
        float thicknessRange = 0.4f;
        RaycastHit[] laserMiddle = Physics.RaycastAll(transform.position, transform.forward);
        RaycastHit[] laserLeft = Physics.RaycastAll(transform.position-transform.right* thicknessRange, transform.forward);
        RaycastHit[] laserRight = Physics.RaycastAll(transform.position + transform.right * thicknessRange, transform.forward);
        RaycastHit[] laserUp = Physics.RaycastAll(transform.position+transform.up * thicknessRange, transform.forward);
        RaycastHit[] laserDown = Physics.RaycastAll(transform.position-transform.up * thicknessRange, transform.forward);
        float roundedPerc = 0.707f;
        RaycastHit[] laserUL = Physics.RaycastAll(transform.position + transform.up * thicknessRange * roundedPerc 
                                                                     - transform.right * thicknessRange * roundedPerc, transform.forward);
        RaycastHit[] laserUR = Physics.RaycastAll(transform.position + transform.up * thicknessRange * roundedPerc 
                                                                     + transform.right * thicknessRange * roundedPerc, transform.forward);
        RaycastHit[] laserDL = Physics.RaycastAll(transform.position - transform.up * thicknessRange * roundedPerc
                                                                     - transform.right * thicknessRange * roundedPerc, transform.forward);
        RaycastHit[] laserDR = Physics.RaycastAll(transform.position - transform.up * thicknessRange * roundedPerc
                                                                     + transform.right * thicknessRange * roundedPerc, transform.forward);

        List<RaycastHit> rhListTemp = new List<RaycastHit>();
        AddArrayIntoListIfUnique(rhListTemp, laserLeft);
        AddArrayIntoListIfUnique(rhListTemp, laserRight);
        AddArrayIntoListIfUnique(rhListTemp, laserUp);
        AddArrayIntoListIfUnique(rhListTemp, laserDown);
        AddArrayIntoListIfUnique(rhListTemp, laserUL);
        AddArrayIntoListIfUnique(rhListTemp, laserUR);
        AddArrayIntoListIfUnique(rhListTemp, laserDL);
        AddArrayIntoListIfUnique(rhListTemp, laserDR);
        AddArrayIntoListIfUnique(rhListTemp, laserMiddle);

        RaycastHit[] allRH = rhListTemp.ToArray(); // Physics.RaycastAll(transform.position, transform.forward);

        int hitsInRange = 0;
        for(int i=0; i< allRH.Length;i++)
        {
            RaycastHit rhInfo = allRH[i];

            // crosshair.rectTransform.position = Camera.main.WorldToScreenPoint(rhInfo.point);
            lastAimedRange = Vector3.Distance(transform.position, rhInfo.point);
            crosshair.color = Color.red;

            ExplodeChainReact ecrScript = rhInfo.collider.gameObject.GetComponent<ExplodeChainReact>();
            if (Input.GetMouseButtonDown(0))
            {
                emitter.Emit(1000);
                Debug.Log("DIRECT HIT:" + rhInfo.collider.gameObject.name);
                if(ecrScript)
                {
                    ecrScript.Explode(1);
                }
            }
            else
            {
                if (ecrScript)
                {
                    hitsInRange += ecrScript.HitsInRange();
                }
                else
                {
                    cannonReadout.text = "X";
                }
            }
        }

        if(lastAimedRange> maxAimRange)
        {
            lastAimedRange = maxAimRange;
        }
        crosshair.rectTransform.position = Camera.main.WorldToScreenPoint(transform.position + transform.forward * lastAimedRange);

        if (allRH.Length==0) // essentially, else / nothing under gun
        {
            crosshair.color = Color.cyan;

            cannonReadout.text = "0";
        } else if(hitsInRange != 0)
        {
            cannonReadout.text = "" + hitsInRange;
        }
        else
        {
            cannonReadout.text = "X";
        }
    }
}
