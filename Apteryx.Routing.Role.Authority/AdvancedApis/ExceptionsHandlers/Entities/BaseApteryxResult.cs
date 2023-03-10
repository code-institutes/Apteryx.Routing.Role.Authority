namespace Apteryx.Routing.Role.Authority
{
    public abstract class BaseApteryxResult : IApteryxResult
    {
        public ApteryxCodes Code { get; protected set; } = ApteryxCodes.请求成功;
        public string Msg { get; protected set; } = ApteryxCodes.请求成功.ToString();
        public DateTime serverTime => DateTime.Now;
    }

    public abstract class BaseApteryxResult<T> : IApteryxResult<T>
        where T : class
    {
        public T? Result { get; protected set; } = default(T);
        public ApteryxCodes Code { get; protected set; } = ApteryxCodes.请求成功;
        public string Msg { get; protected set; } = ApteryxCodes.请求成功.ToString();
        public DateTime serverTime => DateTime.Now;
    }
}
