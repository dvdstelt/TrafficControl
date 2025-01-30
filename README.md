# NServiceBus Traffic Control Sample

| Attribute            | Details                   |
| -------------------- | ------------------------- |
| Language             | C#                        |
| Platform             | .NET 8                    |
| Environment          | Self hosted or Docker     |

This repository contains a sample application that simulates a traffic-control system using NServiceBus.
It is based purely of the idea from Edwin van Wijk his [sample application using Dapr](https://github.com/EdwinVW/dapr-traffic-control/). Attending his Dapr workshop, I found the case quite entertaining and wanted to build my own version. No code was copied, everything was created by hand. And maybe a little Claude AI.

## Run the demo

The following projects need to be run, everything will run automatically. No need to install anything.

- Simulation  
  Sends messages to simulate vehicles driving through a zone.
- TrafficControl  
  Keeps track of cars entering and leaving a zone, calculating their average speed.
- FineCollection
  Makes sure to fine people. All it does right now is send an email.
- VehicleRegistration  
  Randomly generates fake vehicles, persons and email addresses.

It sends email messages to an SMTP server running on `localhost:25`. To test this, it is recommended to use [smtp4dev](https://github.com/rnwood/smtp4dev) which allows you to see incoming emails.
When no SMTP server is running, those messages will fail and end up in the error queue. Which is the expected scenario, so the operations team can fix the issue and replay all those messages using ServicePulse.

## Disclaimer

The code in this repo is NOT production grade and lacks any automated testing. Its primary purpose is demonstrating how a similar sample works with NServiceBus. You might be able to get some ideas how to build your own project though. And you're always welcome to contact me for info.

The author can in no way be held liable for damage caused directly or indirectly by using this code.
