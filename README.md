# .net-core-3.1-mass-transit-example
A project that demonstrates how to use .NET Core, Generic Host Builder, and Mass Transit.

- Dependency injection
- Multiple environments
  - Development
  - QA
  - Production

### Background Service 
https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-3.1&tabs=visual-studio

### Generic Host
https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.1

### Mass Transit
https://masstransit-project.com/

### Rabbit MQ
https://www.rabbitmq.com/

### Scheduler
Note: this project uses the in-memory scheduler for the purpose of demonstrating scheduled retries when an error occurred before the consumer completes processing a message. Be advised this approach should be avoided in production applications.
For more information see the link below.
https://masstransit-project.com/advanced/scheduling/

### NLog
https://nlog-project.org/
