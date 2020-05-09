using System.Collections.Generic;
using Sproto;

public delegate void RpcRspHandler(SprotoTypeBase rpcRsp);

public class NetSender
{
    private static long session;
    private static Dictionary<long, RpcRspHandler> rpcRspHandlerDict;
    public static bool showWaitUI = false;

    public static void Init()
    {
        rpcRspHandlerDict = new Dictionary<long, RpcRspHandler>();
    }

    public static void Send<T>(SprotoTypeBase rpcReq = null, RpcRspHandler rpcRspHandler = null)
    {
        if (rpcRspHandler != null)
        {
            session++;
            AddHandler(session, rpcRspHandler);
            NetCore.Send<T>(rpcReq, session);
        }
        else
        {
            NetCore.Send<T>(rpcReq);
        }
    }

    private static void AddHandler(long session, RpcRspHandler rpcRspHandler)
    {
        rpcRspHandlerDict.Add(session, rpcRspHandler);
        showWaitUI = true;
    }

    private static void RemoveHandler(long session)
    {
        if (rpcRspHandlerDict.ContainsKey(session))
        {
            rpcRspHandlerDict.Remove(session);
        }

        if (rpcRspHandlerDict.Count > 0)
        {
            showWaitUI = true;
        } else
        {
            showWaitUI = false;
        }
    }

    public static void RemoveAllHandler()
    {
        rpcRspHandlerDict.Clear();
        showWaitUI = false;
    }

    public static RpcRspHandler GetHandler(long session)
    {
        RpcRspHandler rpcRspHandler;
        rpcRspHandlerDict.TryGetValue(session, out rpcRspHandler);
        RemoveHandler(session);
        return rpcRspHandler;
    }
}