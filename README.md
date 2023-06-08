![Unity 2021.3.5f1 badge](https://img.shields.io/badge/Unity-2021.3.5f1-blue)
![Elympics 0.7.0 badge](https://img.shields.io/badge/Elympics-0.7.0-white)

![Elympics](Resources/images/logo-light.png#gh-dark-mode-only)
![Elympics](Resources/images/logo-dark.png#gh-light-mode-only)

# Open-source endless runner template by Elympics

https://github.com/Elympics/template-run/assets/1230922/073b09aa-96f1-4586-97cc-f02a6ce885ef

This repository is a *free* endless runner template for Unity game developers that want to build their first secure and cheat-proof competitive game with leaderboards. Security is ensured by constant supervision of the server, which makes game results reliable and trustworthy. The template is designed as a sample mobile game but feel free to adapt it to your needs and other platforms. You can access its source code here and use it as a basis for your own competitive game to further build on it. Note that integration with Elympics and leaderboards is universal and can be used in any other type of game.

This template is meant to be a learning resource for the new users of Elympics, our standard industry framework for blockchain-integrated multiplayer games. It’ll help you understand how it works and how certain features could be implemented.

![Template Run Screenshot 0](https://static.elympics.cc/screenshots/templaterun-0.png)
![Template Run Screenshot 1](https://static.elympics.cc/screenshots/templaterun-1.png)
![Template Run Screenshot 2](https://static.elympics.cc/screenshots/templaterun-2.png)
![Template Run Screenshot 3](https://static.elympics.cc/screenshots/templaterun-3.png)

## How to use it?

- Clone repository
- Launch this project in Unity (version 2021.3.5f1 is recommended)
- To see it in action, run the game starting from the *MainMenu* scene using *Play* button
- To start building on it and see your changes, use [half remote](https://docs.elympics.cc/getting-started/run-locally/#half-remote-mode) and start from *GameplayScene* on both original Unity instance and a clone
- If you want to be able to upload your own builds register your own game in [Elympics panel](https://panel.elympics.cc/login) and change *GameConfig* in *Tools/Elympics/ManageGamesInElympics* in Unity

## Features

- Live time leaderboards
- Full game loop from initializing the game, ensuring player connection, core gameplay and finalization
- Synchronized randomization and map generation
- Jumping
- Simple pickups
