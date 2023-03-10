namespace Apteryx.Routing.Role.Authority
{
    public interface IApteryxResult
    {
        ApteryxCodes Code { get; }
        string Msg { get; }
    }

    public interface IApteryxResult<out T> : IApteryxResult
        where T : class
    {
        T Result { get; }
    }
}
