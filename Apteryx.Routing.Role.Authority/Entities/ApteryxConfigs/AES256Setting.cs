namespace Apteryx.Routing.Role.Authority
{
    /// <summary>
    /// 
    /// </summary>
    public class AES256Setting
    {
        /// <summary>
        /// KEY
        /// </summary>
        public required string Key { get; set; }

        /// <summary>
        /// 偏移
        /// </summary>
        public required string IV { get; set; }
    }
}
