# Unity/PlayFab Based Authentication Server
An authentication server built directly inside Unity that utilizes PlayFab log-in services.

## About
This is an http authentication server that I'm playing around with for my upcoming dungeon-builder game, Delfan. It uses PlayFab for login services so you don't have to worry about user data security and setting up/maintaining options to log in through many different services. PlayFab already has built in authentication code but this authentication server is designed specifically for player-run servers. IE games similar to, Minecraft, Rust, etc. This server allows a player run server to contact a developer run authentication server when a player attempts to connect which will authenticate the user using Playfab's Server API. 

*Please note, I have no idea what I'm doing. This is an experimental idea that I'm using to learn. I would not recommend using it in production but it can be a stepping stool to help other people learn who are trying to learn this kind of thing, too. Also note that a production version should run on HTTPS, not HTTP.* 

![Authentication Server Flow](https://user-images.githubusercontent.com/62683395/155448245-1dae2e73-4038-4b24-8619-719d2da0c380.png)

## Setup

1) Create an account @ playfab.com and setup your studio / game title
2) Clone this repo to your local machine
3) Follow the steps on the [Playfab Unity SDK](https://github.com/PlayFab/UnitySDK) to setup Playfab inside of your project (PlayFab does not come with this repo)

Now you should be good to begin testing. Open the AuthenticationScene to get started. By default the listener will run when you press play, activating the other objects in the scene will allow you to test it. All you need to do to interact with your newly made authentication server is add the [Kyoshi Studios Auth Utils](https://github.com/cabbagegod/Authentication-Server/releases) to your game project. The contained classes can then be serialized/deserialized as you need them.

## How To Test

Simple. Open the AuthenticationScene and press play. Now activate the PlayfabLogin object, you should see a "Logged in!" message appear in the console shortly after. If not or if an error appeared then you likely setup your environment incorrectly. If it worked then you can activate the "RequestTester" object which will send a request to the listener and after a moment should send a message in the console verifying that the user has been authenticated.

## Explanation

### Authenticator
Used to authenticate a Session Ticket with the PlayFab API

### UnityHttpListener
The core of the project, listens on http://127.0.0.1:4444/ (default) for POST requests and responds with if the user has been authenticated or not.

### HttpRequestTest
Used to test the authentication server

### PlayFabLoginTest
A simple example script to login to a playfab title designed to test the authenticator
