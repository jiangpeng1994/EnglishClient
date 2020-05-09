using System.Collections.Generic;
using Sproto;

public delegate SprotoTypeBase RpcReqHandler(SprotoTypeBase rpcReq);

public class NetReceiver
{
    private static ProtocolFunctionDictionary protocol = ProtoProtocol.Instance.Protocol;
    private static Dictionary<int, RpcReqHandler> rpcReqHandlerDict;

    public static void Init()
    {
        rpcReqHandlerDict = new Dictionary<int, RpcReqHandler>();
    }

    public static void AddHandler(int tag, RpcReqHandler rpcReqHandler)
    {
        try
        {
            rpcReqHandlerDict.Add(tag, rpcReqHandler);
        }
        catch (System.Exception)
        {
        }

    }

    public static int AddHandler<T>(RpcReqHandler rpcReqHandler)
    {
        int tag = protocol[typeof(T)];
        AddHandler(tag, rpcReqHandler);
        return tag;
    }

    public static void RemoveHandler(int tag)
    {
        if (rpcReqHandlerDict.ContainsKey(tag))
        {
            rpcReqHandlerDict.Remove(tag);
        }
    }

    public static void RemoveHandler<T>()
    {
        RemoveHandler(protocol[typeof(T)]);
    }

    public static void RemoveAll()
    {
        try
        {
            rpcReqHandlerDict.Clear();
        }
        catch (System.Exception)
        {
        }

    }

    public static RpcReqHandler GetHandler(int tag)
    {
        RpcReqHandler rpcReqHandler;
        rpcReqHandlerDict.TryGetValue(tag, out rpcReqHandler);
        return rpcReqHandler;
    }

    public static RpcReqHandler GetHandler<T>()
    {
        return GetHandler(protocol[typeof(T)]);
    }

}
