namespace Common.Saga;

public interface ICommand<TEnum>
{
    public TEnum Type { get; set; }
    public TEnum UnknownType { get; }
}

public interface IReply<TEnum>
{
    public TEnum Type { get; set; }

    public TEnum UnknownType { get; }
}