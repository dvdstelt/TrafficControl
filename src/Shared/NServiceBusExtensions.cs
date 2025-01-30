using System.Runtime.CompilerServices;
using NServiceBus;

namespace Shared;

public static class NServiceBusExtensions
{
    public static EndpointConfiguration Configure(this EndpointConfiguration endpointConfiguration,
        Action<RoutingSettings<LearningTransport>> configureRouting = null!)
    {
        var transport = endpointConfiguration.UseTransport<LearningTransport>();
        endpointConfiguration.UsePersistence<LearningPersistence>();
        endpointConfiguration.UseSerialization<SystemJsonSerializer>();

        configureRouting?.Invoke(transport.Routing());

        var conventions = endpointConfiguration.Conventions();
        conventions.DefiningCommandsAs(t => t.Namespace != null && t.Namespace.EndsWith("Commands"));
        conventions.DefiningEventsAs(type => type.Namespace != null && type.Namespace.EndsWith("Events"));
        conventions.DefiningMessagesAs(type => type.Namespace != null && type.Namespace.EndsWith("Messages"));

        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.AuditProcessedMessagesTo("audit");

        endpointConfiguration.Recoverability().Delayed(c => c.NumberOfRetries(0));

        endpointConfiguration.EnableInstallers();

        return endpointConfiguration;
    }
}