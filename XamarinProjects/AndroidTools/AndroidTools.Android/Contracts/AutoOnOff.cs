using Android.Content;
using Android.Widget;
using AndroidTools.Contracts;
using Java.Lang;
using Java.Util;

namespace AndroidTools.Droid.Contracts
{
    /// <summary>
    /// 自动开关机计划
    /// </summary>
    public class AutoOnOff : IAutoOnOff
    {
        #region 常量
        /// <summary>
        /// 开机 Intent
        /// </summary>
        private static readonly string SETPOWERON = "android.intent.action.setpoweron";
        /// <summary>
        /// 关机 Intent
        /// </summary>
        private static readonly string SETPOWEROFF = "android.intent.action.setpoweroff";
        /// <summary>
        /// 开机 poweron
        /// </summary>
        private static readonly string POWERON = "poweron";
        /// <summary>
        /// 关机 poweroff
        /// </summary>
        private static readonly string POWEROFF = "poweroff";
        /// <summary>
        /// enable
        /// </summary>
        private static readonly string ENABLE = "enable";
        #endregion

        private readonly Context _context = MainActivity.Instance;

        public bool PowerOffPlan(bool enable, long planTime)
        {
            try
            {
                Intent intent = new Intent(SETPOWEROFF);
                intent.PutExtra(ENABLE, enable);
                if (enable && planTime > 0)
                {
                    Date dateTime = new Date(planTime);
                    Calendar calendar = Calendar.GetInstance(Locale.SimplifiedChinese);

                    calendar.Time = dateTime;
                    calendar.Set(CalendarField.Second, 0);
                    int[] time = new int[]
                    {
                        calendar.Get(CalendarField.Year),
                        calendar.Get(CalendarField.Month),
                        calendar.Get(CalendarField.DayOfMonth),
                        calendar.Get(CalendarField.HourOfDay),
                        calendar.Get(CalendarField.Minute),
                        calendar.Get(CalendarField.Second),
                    };
                    intent.PutExtra(POWEROFF, time);
                }
                _context.SendBroadcast(intent);
                return true;
            }
            catch (Throwable ex)
            {
                Toast.MakeText(_context, $"Exception：{ex.Message}", ToastLength.Short).Show();
            }
            //Toast.MakeText(_context, "设置自动关机成功", ToastLength.Short).Show();
            return false;
        }

        public bool PowerOnPlan(bool enable, long planTime)
        {
            try
            {
                Intent intent = new Intent(SETPOWERON);
                intent.PutExtra(ENABLE, enable);
                if (enable && planTime > 0)
                {
                    Date dateTime = new Date(planTime);
                    Calendar calendar = Calendar.GetInstance(Locale.SimplifiedChinese);

                    calendar.Time = dateTime;
                    calendar.Set(CalendarField.Second, 0);
                    int[] time = new int[]
                    {
                        calendar.Get(CalendarField.Year),
                        calendar.Get(CalendarField.Month),
                        calendar.Get(CalendarField.DayOfMonth),
                        calendar.Get(CalendarField.HourOfDay),
                        calendar.Get(CalendarField.Minute),
                        calendar.Get(CalendarField.Second),
                    };
                    intent.PutExtra(POWERON, time);
                }
                _context.SendBroadcast(intent);
                return true;
            }
            catch (Throwable ex)
            {
                Toast.MakeText(_context, $"Exception：{ex.Message}", ToastLength.Short).Show();
            }
            //Toast.MakeText(_context, "设置自动开机成功", ToastLength.Short).Show();
            return false;
        }
    }
}