using Android.Content;
using Android.Content.PM;
using Android.Widget;
using AndroidTools.Contracts;
using Java.Lang;
using System.Linq;

namespace AndroidTools.Droid.Contracts
{
    /// <summary>
    /// APP安装、检测
    /// </summary>
    public class Install : IInstall
    {
        private readonly Context _context = MainActivity.Instance;

        public bool CheckAppInstalled(string packageName)
        {
            var packageInfo = _context.PackageManager.GetInstalledPackages(PackageInfoFlags.Activities).Where(pm => pm.PackageName == packageName).FirstOrDefault();

            if (packageInfo != null)
                return true;

            return false;
        }

        public bool SilentInstall(string apkPath)
        {
            try
            {
                string cmd = $"pm install -r {apkPath}";

                using Process p = Runtime.GetRuntime().Exec("su");

                Java.IO.BufferedReader ins = new Java.IO.BufferedReader(new Java.IO.InputStreamReader(p.InputStream));
                Java.IO.BufferedReader ie = new Java.IO.BufferedReader(new Java.IO.InputStreamReader(p.ErrorStream));
                Java.IO.BufferedWriter w = new Java.IO.BufferedWriter(new Java.IO.OutputStreamWriter(p.OutputStream));

                w.Write(cmd);
                w.Flush();
                w.Close();

                string infoLine = string.Empty;
                while ((infoLine = ie.ReadLine()) != null)
                {
                    Toast.MakeText(_context, $"Error:{infoLine}", ToastLength.Short).Show();
                }

                while ((infoLine = ins.ReadLine()) != null)
                {
                    Toast.MakeText(_context, $"Message:{infoLine}", ToastLength.Short).Show();
                }

                ins.Close();
                ie.Close();

                int res = p.WaitFor();
                Toast.MakeText(_context, $"Result:{res}", ToastLength.Short).Show();

                if (res is 0)
                    return true;
            }
            catch (Throwable ex)
            {
                Toast.MakeText(_context, $"Exception：{ex.Message}", ToastLength.Short).Show();
                return false;
            }
            return true;
        }
    }
}