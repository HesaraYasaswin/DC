# Mortal Kombat X - Online Gaming Lobby (WCF + WPF)

This project implements a .NET-based Online Gaming Lobby System for Mortal Kombat X. The application is designed as a distributed system using Windows Communication Foundation (WCF) for the server and Windows Presentation Foundation (WPF) for the client. Players can log in, join or create rooms, chat publicly or privately, and share files in a simulated multiplayer lobby environment.

This project was developed for COMP3008 (Distributed Computing) and demonstrates the core principles of client-server communication, multi-room management, real-time interaction via pull-based messaging, and file transfer between distributed clients.

## Project Overview

The system includes two main components:
- **Gaming Lobby Server (WCF)**: Manages users, rooms, messaging, and file sharing.
- **Client Application (WPF)**: Provides a graphical interface for players to interact with the system.

Communication between clients and server is conducted over TCP using a static IP and port for simplicity. The system is designed to operate on a single machine with support for multiple client instances (tested with 1 to 5 players).

## Technologies Used

| Component       | Technology                                  |
|----------------|----------------------------------------------|
| Server          | .NET WCF (Windows Communication Foundation) |
| Client          | .NET WPF (Windows Presentation Foundation)  |
| Programming     | C#                                          |
| Architecture    | Client-Server, Pull-based                   |
| Communication   | TCP/IP over static port                     |

## Server Features

- **Unique User Management**: Ensures no two users can log in with the same username simultaneously.
- **Lobby Room Management**: Supports creating, joining, and leaving rooms with dynamic room lists.
- **Public Messaging**: Broadcast messages to all users within the same room.
- **Private Messaging**: Send direct messages to specific users within the same room.
- **File Sharing**: Allows sharing of image (.jpg, .png) and text (.txt) files between users in the same room only.

## Client Features

- **Login System**: Prompts the user for a unique name to log in to the server.
- **Room Selection**: Displays a list of available rooms to join.
- **Room Creation**: Users can create new rooms with unique names.
- **Public Chat**: Send and receive messages visible to all users in the room.
- **Private Chat**: Send direct messages to specific users in the room.
- **File Sharing**: Upload and download shared files displayed as clickable links.
- **Manual Refresh**: Buttons are available to refresh room lists, user lists, and messages.

## Optional Real-Time Enhancement (Threaded Polling)

To simulate near real-time updates using a pull strategy, optional threading features are provided:
- A background thread periodically updates the list of rooms and active users.
- Another thread polls for new public messages, private messages, and shared files.
- GUI components are updated automatically when new data is received.

## How It Works

1. **Start the WCF Server**: Hosts a chat service on a static IP and port (e.g., `net.tcp://localhost:8080/ChatServer`).
2. **Run Multiple WPF Clients**: Each client instance connects to the server and logs in with a unique username.
3. **Interaction**: Users can chat, create/join rooms, share files, and communicate either publicly or privately.
4. **Updates**: Users can either manually refresh data or rely on optional background threads for automatic updates.
5. **Exit**: Users can log out and terminate their session cleanly from the client interface.

## Testing

- The system has been tested locally with 1 to 5 clients.
- All users connect to the same server endpoint hosted on the local machine.
- Each client runs as a separate instance and maintains independent session states.

## Getting Started

1. Open the solution in Visual Studio.
2. Start the WCF server project to begin hosting the lobby service.
3. Launch the WPF client application and log in with a unique name.
4. Interact with the server through lobby room selection, chat, and file sharing.
5. Open multiple instances of the client to simulate multiple users.

