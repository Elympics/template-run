![Unity 2021.3.33f1 badge](https://img.shields.io/badge/Unity-2021.3.33f1-blue)
![Elympics 0.15.3 badge](https://img.shields.io/badge/Elympics-0.15.3-white)

<picture>
  <source media="(prefers-color-scheme: dark)" srcset="https://static.elympics.cc/assets/logo/elympics-light.png">
  <img alt="Elympics" src="https://static.elympics.cc/assets/logo/elympics-dark.png">
</picture>


# Open-source endless runner template by Elympics

https://github.com/Elympics/template-run/assets/1230922/073b09aa-96f1-4586-97cc-f02a6ce885ef

This repository is a *free* endless runner template for Unity game developers that want to build their first secure and cheat-proof competitive game with leaderboards. Security is ensured by constant supervision of the server, which makes game results reliable and trustworthy. The template is designed as a sample mobile game but feel free to adapt it to your needs and other platforms. You can access its source code here and use it as a basis for your own competitive game to further build on it. Note that integration with Elympics and leaderboards is universal and can be used in any other type of game.

This template is meant to be a learning resource for the new users of Elympics, our standard industry framework for blockchain-integrated multiplayer games. It’ll help you understand how it works and how certain features could be implemented.

![Template Run Screenshot 0](https://static.elympics.cc/screenshots/templaterun-0.png)
![Template Run Screenshot 1](https://static.elympics.cc/screenshots/templaterun-1.png)
![Template Run Screenshot 2](https://static.elympics.cc/screenshots/templaterun-2.png)
![Template Run Screenshot 3](https://static.elympics.cc/screenshots/templaterun-3.png)

## How to use it?

* Launch this project in Unity (version 2021.3.33f1 is recommended).
* To see it in action, run **unmodified** template starting from the *MainMenu* scene using the *Play* button.
* To start building on the template, switch to [Half Remote development mode](https://docs.elympics.cc/getting-started/run-locally/#half-remote-mode). Create a single clone and open *GameplayScene* on both the original and cloned Unity instance. You can then test your changes by entering play mode on both at the same time.[^1]
* :warning: Note that both the *Play* button in the *MainMenu* scene will try to connect to the server build uploaded by us, which doesn't have your changes applied. It may result in **different gameplay behaviour than expected**.
* If you want to **build and release your changes**, you have to register your own game using [Elympics console](https://console.elympics.ai/login) and then update existing *GameConfig* available by choosing *Tools -> Elympics -> Manage games in Elympics* from top bar menu in Unity. See our [tutorial](https://docs.elympics.ai/gameplay/add-elympics) for more details.
* :warning: For your online build to **work properly** you also have to configure additional settings using [Elympics console](https://console.elympics.ai/login) or our [command-line client (CLI)](https://docs.elympics.ai/deploy/advanced/cli/introduction):
  * Add queue named "Solo" for one player only. Otherwise your client build **won't connect** to your server build.
* :warning: Finally, don't forget to [upload](https://docs.elympics.ai/gameplay/run-online) your server build. You have to reupload your server build every time you change version number in your client build.
* You can now test your game using *MainMenu* with *Play* button working correctly :tada: 


[^1]: Optionally, you can use editor-only *half remote* buttons in the *MainMenu* instead - *host* on the original editor instance and *play* on the clone.

## Features

- Integrated `AsyncGameSample`
- Live time leaderboards & tournaments integration
- Full game loop from initializing the game and ensuring player connection, through core gameplay, to the finalization and saving score
- Synchronized randomization and map generation
- Refined jumping
- Simple pickups
- Animations, visual and sound effects synchronization
