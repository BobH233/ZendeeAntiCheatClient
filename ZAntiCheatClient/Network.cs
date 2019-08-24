using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace ZAntiCheatClient
{

    class Network
    {
        private static string API_KEY = "[APIKEY]";
        //mind to add api key
        private static string API_MD5 = "http://[APIURL]/md5.php";
        private static string API_SERVER_STATUS = "http://[APIURL]/ServerStatus.php";
        private static string API_UPLOAD_UUID = "http://[APIURL]/UUIDbeat.php";
        private static string API_CANCEL_UUID = "http://[APIURL]/cUUID.php";
        private static string API_EXIT = "http://[APIURL]/canexit.php";
        public static long GetTimeStamp()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }
        public static bool getCanexit(string uuid)
        {
            string str = GetResponseString(CreateGetHttpResponse(API_EXIT + "?uuid=" + uuid));
            if (str == "true") return true;
            else return false;
        }
        public static string getStandardMD5()
        {
            Random rd = new Random();
            long time = rd.Next() + GetTimeStamp();
            string token = STRMD5.GetMD5HashString(time + "??!?!/!?" + API_KEY + "?!@?2?!" + time);
            //Shell.WriteLine("time:{0} apikey:{1} token:{2}",time,API_KEY,token);
            Dictionary<string, string> ps = new Dictionary<string, string>();
            ps.Add("time", time.ToString());
            ps.Add("token", token);
            ps.Add("apikey", API_KEY);
            return GetResponseString(CreatePostHttpResponse(API_MD5, ps));
        }
        public static string UUIDBeat(string uuid)
        {
            Random rd = new Random();
            long time = rd.Next() + GetTimeStamp();
            string token = STRMD5.GetMD5HashString(time + "??!?!/!?" + API_KEY + "?!@?2?!" + time);
            //Shell.WriteLine("time:{0} apikey:{1} token:{2}",time,API_KEY,token);
            Dictionary<string, string> ps = new Dictionary<string, string>();
            ps.Add("time", time.ToString());
            ps.Add("token", token);
            ps.Add("apikey", API_KEY);
            ps.Add("uuid", uuid);
            return GetResponseString(CreatePostHttpResponse(API_UPLOAD_UUID, ps));
        }
        public static string setCancelUUID(string uuid)
        {
            Random rd = new Random();
            long time = rd.Next() + GetTimeStamp();
            string token = STRMD5.GetMD5HashString(time + "??!?!/!?" + API_KEY + "?!@?2?!" + time);
            //Shell.WriteLine("time:{0} apikey:{1} token:{2}",time,API_KEY,token);
            Dictionary<string, string> ps = new Dictionary<string, string>();
            ps.Add("time", time.ToString());
            ps.Add("token", token);
            ps.Add("apikey", API_KEY);
            ps.Add("uuid", uuid);
            return GetResponseString(CreatePostHttpResponse(API_CANCEL_UUID, ps));
        }
        public static string getServerStatus()
        {
            Random rd = new Random();
            long time = rd.Next() + GetTimeStamp();
            string token = STRMD5.GetMD5HashString(time + "??!?!/!?" + API_KEY + "?!@?2?!" + time);
            //Shell.WriteLine("time:{0} apikey:{1} token:{2}",time,API_KEY,token);
            Dictionary<string, string> ps = new Dictionary<string, string>();
            ps.Add("time", time.ToString());
            ps.Add("token", token);
            ps.Add("apikey", API_KEY);
            return GetResponseString(CreatePostHttpResponse(API_SERVER_STATUS, ps));
        }
        /// <summary>
        /// 发送http post请求
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="parameters">查询参数集合</param>
        /// <returns></returns>
        static bool haserror = false;
        public static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;//创建请求对象
            request.Method = "POST";//请求方式
            request.ContentType = "application/x-www-form-urlencoded";//链接类型
                                                                      //构造查询字符串
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                bool first = true;
                foreach (string key in parameters.Keys)
                {

                    if (!first)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                        first = false;
                    }
                }
                byte[] data = Encoding.UTF8.GetBytes(buffer.ToString());
                //写入请求流
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            try
            {
                return request.GetResponse() as HttpWebResponse;
            }
            catch (Exception ee)
            {
                if (!haserror)
                {
                    haserror = true;
                    MessageBox.Show("网络错误,即将退出");
                }
                Application.Exit();
                return null;
            }

        }
        /// <summary>
        /// 发送http Get请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static HttpWebResponse CreateGetHttpResponse(string url)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";//链接类型
            return request.GetResponse() as HttpWebResponse;
        }
        /// <summary>
        /// 从HttpWebResponse对象中提取响应的数据转换为字符串
        /// </summary>
        /// <param name="webresponse"></param>
        /// <returns></returns>
        public static string GetResponseString(HttpWebResponse webresponse)
        {
            if (webresponse == null) return "";
            using (Stream s = webresponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(s, Encoding.UTF8);
                return reader.ReadToEnd();
            }
        }
    }
}
