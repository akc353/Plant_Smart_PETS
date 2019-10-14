using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using UnityEngine.UI;
using Microsoft.Win32;

public enum PRESSCODESELECT
{
    SOLTWORKS,
    LGDISPLAY
}

public class SerialBase : MonoBehaviour
{
    private string deviceName = @"\Device\Silabser0";
    public string SerialPORT = "COM3";
    public bool bConnectDevice = false;

    private byte[] serialSNDBUFF;
    private static SerialPort mySerial;

    private byte m_sol1;
    private byte m_sol2;
    private byte m_vibrate;
    private byte m_fan;

    public bool DEVICE_ABLE;

    //public void SetValue(byte _sol1, byte _sol2, byte _vibrate, byte _fan)
    //{
    //    if (!DEVICE_ABLE)
    //        return;

    //    m_sol1 = _sol1;
    //    m_sol2 = _sol2;
    //    m_vibrate = _vibrate;
    //    m_fan = _fan;
    //}

    private void Start()
    {
        serialSNDBUFF = new byte[9];
        connectDevice();
    }

    private void OnApplicationQuit()
    {
        disconnectDevice();
    }

    private void OnDisable()
    {
        disconnectDevice();
    }

    public void connectDevice()
    {
        disconnectDevice();
        //HKEY_LOCAL_MACHINE,"HARDWARE\\DEVICEMAP\\SERIALCOMM
        RegistryKey localmachine = Registry.LocalMachine;
        RegistryKey hardware = localmachine.OpenSubKey("HARDWARE");
        RegistryKey devicemap = hardware.OpenSubKey("DEVICEMAP");
        RegistryKey serialcomm = devicemap.OpenSubKey("SERIALCOMM");
        if (serialcomm != null)
        {
            Debug.Log("Serial Port : installed : " + serialcomm.ValueCount.ToString());
            foreach (string portName in serialcomm.GetValueNames())
            {
                if (deviceName == portName)
                {
                    SerialPORT = serialcomm.GetValue(portName).ToString();
                  
                }
            }
        }

        string connectPORT = @"\\.\" + SerialPORT;
        mySerial = new SerialPort(connectPORT, 115200);
        mySerial.ReadTimeout = 100;

        try
        {
            mySerial.Open();
        }
        catch (Exception e)
        {
            ;
        }

        if (mySerial.IsOpen)
        {
            Debug.Log("COM Open " + connectPORT);
            bConnectDevice = true;
            sendDATA(0, 0, 0xFE, 2);
        }
        else
        {
            Debug.Log("COM Not Open " + connectPORT);
            bConnectDevice = false;
        }
    }

    public void disconnectDevice()
    {
        if (mySerial != null)
        {
            if (mySerial.IsOpen)
            {
                bConnectDevice = false;
                mySerial.Close();
            }
        }
    }

    public void sendDATA(byte _sol1, byte _sol2, byte _vibrate, byte _fan)
    {
        if (bConnectDevice == false)
        {
            return;
        }

        // STX
        serialSNDBUFF[0] = 0x02;

        // 솔레노이드1
        serialSNDBUFF[1] = _sol1;

        // 솔레노이드2
        serialSNDBUFF[2] = _sol2;

        // 바닥진동
        serialSNDBUFF[3] = _vibrate;

        // 선풍기
        serialSNDBUFF[4] = _fan;

        // 리프트 및 히터
        serialSNDBUFF[5] = _fan;

        // CHECK SUM
        serialSNDBUFF[6] = (byte)((byte)serialSNDBUFF[1] + (byte)serialSNDBUFF[2] + (byte)serialSNDBUFF[3] + (byte)serialSNDBUFF[4]);

        // ETX
        serialSNDBUFF[7] = 0x03;

        mySerial.Write(serialSNDBUFF, 0, 8);
    }

    public void Solrenoid(byte sol1, byte sol2)
    {
        if (bConnectDevice == false)
        {
            return;
        }

        // STX
        serialSNDBUFF[0] = 0x02;

        // 솔레노이드1
        serialSNDBUFF[1] = sol1;

        // 솔레노이드2
        serialSNDBUFF[2] = sol2;

        // 바닥진동
        serialSNDBUFF[3] = 0x00;

        // 리프트 및 히터
        serialSNDBUFF[4] = 0x00;

        // 협착 프레스
        serialSNDBUFF[5] = 0x00;

        // 롤회전
        serialSNDBUFF[6] = 0x00;

        // CHECK SUM
        serialSNDBUFF[7] = (byte)((byte)serialSNDBUFF[1] + (byte)serialSNDBUFF[2] + (byte)serialSNDBUFF[3] + (byte)serialSNDBUFF[4] + (byte)serialSNDBUFF[5] + (byte)serialSNDBUFF[6]);

        // ETX
        serialSNDBUFF[8] = 0x03;

        Debug.Log("Send!!");
        mySerial.Write(serialSNDBUFF, 0, serialSNDBUFF.Length);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Solrenoid(0x01, 0x00);
        }

        if (Input.GetKeyDown(KeyCode.F6))
        {
            Solrenoid(0x00, 0x01);
        }

    }
}