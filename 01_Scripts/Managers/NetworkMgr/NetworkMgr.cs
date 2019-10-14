using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.IO.Ports;
using Microsoft.Win32;


public enum eDEVICETYPE
{
    NONE,
    LEFT_SHOCK,           //왼손 전류
    RIGHT_SHOCK,             //오른손 전류
    PAIR_SHOCK,              //양손 전류
    PAIR_VIBE,               //양손 진동
    LEFT_VIBE,              //왼손 진동
    RIGHT_VIBE,            //오른손 진동
    SOLENOID,            //솔레노이드
    PRESSOR,              //협착
    FALL,                 //떨어짐 
    FALL_FIX,             //떨어짐 고정
    FAN,                  //팬
    HAND_PRESSURE,         //손 압착
    PUSH,                   //미는거 (EX)기본준수
    KNOCK_FLOOR,            //바닥에서 두드리는거 (EX)중량물
   
}

public enum eHANDCONTENT
{
    SHOCK = 0,
    VIBE
}

public class DeviceInfo
{
    public string device;                               //장치
    public float batteryVoltage;                         //배터리전압
    public float udpSARCount;                            //송수신횟수
    public int udpSARSensitivity;                       //송수신감도
    public string udpIp;
    public bool isDeviceConnection = false;             //디바이스연결상태
    public string deviceConnectionText = "연결안됨";     //디바이스GUI연결상태


    public DeviceInfo(string device, string ip)
    {
        this.device = device;
        this.udpIp = ip;
    }
}

public class NetworkMgr : Singleton<NetworkMgr>
{
    #region 멤버 변수
    //private
    protected List<DeviceInfo> deviceInfoList;            //장치정보
    private List<UdpClient> udpClientList;
    private List<string> sendIpList;
    private byte[] mByteSNDBUFF;
    private string strFilePath;
    private string readstr;

    private string udpLeftIp = "192.168.1.200";                 //왼손
    private string udpRightIp = "192.168.1.201";                //오른손
    private string udpSolenoidIp = "192.168.1.109";             //솔레노이드
    private string udpPressorIp= "192.168.1.55";                 //프레스 협착
    private string udpFallIp = "192.168.1.52";                  //떨어짐
    private string udpFanIp = "192.168.1.1";                    //팬
    private string udpHandPressureIP= "192.168.2.1";            //손 압착
    private string udpPushIp = "192.168.10.50";                //밀기 (ex)기본준수
    private string udpKnockFloorIP = "192.168.1.53";              //바닥에서 두드리기 (ex)중량물
    private string port = "8888";

    private Dictionary<string, string> textReadDic;
    private byte[] dgram = new byte[12];
    private UdpClient srv;
    private IPEndPoint remoteEP;
    private bool isGUIShow;
    private bool isLostUdp;
    private float waitingTime = 5f;
    public float timer = 0f;
    private float receiveTimer = 0f;
    private float receiveWaitingTime = 0.5f;
    private bool isLeftNoBattery = false;
    private bool isRightNoBattery = false;
    private bool isSolenoidNoBattery = false;
    private bool isFallNoBattery = false;
    private int batteryCharge = 40;             //배터리 충전요청 기준 수치
    private string remoteIP;


    private string[] glovesLabel = new string[8]
        {
            "왼손 : ",
            "오른손 : ",
            "배터리 : ",
            "배터리 : ",
            "송수신횟수 : ",
            "송수신횟수 : ",
            "송수신감도 : ",
            "송수신감도 : "
        };

    //public
    public bool isElectricShock;
    public bool isReceiveCheck;
    public int eShockPulseCount;
    public int eSolenoidPulseCount;
    public int deviceSendCount;
    #endregion

    #region 유니티 호출 함수
    public void Init()
    {
        srv = new UdpClient(8888);
        remoteEP = new IPEndPoint(IPAddress.Any, 8888);
        textReadDic = new Dictionary<string, string>();
        sendIpList = new List<string>();
        isGUIShow = false;
        isElectricShock = true;
        isReceiveCheck = false;
        eShockPulseCount = 50;
        eSolenoidPulseCount = 2;


        strFilePath = @"C:/save";
        DirectoryInfo di = new DirectoryInfo(strFilePath);
        if (di.Exists == false)
        {
            di.Create();

            using (StreamWriter wr = new StreamWriter(strFilePath + "/MRData.txt"))
            {
                wr.WriteLine("udpLeftIp:" + udpLeftIp);
                wr.WriteLine("udpRightIp:" + udpRightIp);
                wr.WriteLine("udpSolenoidIp:" + udpSolenoidIp);
                wr.WriteLine("udpPressorIp:" + udpPressorIp);
                wr.WriteLine("udpFallIp:" + udpFallIp);
                wr.WriteLine("udpFanIp:" + udpFanIp);
                wr.WriteLine("udpHandPressureIP:" + udpHandPressureIP);
                wr.WriteLine("udpPushIp:" + udpPushIp);
                wr.WriteLine("udpKnockFloorIP:" + udpKnockFloorIP);
                wr.WriteLine("port:" + port);
                wr.WriteLine("eShockPulseCount:" + eShockPulseCount);
                
            }
        }
        
        strFilePath += @"/MRData.txt";
        FileStream fs = new FileStream(strFilePath, FileMode.Open);
        StreamReader sr = new StreamReader(fs);
  
        while ((readstr = sr.ReadLine()) != null)
        {
            char sp = ':';
            string[] spstring = readstr.Split(sp);
            Debug.Log(spstring[0].Substring(0));                //192.168.0.245
            if (!textReadDic.ContainsKey(spstring[0]))
            {
                textReadDic.Add(spstring[0], spstring[1]);
            }
        }

        fs.Close();
        sr.Close();

        mByteSNDBUFF = new byte[13];
        udpClientList = new List<UdpClient>();

        port = GetDicValue("port");
        udpLeftIp = GetDicValue("udpLeftIp");
        udpRightIp = GetDicValue("udpRightIp");
        udpSolenoidIp = GetDicValue("udpSolenoidIp");
        udpPressorIp = GetDicValue("udpPressorIp");
        udpFallIp = GetDicValue("udpFallIp");
        udpFanIp = GetDicValue("udpFanIp");
        udpHandPressureIP = GetDicValue("udpHandPressureIP");
        udpPushIp = GetDicValue("udpPushIp");
        udpKnockFloorIP = GetDicValue("udpKnockFloorIP");

        
        eShockPulseCount = Int32.Parse(GetDicValue("eShockPulseCount"));
      

        //UdpClient Create
        udpClientList.Add(new UdpClient(udpLeftIp, Int32.Parse(port)));     //왼손
        udpClientList.Add(new UdpClient(udpRightIp, Int32.Parse(port)));    //오른손
        udpClientList.Add(new UdpClient(udpSolenoidIp, Int32.Parse(port)));    //솔레노이드
        udpClientList.Add(new UdpClient(udpPressorIp, Int32.Parse(port)));    //솔레노이드
        udpClientList.Add(new UdpClient(udpFallIp, Int32.Parse(port)));
        udpClientList.Add(new UdpClient(udpFanIp, Int32.Parse(port)));
        udpClientList.Add(new UdpClient(udpHandPressureIP, Int32.Parse(port)));    //솔레노이드
        udpClientList.Add(new UdpClient(udpPushIp, Int32.Parse(port)));    //솔레노이드
        udpClientList.Add(new UdpClient(udpKnockFloorIP, Int32.Parse(port)));    //솔레노이드

        //디바이스 정보값 저장
        deviceInfoList = new List<DeviceInfo>();
        deviceInfoList.Add(new DeviceInfo("Left", udpLeftIp));
        deviceInfoList.Add(new DeviceInfo("Right", udpRightIp));
        deviceInfoList.Add(new DeviceInfo("Solenoid", udpSolenoidIp));
        deviceInfoList.Add(new DeviceInfo("Pressor", udpPressorIp));
        deviceInfoList.Add(new DeviceInfo("Fall", udpFallIp));
        deviceInfoList.Add(new DeviceInfo("Fan", udpFanIp));
        deviceInfoList.Add(new DeviceInfo("HandPressure", udpHandPressureIP));
        deviceInfoList.Add(new DeviceInfo("Push", udpPushIp));
        deviceInfoList.Add(new DeviceInfo("KnockFloor", udpKnockFloorIP));

        StartCoroutine(StartUdpReceive());
    }

    private void OnDisable()
    {
        srv.Close();
        for (int i = 0; i < udpClientList.Count; ++i)
        {
            udpClientList[i].Close();
        }     
    }

    void Update()
    { 
        if (isReceiveCheck)
        {
            receiveTimer += Time.deltaTime;

            if (receiveTimer > receiveWaitingTime)
            {
                receiveTimer = 0f;
                isReceiveCheck = false;
            }
        }
    }



    private void OnGUI()
    {
        if (isGUIShow)
        {
            int labelContentCount = 1;
            int labelContentHeight = 30;
            int labelContentYGab = 3;

            GUI.Box(new Rect(10, 10, 500, 300), "디바이스정보");

            //디바이스정보
            GUIDeviceInfo();
            labelContentCount = 1;
            GUI.Label(new Rect(90, labelContentCount++ * (labelContentHeight + labelContentYGab), 145, labelContentHeight), deviceInfoList[0].deviceConnectionText);
            GUI.Label(new Rect(100, labelContentCount++ * (labelContentHeight + labelContentYGab), 145, labelContentHeight), deviceInfoList[0].batteryVoltage.ToString() + "%");
            GUI.Label(new Rect(100, labelContentCount++ * (labelContentHeight + labelContentYGab), 145, labelContentHeight), deviceInfoList[0].udpSARCount.ToString());
            GUI.Label(new Rect(100, labelContentCount++ * (labelContentHeight + labelContentYGab), 145, labelContentHeight), deviceInfoList[0].udpSARSensitivity.ToString() + "%");

            labelContentCount = 1;
            GUI.Label(new Rect(255, labelContentCount++ * (labelContentHeight + labelContentYGab), 145, labelContentHeight), deviceInfoList[1].deviceConnectionText);
            GUI.Label(new Rect(265, labelContentCount++ * (labelContentHeight + labelContentYGab), 145, labelContentHeight), deviceInfoList[1].batteryVoltage.ToString() + "%");
            GUI.Label(new Rect(265, labelContentCount++ * (labelContentHeight + labelContentYGab), 145, labelContentHeight), deviceInfoList[1].udpSARCount.ToString());
            GUI.Label(new Rect(265, labelContentCount++ * (labelContentHeight + labelContentYGab), 145, labelContentHeight), deviceInfoList[1].udpSARSensitivity.ToString() + "%");

            GUI.Label(new Rect(345, 33, 145, labelContentHeight), "솔레노이드 : ");
            GUI.Label(new Rect(345, 66, 145, labelContentHeight), "배터리 : ");
            GUI.Label(new Rect(345, 99, 145, labelContentHeight), "송수신횟수 : ");
            GUI.Label(new Rect(345, 132, 145, labelContentHeight), "송수신감도 : ");
            GUI.Label(new Rect(425, 33, 145, labelContentHeight), deviceInfoList[2].deviceConnectionText);
            GUI.Label(new Rect(425, 66, 145, labelContentHeight), deviceInfoList[2].batteryVoltage.ToString() + "%");
            GUI.Label(new Rect(425, 99, 145, labelContentHeight), deviceInfoList[2].udpSARCount.ToString());
            GUI.Label(new Rect(425, 132, 145, labelContentHeight), deviceInfoList[2].udpSARSensitivity.ToString() + "%");

            //전류정보
            GUI.Label(new Rect(20, 170, 145, 30), "장갑전류정보");
            GUI.Label(new Rect(15, 200, 145, 30), "E-Shock : ");
            GUI.Label(new Rect(80, 200, 145, 30), eShockPulseCount.ToString());
            eShockPulseCount = (int)GUI.HorizontalSlider(new Rect(110, 205, 170, 30), eShockPulseCount, 1f, 250f);

            if (GUI.Button(new Rect(30, 270, 100, 30), "OK"))
            {
                Debug.Log("값 교체");
                SetShockPulseCount(eShockPulseCount);
            }

            if (GUI.Button(new Rect(180, 270, 100, 30), "Default"))
            {
                Debug.Log("디폴트 값으로 교체");
                SetShockPulseCount(50);             //디폴트 50
            }
        }

        if (isLeftNoBattery)
        {
            GUI.Label(new Rect(Screen.width - 180, 20, 200, 30), "왼손장갑 배터리 충전 요청");
        }

        if (isRightNoBattery)
        {
            GUI.Label(new Rect(Screen.width - 180, 50, 200, 30), "오른손장갑 배터리 충전 요청");
        }

        if (isSolenoidNoBattery)
        {
            GUI.Label(new Rect(Screen.width - 180, 80, 200, 30), "머리충격 배터리 충전 요청");
        }
    }

    #endregion

    #region 장치 Send

    public void GlovePairShock()
    {
        DeviceDataSend(eDEVICETYPE.PAIR_SHOCK, 255, eHANDCONTENT.SHOCK);
    }

    public void GlovePairVive()
    {
        DeviceDataSend(eDEVICETYPE.PAIR_VIBE, 255, eHANDCONTENT.VIBE);
    }

    public void GloveRightVibe()
    {
        DeviceDataSend(eDEVICETYPE.RIGHT_VIBE, 255, eHANDCONTENT.VIBE);
    }

    public void GloveLeftVibe()
    {
        DeviceDataSend(eDEVICETYPE.LEFT_VIBE, 255, eHANDCONTENT.VIBE);
    }

    public void GloveRightShock()
    {
        DeviceDataSend(eDEVICETYPE.RIGHT_SHOCK, 255, eHANDCONTENT.SHOCK);
    }

    public void GloveLeftShock()
    {

    }

  

    public void DeviceDataSend(eDEVICETYPE deviceType, int pVibe, eHANDCONTENT handContent = eHANDCONTENT.SHOCK, int iCount = 1, float intervalTime = 0, int pPulseLow = 70, int pPulseHigh = 128, int pChargePuleCount = 50, int pDischargePulse = 128)
    {

        StartCoroutine(CoDataSend(deviceType, pVibe, pPulseLow, pPulseHigh, pChargePuleCount, pDischargePulse, handContent, iCount, intervalTime));

        isReceiveCheck = true;
    }

    //데이터 send 업데이트
    IEnumerator CoDataSend(eDEVICETYPE deviceType, int pVibe, int pPulseLow, int pPulseHigh, int pChargePuleCount, int pDischargePulse, eHANDCONTENT handContent, int iCount = 1, float intervalTime = 0)
    {
        int index = 0;
        int ihandContent = (int)handContent;
        while (index < iCount)
        {
            switch (deviceType)
            {
                case eDEVICETYPE.NONE:
                    ElectricConnect(0, 0, 0, 0, 0, 0);
                    break;

                case eDEVICETYPE.LEFT_SHOCK:
                    {
                        if (ihandContent == 0)
                        {
                            OneObjDataSend(pVibe, pPulseLow, pPulseHigh, pChargePuleCount, pDischargePulse, 1, udpClientList[0]);
                        }
                        else
                        {
                            OneObjDataSend(pVibe, 0, 0, 0, 0, 0, udpClientList[0]);
                        }
                    }
                    break;


                case eDEVICETYPE.LEFT_VIBE:
                    if (ihandContent == 0)
                    {
                        OneHandVIbeDataSend(pVibe, pPulseLow, pPulseHigh, pChargePuleCount, pDischargePulse, 1, udpClientList[0]);
                    }
                    else
                    {
                        OneHandVIbeDataSend(pVibe, 0, 0, 0, 0, 0, udpClientList[0]);
                    }
                    break;



                case eDEVICETYPE.RIGHT_SHOCK:
                    {
                        if (ihandContent == 0)
                        {
                            OneObjDataSend(pVibe, pPulseLow, pPulseHigh, pChargePuleCount, pDischargePulse, 1, udpClientList[1]);
                        }
                        else
                        {
                            OneObjDataSend(pVibe, 0, 0, 0, 0, 0, udpClientList[1]);
                        }
                    }
                    break;


                case eDEVICETYPE.RIGHT_VIBE:
                    if (ihandContent == 0)
                    {
                        OneHandVIbeDataSend(pVibe, pPulseLow, pPulseHigh, pChargePuleCount, pDischargePulse, 1, udpClientList[1]);
                    }
                    else
                    {
                        OneHandVIbeDataSend(pVibe, 0, 0, 0, 0, 0, udpClientList[1]);
                    }
                    break;

                case eDEVICETYPE.PAIR_SHOCK:
                    {
                        if (ihandContent == 0)
                        {
                            PairHandDataSend(pVibe, pPulseLow, pPulseHigh, pChargePuleCount, pDischargePulse, 1);
                        }
                        else
                        {
                            PairHandDataSend(pVibe, 0, 0, 0, 0, 0);
                        }
                    }
                    break;

              
                case eDEVICETYPE.PAIR_VIBE:
                    if (ihandContent == 0)
                    {
                        PairHandVibeDataSend(pVibe, pPulseLow, pPulseHigh, pChargePuleCount, pDischargePulse, 1);
                    }
                    else
                    {
                        PairHandVibeDataSend(pVibe, 0, 0, 0, 0, 0);
                    }
                    break;
                case eDEVICETYPE.SOLENOID:
                    OneObjDataSend(pVibe, pPulseLow, pPulseHigh, pChargePuleCount, pDischargePulse, eSolenoidPulseCount, udpClientList[2]);
                    break;

                case eDEVICETYPE.PRESSOR:
                    break;

                case eDEVICETYPE.FALL:
                    break;

                case eDEVICETYPE.FALL_FIX:
                    break;

                case eDEVICETYPE.FAN:
                    break;

                case eDEVICETYPE.HAND_PRESSURE:
                    break;

                case eDEVICETYPE.PUSH:
                    break;

                case eDEVICETYPE.KNOCK_FLOOR:
                    break;
            }
            ++index;
            yield return new WaitForSeconds(intervalTime);
        }
        yield return null;
    }

    
    //양쪽손
    public void PairHandDataSend(int pVibe, int pPulseLow, int pPulseHigh, int pChargePuleCount, int pDischargePulse, int pEshockPulseCount)
    {
        PairHandData(Convert.ToByte(pVibe), Convert.ToByte(pPulseLow), Convert.ToByte(pPulseHigh), Convert.ToByte(pChargePuleCount),
                         Convert.ToByte(pDischargePulse), Convert.ToByte(pEshockPulseCount));
    }

    //하나만 보냄. (ex)한쪽손, 솔레노이드
    public void OneObjDataSend(int pVibe, int pPulseLow, int pPulseHigh, int pChargePuleCount, int pDischargePulse, int pEshockPulseCount, UdpClient udpClient)
    {
        OneData(Convert.ToByte(pVibe), Convert.ToByte(pPulseLow), Convert.ToByte(pPulseHigh), Convert.ToByte(pChargePuleCount),
                         Convert.ToByte(pDischargePulse), Convert.ToByte(pEshockPulseCount), udpClient);
    }

    //양쪽손 진동
    public void PairHandVibeDataSend(int pVibe, int pPulseLow, int pPulseHigh, int pChargePuleCount, int pDischargePulse, int pEshockPulseCount)
    {
        PairHandVibeData(Convert.ToByte(pVibe), Convert.ToByte(pPulseLow), Convert.ToByte(pPulseHigh), Convert.ToByte(pChargePuleCount),
                         Convert.ToByte(pDischargePulse), Convert.ToByte(pEshockPulseCount));
    }

    //한쪽손 진동
    public void OneHandVIbeDataSend(int pVibe, int pPulseLow, int pPulseHigh, int pChargePuleCount, int pDischargePulse, int pEshockPulseCount, UdpClient udpClient)
    {
        OneHandVibeData(Convert.ToByte(pVibe), Convert.ToByte(pPulseLow), Convert.ToByte(pPulseHigh), Convert.ToByte(pChargePuleCount),
                         Convert.ToByte(pDischargePulse), Convert.ToByte(pEshockPulseCount), udpClient);
    }


    //양쪽손을 감전 위한 함수
    public void PairHandData(byte pVibe, byte pPulseLow, byte pPulseHigh, byte pChargePuleCount, byte pDischargePulse, byte pEshockPulseCount)
    {
        mByteSNDBUFF[0] = 0x02;
        mByteSNDBUFF[1] = 00;
        mByteSNDBUFF[2] = 00;
        mByteSNDBUFF[3] = 00;
        mByteSNDBUFF[4] = 00;
        mByteSNDBUFF[5] = 00;
        mByteSNDBUFF[6] = 30;
        mByteSNDBUFF[7] = Convert.ToByte(0xFF);
        mByteSNDBUFF[8] = 07;
        mByteSNDBUFF[9] = 80;
        mByteSNDBUFF[10] = 65;
        mByteSNDBUFF[11] = (byte)((byte)mByteSNDBUFF[1] + (byte)mByteSNDBUFF[2] + (byte)mByteSNDBUFF[3] +
                               (byte)mByteSNDBUFF[4] + (byte)mByteSNDBUFF[5] + (byte)mByteSNDBUFF[6] + (byte)mByteSNDBUFF[7] +
                               (byte)mByteSNDBUFF[8] + (byte)mByteSNDBUFF[9] + (byte)mByteSNDBUFF[10]);
        mByteSNDBUFF[12] = 0x03;

        Debug.Log("pEshockPulseCount : " + pEshockPulseCount);

        for (int i = 0; i < 2; ++i)
        {
            udpClientList[i].Send(mByteSNDBUFF, mByteSNDBUFF.Length);
        }
    }

    //양쪽손 진동
    public void PairHandVibeData(byte pVibe, byte pPulseLow, byte pPulseHigh, byte pChargePuleCount, byte pDischargePulse, byte pEshockPulseCount)
    {
        mByteSNDBUFF[0] = 0x02;
        mByteSNDBUFF[1] = pVibe;
        mByteSNDBUFF[2] = pVibe;
        mByteSNDBUFF[3] = pVibe;
        mByteSNDBUFF[4] = pVibe;
        mByteSNDBUFF[5] = pVibe;
        mByteSNDBUFF[6] = pPulseLow;
        mByteSNDBUFF[7] = pPulseHigh;
        mByteSNDBUFF[8] = pChargePuleCount;
        mByteSNDBUFF[9] = pDischargePulse;
        mByteSNDBUFF[10] = pEshockPulseCount;
        mByteSNDBUFF[11] = (byte)((byte)mByteSNDBUFF[1] + (byte)mByteSNDBUFF[2] + (byte)mByteSNDBUFF[3] +
                               (byte)mByteSNDBUFF[4] + (byte)mByteSNDBUFF[5] + (byte)mByteSNDBUFF[6] + (byte)mByteSNDBUFF[7] +
                               (byte)mByteSNDBUFF[8] + (byte)mByteSNDBUFF[9] + (byte)mByteSNDBUFF[10]);
        mByteSNDBUFF[12] = 0x03;

        Debug.Log("pEshockPulseCount : " + pEshockPulseCount);

        for (int i = 0; i < 2; ++i)
        {
            udpClientList[i].Send(mByteSNDBUFF, mByteSNDBUFF.Length);
        }
    }


    //감전 한쪽손만 할때
    public void OneData(byte pVibe, byte pPulseLow, byte pPulseHigh, byte pChargePuleCount, byte pDischargePulse, byte pEshockPulseCount, UdpClient udpClient)
    {
        //isElectricShock = false;
        mByteSNDBUFF[0] = 0x02;
        mByteSNDBUFF[1] = pVibe;
        mByteSNDBUFF[2] = pVibe;
        mByteSNDBUFF[3] = pVibe;
        mByteSNDBUFF[4] = pVibe;
        mByteSNDBUFF[5] = pVibe;
        mByteSNDBUFF[6] = pPulseLow;
        mByteSNDBUFF[7] = pPulseHigh;
        mByteSNDBUFF[8] = pChargePuleCount;
        mByteSNDBUFF[9] = pDischargePulse;
        mByteSNDBUFF[10] = pEshockPulseCount;
        mByteSNDBUFF[11] = (byte)((byte)mByteSNDBUFF[1] + (byte)mByteSNDBUFF[2] + (byte)mByteSNDBUFF[3] +
                               (byte)mByteSNDBUFF[4] + (byte)mByteSNDBUFF[5] + (byte)mByteSNDBUFF[6] + (byte)mByteSNDBUFF[7] +
                               (byte)mByteSNDBUFF[8] + (byte)mByteSNDBUFF[9] + (byte)mByteSNDBUFF[10]);
        mByteSNDBUFF[12] = 0x03;


        udpClient.Send(mByteSNDBUFF, mByteSNDBUFF.Length);
    }

    //진동 한쪽손
    public void OneHandVibeData(byte pVibe, byte pPulseLow, byte pPulseHigh, byte pChargePuleCount, byte pDischargePulse, byte pEshockPulseCount, UdpClient udpClient)
    {
        mByteSNDBUFF[0] = 0x02;
        mByteSNDBUFF[1] = pVibe;
        mByteSNDBUFF[2] = pVibe;
        mByteSNDBUFF[3] = pVibe;
        mByteSNDBUFF[4] = pVibe;
        mByteSNDBUFF[5] = pVibe;
        mByteSNDBUFF[6] = pPulseLow;
        mByteSNDBUFF[7] = pPulseHigh;
        mByteSNDBUFF[8] = pChargePuleCount;
        mByteSNDBUFF[9] = pDischargePulse;
        mByteSNDBUFF[10] = pEshockPulseCount;
        mByteSNDBUFF[11] = (byte)((byte)mByteSNDBUFF[1] + (byte)mByteSNDBUFF[2] + (byte)mByteSNDBUFF[3] +
                               (byte)mByteSNDBUFF[4] + (byte)mByteSNDBUFF[5] + (byte)mByteSNDBUFF[6] + (byte)mByteSNDBUFF[7] +
                               (byte)mByteSNDBUFF[8] + (byte)mByteSNDBUFF[9] + (byte)mByteSNDBUFF[10]);
        mByteSNDBUFF[12] = 0x03;

        udpClient.Send(mByteSNDBUFF, mByteSNDBUFF.Length);
    }


    //팬
    public void OnFanDataSend(byte pVibe, byte Step, byte pPulseHigh, byte pChargePuleCount, byte pDischargePulse, byte pEshockPulseCount, UdpClient udpClient)
    {
        byte Step1 = 0x00;
        byte Step2 = 0x00;
        byte Step3 = 0x00;

        switch (Step)
        {
            case 1: Step1 = 0xFF; break;
            case 2: Step2 = 0xFF; break;
            case 3: Step3 = 0xFF; break;
            default: Step1 = 0xFF; break;
        }

        //isElectricShock = false;
        mByteSNDBUFF[0] = 0x02;
        mByteSNDBUFF[1] = Step1;
        mByteSNDBUFF[2] = Step2;
        mByteSNDBUFF[3] = Step3;
        mByteSNDBUFF[4] = 00;
        mByteSNDBUFF[5] = 00;
        mByteSNDBUFF[6] = 00;
        mByteSNDBUFF[7] = 00;
        mByteSNDBUFF[8] = 00;
        mByteSNDBUFF[9] = 00;
        mByteSNDBUFF[10] = 00;
        mByteSNDBUFF[11] = 0xFF;
        mByteSNDBUFF[12] = 0x03;

    
        udpClient.Send(mByteSNDBUFF, mByteSNDBUFF.Length);


        //isElectricShock = true;

        // 02 00 FF 00 00 00 00 00 00 00 00 FF 03  //떨어짐
        //02 FF 00 00 00 00 00 00 00 00 00 FF 03   //픽스
    }


    //팬꺼버려
    public void OffFanDataSend(byte pVibe, byte pPulseLow, byte pPulseHigh, byte pChargePuleCount, byte pDischargePulse, byte pEshockPulseCount, UdpClient udpClient)
    {
        //isElectricShock = false;
        mByteSNDBUFF[0] = 0x02;
        mByteSNDBUFF[1] = 00;
        mByteSNDBUFF[2] = 00;
        mByteSNDBUFF[3] = 00;
        mByteSNDBUFF[4] = 0xFF;
        mByteSNDBUFF[5] = 00;
        mByteSNDBUFF[6] = 00;
        mByteSNDBUFF[7] = 00;
        mByteSNDBUFF[8] = 00;
        mByteSNDBUFF[9] = 00;
        mByteSNDBUFF[10] = 00;
        mByteSNDBUFF[11] = 0xFF;
        mByteSNDBUFF[12] = 0x03;

        Debug.Log("OffFan");
        udpClient.Send(mByteSNDBUFF, mByteSNDBUFF.Length);
        //isElectricShock = true;

        // 02 00 FF 00 00 00 00 00 00 00 00 FF 03  //떨어짐
        //02 FF 00 00 00 00 00 00 00 00 00 FF 03   //픽스
    }

    //5~10초마다 장비가 잘 작동하는지 보기위해서 만든함수
    public void ElectricConnect(int pVibe, int pPulseLow, int pPulseHigh, int pChargePuleCount, int pDischargePulse, int pEshockPulseCount)
    {
        //isElectricShock = false;
        mByteSNDBUFF[0] = 0x02;
        mByteSNDBUFF[1] = Convert.ToByte(pVibe);
        mByteSNDBUFF[2] = Convert.ToByte(pVibe);
        mByteSNDBUFF[3] = Convert.ToByte(pVibe);
        mByteSNDBUFF[4] = Convert.ToByte(pVibe);
        mByteSNDBUFF[5] = Convert.ToByte(pVibe);
        mByteSNDBUFF[6] = Convert.ToByte(pPulseLow);
        mByteSNDBUFF[7] = Convert.ToByte(pPulseHigh);
        mByteSNDBUFF[8] = Convert.ToByte(pChargePuleCount);
        mByteSNDBUFF[9] = Convert.ToByte(pDischargePulse);
        mByteSNDBUFF[10] = Convert.ToByte(pEshockPulseCount);
        mByteSNDBUFF[11] = (byte)((byte)mByteSNDBUFF[1] + (byte)mByteSNDBUFF[2] + (byte)mByteSNDBUFF[3] +
                               (byte)mByteSNDBUFF[4] + (byte)mByteSNDBUFF[5] + (byte)mByteSNDBUFF[6] + (byte)mByteSNDBUFF[7] +
                               (byte)mByteSNDBUFF[8] + (byte)mByteSNDBUFF[9] + (byte)mByteSNDBUFF[10]);
        mByteSNDBUFF[12] = 0x03;

        for (int i = 0; i < udpClientList.Count; ++i)
        {
            udpClientList[i].Send(mByteSNDBUFF, mByteSNDBUFF.Length);
        }
        //isElectricShock = true;
    }
    #endregion

    #region 통신받기
    IEnumerator StartUdpReceive()
    {
        while (true)
        {
            yield return StartCoroutine(UdpReceive());
        }
    }

    IEnumerator UdpReceive()
    {
        yield return new WaitForSeconds(0.3f);
        UdpReceiveFun();
    }

    public void UdpReceiveFun()
    {
        if (srv.Available > 0)
        {
            isLostUdp = false;
            //Debug.Log("srv.Available : " + srv.Available);
            //IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 8888);
            dgram = srv.Receive(ref remoteEP);
            if (dgram.Length > 0)
            {
                //Debug.Log(String.Format("[Recive] {0}로부터 {1} 바이트 수신", remoteEP.ToString(), dgram.Length));

                string strData = "";
                for (int i = 0; i < dgram.Length; ++i)
                {
                    strData += dgram[i].ToString() + " ";
                }
               
                remoteIP = GetRemoteEPIP(remoteEP.ToString());

                sendIpList.Add(remoteIP);
                sendIpList = RemoveDuplicateValue(sendIpList);

                for (int i = 0; i < deviceInfoList.Count; ++i)
                {
                    if (remoteIP == udpLeftIp)               //왼손
                    {
                        if (deviceInfoList[i].device.Equals("Left"))
                        {
                            SetDeviceInfo(deviceInfoList[i], ref isLeftNoBattery);
                        }
                    }
                    else if (remoteIP == udpRightIp)         //오른손
                    {
                        if (deviceInfoList[i].device.Equals("Right"))
                        {
                            SetDeviceInfo(deviceInfoList[i], ref isRightNoBattery);
                        }
                    }
                    else if (remoteIP == udpSolenoidIp)      //솔레노이드
                    {
                        if (deviceInfoList[i].device.Equals("Solenoid"))
                        {
                            SetDeviceInfo(deviceInfoList[i], ref isSolenoidNoBattery);
                        }
                    }
                    else if (remoteIP == udpPressorIp)
                    {
                        if (deviceInfoList[i].device.Equals("Pressor"))
                        {
                            SetDeviceInfo(deviceInfoList[i], ref isFallNoBattery);
                        }
                    }
                    else if (remoteIP == udpFallIp)      
                    {
                        if (deviceInfoList[i].device.Equals("Fall"))
                        {
                            SetDeviceInfo(deviceInfoList[i], ref isFallNoBattery);
                        }
                    }
                    else if (remoteIP == udpFanIp)
                    {
                        if (deviceInfoList[i].device.Equals("Fan"))
                        {
                            SetDeviceInfo(deviceInfoList[i], ref isFallNoBattery);
                        }
                    }
                    else if (remoteIP == udpHandPressureIP)
                    {
                        if (deviceInfoList[i].device.Equals("HandPressure"))
                        {
                            SetDeviceInfo(deviceInfoList[i], ref isFallNoBattery);
                        }
                    }
                    else if (remoteIP == udpPushIp)
                    {
                        if (deviceInfoList[i].device.Equals("Push"))
                        {
                            SetDeviceInfo(deviceInfoList[i], ref isFallNoBattery);
                        }
                    }
                    else if (remoteIP == udpKnockFloorIP)
                    {
                        if (deviceInfoList[i].device.Equals("KnockFloor"))
                        {
                            SetDeviceInfo(deviceInfoList[i], ref isFallNoBattery);
                        }
                    }

                }
            }
        }
    }
    #endregion

    #region 일반함수
    private string GetDicValue(string key)
    {
        if (textReadDic == null)
            return "dic is null";

        if (textReadDic.ContainsKey(key) == true)
        {
            return textReadDic[key];
        }
        else
            return "Key:X";
    }

    public List<string> RemoveDuplicateValue(List<string> array)
    {
        List<string> list = new List<string>();

        for (int i = 0; i < array.Count; ++i)
        {
            if (list.Contains(array[i]))
            {
                Debug.Log("중복된 값 발견" + array[i]);
                continue;
            }

            list.Add(array[i]);
        }

        return list;
    }

    public void SetShockPulseCount(int shockValue)
    {
        eShockPulseCount = shockValue;

        using (StreamWriter wr = new StreamWriter(strFilePath, false))
        {
            if (textReadDic.ContainsKey("eShockPulseCount"))
            {
                textReadDic["eShockPulseCount"] = eShockPulseCount.ToString();
            }

            foreach (var txt in textReadDic)
            {
                wr.WriteLine(txt.Key + ":" + txt.Value);
                Debug.Log(txt.Key + txt.Value);
            }
        }
    }

  
    public string GetRemoteEPIP(string remoteIP)
    {
        string[] remoteEpIP = remoteEP.ToString().Split(':');

        return remoteEpIP[0];
    }

    public float GetBatteryVoltage(DeviceInfo device, byte[] dgram)
    {
        float volt = BitConverter.ToSingle(dgram, 0);
        float battery = 0f;

        //Debug.Log("전압 : " + volt + "V");

        if (device.device.Equals("Solenoid"))
        {
            if (volt <= 9.9f)
            {
                battery = 0;
            }
            else if (volt >= 12.6f)
            {
                battery = 100;
            }
            else
                battery = (int)((volt - 9.9f) * 37.037037037);
        }
        else
        {
            if (volt <= 3.3f)
            {
                battery = 0;
            }
            else if (volt >= 4.15f)
            {
                battery = 100;
            }
            else
                battery = (int)((volt - 3.3f) * 117.6470588235294f);
        }

        return battery;
    }

    public uint GetUdpSARCount(byte[] dgram)
    {
        uint udpSARCount = BitConverter.ToUInt32(dgram, 4);
        return udpSARCount;
    }

    public int GetUdpSARSensitivity(byte[] dgram)
    {
        int dBm = BitConverter.ToInt32(dgram, 8);
        int udpSARSensitivityt = 0;
   
        if (dBm <= -100)
        {
            udpSARSensitivityt = 0;
        }
        else if (dBm >= -50)
        {
            udpSARSensitivityt = 100;
        }
        else
        {
            udpSARSensitivityt = 2 * (dBm + 100);
        }
        return udpSARSensitivityt;
    }

    //String을 바이트 배열로 변환 
    private byte[] StringToByte(string str)
    {
        byte[] StrByte = Encoding.UTF8.GetBytes(str);
        return StrByte;
    }

    //GUI 정보
    public void GUIDeviceInfo()
    {
        int labelContentCount = 1;
        int labelContentHeight = 30;
        int labelContentYGab = 3;

        for (int i = 0; i < glovesLabel.Length; ++i)
        {
            if (i % 2 == 0)
            {
                GUI.Label(new Rect(15, labelContentCount * (labelContentHeight + labelContentYGab), 145, labelContentHeight), glovesLabel[i]);
            }
            else
            {
                GUI.Label(new Rect(180, labelContentCount++ * (labelContentHeight + labelContentYGab), 145, labelContentHeight), glovesLabel[i]);
            }
        }
    }

    public void SetDeviceInfo(DeviceInfo device, ref bool isNoBattery)
    {
        device.batteryVoltage = GetBatteryVoltage(device, dgram);
        device.udpSARCount = GetUdpSARCount(dgram);
        device.udpSARSensitivity = GetUdpSARSensitivity(dgram);
        device.deviceConnectionText = "연결됨";

        if (device.batteryVoltage <= batteryCharge)          //배터리충전요청
        {
            isNoBattery = true;
        }
        else                                                  //그냥써도됨
        {
            isNoBattery = false;
        }
    }
    #endregion
}
