#ChannelFactory

A library primarily focused on seamless WCF Client Channel Creation. It tries to do away with minimum syntax, implementing channels on-the-fly.

Generated channels are descendants of ClientBase<TInterface> for every interface attributed with ServiceContract attribute.

Two types of channels are supported:
* Basic, which keeps the connection open
* Client, which creates a client channel on every invocation

Example:

Suppose we have a service exposing this interface contract:
```c#
    [ServiceContract]
    public interface ITestService
    {
        [OperationContract]
        int CopyMe(int x);
    }
```

In order to use it in clients, it is sufficient to write this code:

```c#
var factory = m_Container.Resolve<IChannelFactory>();
var channel = factory.Create(channelCreationFactory: m_ChannelCreationFactory);

var result = channel.CopyMe(1);
```