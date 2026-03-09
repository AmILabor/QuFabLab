using System.Collections;
using System.Collections.Generic;
using MRTKExtensions.QRCodes;
using UnityEngine;

public class ARCasePlacer : MonoBehaviour
{
    // Start is called before the first frame update
    private QRInfo bottomLeftInfo;
    private QRInfo topRightInfo;
    [SerializeField] public GameObject scanningClue;
    [SerializeField] public GameObject bottomLeftPosition;
    [SerializeField] public GameObject topRightPosition;
    public void SetBottomLeftInfo(QRInfo info)
    {
        AMI.Util.Console.Log("ScannedQR-BotLeft",info.PhysicalSideLength);
        bottomLeftInfo = info;
        handleScanningDone();
    }
    public void SetTopRightInfo(QRInfo info)
    {
        topRightInfo = info;
        AMI.Util.Console.Log("ScannedQR-TopRight",info.PhysicalSideLength);
        handleScanningDone();
    }

    public void ReactivateScanning()
    {
        bottomLeftInfo = null;
        topRightInfo = null;
        scanningClue.SetActive(true);
        
    }

    private void handleScanningDone()
    {
        if (isScanningDone())
        {
            scanningClue.SetActive(false);
        }
    }

    private bool isScanningDone()
    {
        return topRightInfo != null && bottomLeftInfo != null;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
