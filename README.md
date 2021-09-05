# Financial Chat

This project has three parts, the server, the client and the bot.

## Client

The client is in angular and can be deployed with nodejs
Run `ng serve` for a dev server. Navigate to `http://localhost:4200/`. The app will automatically reload if you change any of the source files.
Repo: https://github.com/moloyo/financial-chat-client

## Server

The server was created with .net 5.0 and can be run with visual studio or running the attached binary files.
It's important the server runs on https://localhost:5001;. If not the environment file in the Client has to be updated.
If run from the binary files, it need a SQL server instance on (localdb)\mssqllocaldb.
If run from docker the connection string "DefaultConnection" as to be changed to "DockerConnection" on Startup.cs
Repo: https://github.com/moloyo/financial-chat-api

## Bot

The bot comes with an installer or can be run from visual studio.
The bot requires the API url on startup, like `https://localhost:5001/api`
Repo: https://github.com/moloyo/financial-chat-bot
