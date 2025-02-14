# NServiceBus Traffic Control Sample

| Attribute            | Details                   |
| -------------------- | ------------------------- |
| Language             | C#                        |
| Platform             | .NET 8                    |
| Main Library         | NServiceBus               |
| Environment          | Self hosted or Docker     |

This repository contains a sample application that simulates a traffic-control system using NServiceBus.
It is based purely of the idea from Edwin van Wijk his [sample application using Dapr](https://github.com/EdwinVW/dapr-traffic-control/). Attending his Dapr workshop, I found the case quite entertaining and wanted to build my own version. No code was copied, everything was created by hand. And maybe a little Claude AI.

## Architecture

The main goal is to demonstrate messaging with NServiceBus in the Event Driven Architecture style.

The demo demonstrates how to deal with messages and how to structure your system into different components (NServiceBus endpoints) that each serve their own purpose. When to use commands, events, and how to use the request/response pattern.

### Diagram

The architecture is made up of the following components

- **Simulation** is the component that simulates the creation of messages that would normally arrive after cameras detect vehicles
- **TrafficControl** is the heart of the system.
  - It keeps track of cars entering and exiting a zone and calculates the speed based on how much time it took them  
    If the average speed is too high, it will inform FineCollection
  - It keeps track of cars that don't leave for a long period of time and likely have broken down  
  - It keeps track of cars that the police wants to get notified about  
- **FineCollection**
  - This will send a message to email to make sure a fine is collected
  - It simulates that if after 10 seconds, the payment wasn't done, it will send a reminder
- **VehicleRegistration** is an endpoint that has information about vehicles and their owners based on license plates
  It will randomly create names, car brand and model and more
- **PoliceAPI** is an HTTP WebApi
  - It provides a list if hashed license plates
  - If a license plate is recognized, it should be reported that it has been seen
- **Email** is an endpoint for sending emails
  - Every message that results in an email is an event
  - Use [smtp4dev](https://github.com/rnwood/smtp4dev) to see emails in its desktop client

<p align="center">
  <img src="/docs/diagram.svg" width=70% />
</p>

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
