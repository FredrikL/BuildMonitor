using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using Hardware.I2C.LED.BlinkM;
using Hardware.I2C.LED.BlinkM.Command;
using System.IO;

namespace BuildMonitor
{
    public class Program
    {
        public static void Main()
        {
            SetupNetworking();

            while (true) {
                BlinkMController.GetInstance().Write(new PlayLightScriptCommand(ScriptId.Hue_Cycle));

                var result = getStatus();
                
                BaseCommand cmd = null;
                
                switch (result)
                {
                    case Status.Ok:
                        cmd= new PlayLightScriptCommand(ScriptId.Green_Flash);
                        break;
                    case Status.Broken:
                        cmd = new PlayLightScriptCommand(ScriptId.Red_Flash);
                        break; 
                    case Status.Building:
                        cmd = new PlayLightScriptCommand(ScriptId.While_Flash);
                        break;
                    case Status.NetworkError:
                        cmd = new PlayLightScriptCommand(ScriptId.Stop_Light);
                        break;
                    case Status.ParseError:
                        cmd = new PlayLightScriptCommand(ScriptId.Blue_Flash);
                        break;
                    default:
                        cmd = new PlayLightScriptCommand(ScriptId.Black);
                        break;
                        
                }

                BlinkMController.GetInstance().Write(cmd);
                Thread.Sleep(30 * 1000);
            }

        }

        private static Status getStatus()
        {
            var checker = new TeamCityBuildCheck();
            var result = checker.Check();
            return result;
        }


        private static void SetupNetworking()
        {
            Microsoft.SPOT.Net.NetworkInformation.NetworkInterface NI = Microsoft.SPOT.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0];

            NI.EnableStaticIP("192.168.0.50", "255.255.255.0", "192.168.0.198");
            Debug.Print("Got IP: " + NI.IPAddress.ToString());      
        }
    }
}
