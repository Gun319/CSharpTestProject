namespace AndroidTools.Contracts
{
    /// <summary>
    /// 自动开关机计划
    /// </summary>
    public interface IAutoOnOff
    {
        /// <summary>
        /// 开机计划
        /// </summary>
        /// <param name="context"></param>
        /// <param name="enable">管理计划</param>
        /// <param name="planTime">开机时间设定</param>
        bool PowerOnPlan(bool enable, long planTime);

        /// <summary>
        /// 关机计划
        /// </summary>
        /// <param name="context"></param>
        /// <param name="enable">管理计划</param>
        /// <param name="planTime">开机时间设定</param>
        bool PowerOffPlan(bool enable, long planTime);
    }
}
