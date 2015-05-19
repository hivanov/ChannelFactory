#ChannelFactory

## Description
A library primarily focused on seamless WCF Client Channel Creation. It tries to do away with minimum syntax, implementing channels on-the-fly.

## Overview
Generated channels are descendants of ClientBase<TInterface> for every interface attributed with ServiceContract attribute.

Two types of channels are supported:
* Basic, which keeps the connection open
* Client, which creates a client channel on every invocation

## A quick example:

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
container = new WindsorContainer();
container.Install(new ChannelFactoryWindsorInstaller());

var channelCreationFactory = new ChannelCreationFactory<ITestService>(serviceUri);
var factory = container.Resolve<IChannelFactory>();
var channel = factory.Create(channelCreationFactory: channelCreationFactory);

var result = channel.CopyMe(1);
```

# Where does the Channel Factory come in handy?

Suppose you have a large base of web services, all written in C#. For every one of them, a public, redistributable Service Contract Interface is present. Writing client consumer applications for the services can become tedious operation. 
Enter the Channel Factory. It allows for generic (currently only synchronous) clients creation without adding generators, service references, etc. in the projects.
Automating the process of communication, it does away with all the tools needed for redistributing the services (service references, synchronizing boilerplate code, etc.)
The only thing needed for the communication is a simple ServiceContract interface.
Also, face deployment automation: Windsor XML Configuration allows us to set up channels pointing to arbitrary locations, fixed at deployment time.

# Constitutes

## IChannelCreationFactory

* Member of ChannelFactory.Channels

### Summary
An interface for basic channel creation

### Description
This interface creates direct communication channels for WCF communication. Descendants of ClientBase<TInterface>, the created channels are used for network communication.
The reference implementation, *ChannelCreationFactory*, should be enough for almost all needs.

```c#
public interface IChannelCreationFactory<TChannel,TInterface>
	where TChannel : System.ServiceModel.ClientBase<TInterface>
	where TInterface : class
```

Type Parameters:
TChannel: The type of the channel.
TInterface: The type of the interface.


## ClientChannel
* Member of ChannelFactory.Channels

### Summary
A basic ClientBase<T> wrapper with no additional logic.

### Description
A basic network channel, used for network communication. Should be sufficient for almost all needs.
Note that it does not implement the interface T -- it just provides the communication basics.
```c#
public class ClientChannel<T> : System.ServiceModel.ClientBase<T>
	where T : class
```

## IChannelFactory
* Member of ChannelFactory.Contracts

### Summary
This is the central reference of the library. It creates channel implementations dynamically (creating assemblies on-the-fly.)

```c#
public interface IChannelFactory
```


# Examples
Most of the example usages can be found in Tests/UnitTests folder.