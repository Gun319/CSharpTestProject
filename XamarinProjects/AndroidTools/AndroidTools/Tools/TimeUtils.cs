using System;

namespace AndroidTools.Tools
{
    public class TimeUtils
    {
        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// 时间转换
        /// </summary>
        /// <param name="dateTime">所选时间</param>
        /// <returns></returns>
        public static long CurrentTimewillis(DateTime dateTime) => (long)(dateTime - Jan1st1970).TotalMilliseconds;
    }
}
