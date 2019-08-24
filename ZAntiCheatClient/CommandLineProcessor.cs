namespace ZAntiCheatClient
{
    class CommandLineProcessor
    {
        public static MCClient mcc;
        public static string MD5 = "";
        public static bool isMCProcess(string cmdLine)
        {
            return cmdLine.Contains("uuid") && cmdLine.Contains("Dminecraft.client.jar") && cmdLine.Contains("accessToken");
        }
        public static void getVerifyMD5()
        {
            MD5 = Network.getStandardMD5();
            Shell.WriteLine("md5:{0}", MD5);
        }
        private static string getJarPath(string gameDir, string dcj)
        {
            string ret = "";
            string[] gd = gameDir.Split('\\');
            for (int i = 0; i < gd.Length - 1; i++)
            {
                ret = ret + gd[i] + '\\';
            }
            return ret + dcj;
        }
        public static bool isVailidClient(string cmdLine)
        {
            mcc = MCClient.resolveCmdline(cmdLine);
            Shell.WriteLine("gamedir:{0} uuid:{1} dcj:{2} uname:{3}", mcc.gameDir, mcc.uuid, mcc.Dminecraft_client_jar, mcc.username);
            //Shell.WriteLine(getJarPath(mcc.gameDir,mcc.Dminecraft_client_jar));
            //Shell.WriteLine(FILEMD5.GetMD5HashFromFile(getJarPath(mcc.gameDir, mcc.Dminecraft_client_jar)));
            return FILEMD5.GetMD5HashFromFile(getJarPath(mcc.gameDir, mcc.Dminecraft_client_jar)) == MD5;
        }
        public static string getClientName()
        {
            return mcc.username;
        }
        public static string getClientUUID()
        {
            return mcc.uuid;
        }
    }
    class MCClient
    {
        public string gameDir;
        public string uuid;
        public string Dminecraft_client_jar;
        public string username;
        public static MCClient resolveCmdline(string cmdline)
        {
            MCClient ret = new MCClient();
            string[] args = cmdline.Split(' ');
            for (int i = 0; i < args.Length; i++)
            {
                string nowstr = args[i];
                if (nowstr.Contains("-Dminecraft.client.jar"))
                {
                    string[] tmp = nowstr.Split('=');
                    ret.Dminecraft_client_jar = tmp[1];
                }
                else if (nowstr.Contains("--uuid"))
                {
                    ret.uuid = args[i + 1];
                }
                else if (nowstr.Contains("--username"))
                {
                    ret.username = args[i + 1];
                }
                else if (nowstr.Contains("--gameDir"))
                {
                    ret.gameDir = args[i + 1];
                }
            }
            return ret;
        }
    }
}
