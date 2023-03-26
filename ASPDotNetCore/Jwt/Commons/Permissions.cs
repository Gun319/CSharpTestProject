namespace Jwt.Commons
{
    /// <summary>
    /// 预定义策略
    /// </summary>
    public static class Permissions
    {
        public const string User = "User";
        public const string UserCreate = User + ".Create";
        public const string UserDelete = User + ".Delete";
        public const string UserUpdate = User + ".Update";
    }
}
