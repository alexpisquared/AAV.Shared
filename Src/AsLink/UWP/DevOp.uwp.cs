using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.UI.ViewManagement;

namespace AsLink
{
    public static partial class DevOp
    {
        public static DateTime BuildTime(Type appType) //tu: ...but set [assembly: AssemblyVersion("0.1.*")] !!!
        {
            //appType = typeof(Windows.UI.Xaml.Application);

            var ver = appType.GetTypeInfo().Assembly.GetName().Version;
            if (ver.Build < 10)
                Debug.WriteLine("WARNING: [assembly: AssemblyVersion(\"1.0.0.0\")] is not set to \"0.1.*\"");
            return new DateTime(2000, 1, 1)
              + TimeSpan.FromDays(ver.Build)
              + TimeSpan.FromSeconds(2 * ver.Revision);
        }
        public static string CurVerStr(string pls) { return string.Format("{0}.{1}", CurVer, pls); }
        public static Version CurVer
        {
            get
            {
                return new Version(0, 0);
                //#if usingSystemDeploymentApplication //  <== define for Release
                // System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed ?
                //          System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion : // z.ves.: ClickOnce deployed.
                //#endif
                // Assembly.GetEntryAssembly().GetName().Version;       // BMO.OLP.DAQ -.EXE !!!
                //                                                      //          Assembly.GetCallingAssembly().GetName().Version;		// BMO.OLP.VwMdl.DAQ - use this as the most likely to change, thus be reflective of the latest state.
                //                                                      //          Assembly.GetExecutingAssembly().GetName().Version;	// BMO.OLP.Common
                //                                                      // FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime.ToString("MM.dd HH"); 
            }
        }

        public static string CurVerStr(object sqlEnv)
        {
            throw new NotImplementedException("AP: Later ...maybe...");
        }

        public static string CompileMode
        {
            get
            {
#if DEBUG
                return Debugger.IsAttached ? "Dbg-Atchd" : "Dbg";
#else
        return Debugger.IsAttached ? "Rls-Atchd" : "Rls";
#endif
            }
        }

        public static ISysLogger SysLogger = null;

        public static bool IsVIP
        {
            get
            {
                var un = Environment.GetEnvironmentVariable("UserName").ToLower();
                return un.Contains("zzz") || un.Contains("pigid") || un.Contains("lex");
            }
        }
        public static string ExHrT(Exception ex, string info = "", [CallerMemberName] string caller = null)
        {
            Exception imex = null;
            try
            {
                BeepEr();

                imex = GetInnermostException(ex);
                try { ApplicationView.GetForCurrentView().Title = imex.Message; } catch (Exception ex2) { Debug.WriteLine(ex2.Message, " ************** Ignore this one ************** "); }

                var msg4cb = GetErrMsg4Cb(imex, caller, info);

                ApplicationData.Current.RoamingSettings.Values["Exn"] = msg4cb;

#if !RAD_DEBUG
                try { AppendTextToClipboard(msg4cb); } catch (Exception ex2) { Debug.WriteLine(ex2.Message, " ************** Ignore this one ************** "); }
#endif

#if DEBUG
                log(ex, caller, info, msg4cb);
#else
        Task.Run(() => log(ex, caller, info, msg4cb));
#endif

                if (Debugger.IsAttached) Debugger.Break();
            }
            catch (Exception fatalEx) { Environment.FailFast("*&^%$#: An error occured ...whilst reporting an error.", fatalEx); }//tu: http://blog.functionalfun.net/2013/05/how-to-debug-silent-crashes-in-net.html // Capturing dump files with Windows Error Reporting: Create a key at HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\Windows Error Reporting\LocalDumps\[Your Application Exe FileName]. In that key, create a string value called DumpFolder, and set it to the folder where you want dumps to be written. Then create a DWORD value called DumpType with a value of 2.

            return $"{DateTime.Now:ddd HH:mm} - {info}.{caller}() \n  {imex?.Message}\n";
        }
        static void log(Exception ex, string caller, string info, string msg4fs)
        {
            Debug.WriteLine(msg4fs);

            if (SysLogger != null)
                SysLogger.LogMessage(GetErrMsg4Db(GetInnermostException(ex), caller, info), "Exception");
        }

        public static void AppendTextToClipboard(string exMsg)
        {
            try
            {
                var dataPackage = new DataPackage();
                var dataPkgView = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();
                if (dataPkgView.Contains(StandardDataFormats.Text))
                    dataPackage.SetText(exMsg + Environment.NewLine + Environment.NewLine + dataPkgView.GetTextAsync().GetResults());
                else
                    dataPackage.SetText(exMsg);

                Clipboard.SetContent(dataPackage);
            }
            catch (Exception ex) { ApplicationView.GetForCurrentView().Title = "Clipboard ass-t has failed: " + ex.Message; }
        }


        public static string GetErrMsg4Db(Exception ex, string caller, string info = "") => $"{DateTime.Now.ToString("MMM-dd HH:mm")}   {caller}({info}): \r\n\n{ex.Message} \r\n\nStack: \r\n{ex.StackTrace} \r\n\nSource: \r\n{ex.Source} ";
        public static string GetErrMsg4Cb(Exception ex, string caller, string info = "") => $"{DateTime.Now.ToString("MMM-dd HH:mm")}   {caller}({info}): \r\n\n{ex.Message} \r\n\nStack: \r\n{ex.StackTrace}";
        public static string GetErrMsg4Fs(Exception ex, string caller, string info = "") => $"\r\n{new string('═', 110)}\r\n!Exn: {DateTime.Now.ToString("MMM-dd HH:mm")} in {caller}({info}): \r\n\n{ex.Message}\r\n\nStack: \r\n{ex.StackTrace}\n{new string('═', 110)}\r\n\n";

        public static Exception GetInnermostException(this Exception ex) => ex.InnerException != null ? GetInnermostException(ex.InnerException) : ex;

        public static string LocalAppData
        {
            get
            {
                return "//todo:...";
                //var rv = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", "Local AppData", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData\\Local"));
                //if (rv == null) rv = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Explorer\Shell Folders", "Local AppData", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData\\Local"));
                //return rv != null ? rv.ToString() : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData\\Local"); // Sep 25, 2014 ... feels like there is a copy with this fix already: use compare to find out which one is the best.
            }
        }

        public static string MachineName
        {
            get
            {
                //654: Debug.WriteLine($"'{Environment.GetEnvironmentVariable("COMPUTERNAME")}' - '{Environment.GetEnvironmentVariable("MachineName")}' - '{NetworkInformation.GetHostNames().FirstOrDefault(name => name.DisplayName.Contains(".local")).DisplayName.Replace(".local", "")}' - '{Environment.ProcessorCount} cores.'");
                return //always null: Environment.GetEnvironmentVariable("COMPUTERNAME") ?? Environment.GetEnvironmentVariable("MachineName") ?? 
                  NetworkInformation.GetHostNames().FirstOrDefault(name => name.DisplayName.Contains(".local")).DisplayName.Replace(".local", "");
            }
        }

        public static string LocalAppDataFolder(string subFolder) { return Path.Combine(LocalAppData, subFolder); }

        public static string GetCaller([System.Runtime.CompilerServices.CallerMemberName] string memberName = "") { return memberName; }

        //<< Beep
        const int mind = 41, medd = 50; // minimal and medium durations on Dell 990.
        static double K = .95;
        static int N = 9, D = mind, F = 9000;

        public static void BeepOk() { BeepFD(8400, medd); }
        public static void BeepOkB() { BeepFD(6300, medd); }
        public static void BeepClk() { BeepFD(10000, medd); }
        public static void Beep2hort() { BeepFD(11000, mind); }
        public static void BeepHi() { BeepFD(12000, mind); }
        public static void BeepLong() { BeepFD(8000, 100); }
        public static void BeepNo() { BeepFD(4000, mind); BeepFD(4000, mind); }
        public static void Beep1of2() { BeepFD(6000, medd); BeepFD(7700, mind); }
        public static void Beep2of2() { BeepFD(7700, mind); BeepFD(5000, medd); }
        public static void BeepEnd6() { BeepFD(7700, mind); BeepFD(6000, mind); BeepFD(5000, mind); BeepFD(4000, 100); BeepFD(7700, 100); BeepFD(7700, 100); }
        public static void BeepN(int n) { for (int i = 0; i < n; i++) BeepFD(9999, 22); }
        public static void BeepN(int f, int d, int n, int df) { for (int i = 0; i < n; i++) BeepFD(f += df, d); }
        public static void BeepFDNK(int f = 3000, int d = 70, int n = 3, double k = .04) { for (int i = 0; i < n; i++) BeepFD(f = ((int)(f * (1.0 + k))), d); F = f; D = d; N = n; K = k; }
        public static void BeepFDNKAsy(int f = 3000, int d = 70, int n = 3, double k = .04) { Task.Run(() => BeepFDNK(f, d, n, k)); }
        public static void BeepDone() { for (int i = 0; i < N; i++) BeepFD(F = ((int)(F * (1.0 - K))), D); }

        public static void BeepFD(int freq, int dur) { Beep(freq, dur); }
        public static void BeepEr()
        {
#if DEBUG
            Task.Run(() =>
            {
                Beep(6000, 40);
                Beep(6000, 40);
                Beep(6000, 40);
                Beep(5000, 80);
            });
#else
      BeepFD(10000, 25);
#endif
        }
        public static void Beep2mallError()                         /**/
        { for (double i = 8000; i < 9000; i *= 1.141) Beep((int)i, 12); }
        public static void BeepBigError()                           /**/
        { for (double i = 1000; i < 5000; i *= 1.5) Beep((int)i, (int)(10000 / i)); }
        public static void BeepFinish()                             /**/
        { for (double i = 300; i > 240; i *= 0.9) Beep((int)i, (int)(1000 / i)); }
        static void dbgSleepAndBeep(ref int cummulativeDelay, bool final = false)
        {
            Task.Delay /*Thread.Sleep*/(cummulativeDelay += 500);
            if (final) { Beep(9900, 40); Beep(9000, 40); Beep(81000, 140); }
            else { Beep(9000, 40); Beep(9900, 40); }
        }
        public static void BeepTestPlayAll()
        {
            Beep2hort();
            BeepOk();
            BeepLong();
            BeepOkB();
            Beep1of2();
            Beep2of2();
            BeepEr();
        }


        //  [DllImport(ExactSpelling=true) 
        //[System.Runtime.InteropServices.DllImport("kernel32.dll")]
        //public static extern bool Beep(int freq, int dur);
        public static void Beep(int freq, int dur) { }

        //>>
    }
    public interface ISysLogger
    {
        void LogSessionStart();
        void LogSessionEnd();
        void LogException(Exception ex, string caller = null);
        void LogMessage(string message, string logReason = "Info.");
    }
}