using System;
using Microsoft.SPOT;
using System.Net;
using System.IO;

namespace BuildMonitor
{
    public class TeamCityBuildCheck
    {
        const string hostname = "";

        const string url = "http://" + hostname +"/httpAuth/app/rest/cctray/projects.xml";
        const string localPath = @"\SD\tc.xml";
        const string username = "", password = "";

        public Status Check()
        {
            if (!downloadFile())
            {
                return Status.NetworkError;
            }
            return checkFile();
        }

        private Status checkFile()
        {
            try
            {
                var status = Status.Ok;

                TextReader sr = new StreamReader(new FileStream(localPath, FileMode.Open));
                string sLine = null;


                sLine = sr.ReadToEnd();
                sr.Close();
                sr = null;
                var x = sLine.Split(new char[] { '>' });
                sLine = null;
                foreach (var line in x)
                {
                    Debug.Print(line);

                    if (line.IndexOf("Tretton37 company website :: ") > 0)
                    {
                        if (line.IndexOf("activity=\"Building\"") > 0)
                        {
                            status = Status.Building;
                            break;
                        }
                        if (line.IndexOf("lastBuildStatus=\"Failure\"") > 0)
                        {
                            status = Status.Broken;
                        }
                    }
                }

                return status;
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
                Debug.Print(e.StackTrace);

                return Status.ParseError;
            }
        }

        private bool downloadFile()
        {

            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Credentials = new NetworkCredential(username, password);
                var reponse = req.GetResponse();
                var strm = reponse.GetResponseStream();

                FileStream fs = new FileStream(localPath, FileMode.Create);
                byte[] b = new byte[64];
                int c = 0;
                while ((c = strm.Read(b, 0, 64)) > 0)
                    fs.Write(b, 0, c);
                strm.Close();
                fs.Flush();
                fs.Close();
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
                Debug.Print(e.StackTrace);
                return false;
            }
            return true;
        }
    }


    public enum Status
    {
        Ok, Broken, Building, NetworkError, ParseError
    }

}
