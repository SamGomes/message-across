# Message Across

<img src="./ReadmeImages/logo.png" width="300">

Message Across is a simple multiplayer game which focuses on word matching.
In the course of the game, the players try to complete words as they advance through the levels. Each level presents two words on the top of the screen, one for each player.
The words can be completed by selecting letters on a track positioned at the center.
The track contains three lanes where the letters move.
In order to select a letter, a player has to position their marker to the lane where the letter is coming, and select an action.
When the letter collides with the marker, the selected action is performed.
If two players are in the same track, only the first player that selects an action is able to perform it.

The players can perform one of two possible actions at each moment.
They can either *take the letter* or *give the letter* to the other player.
The objective of each player is to *obtain the highest score*.

The game was developed exclusively for touch screens, and players interact with it through virtual buttons positioned in each bottom side of the screen.
The current scores and number of available actions are displayed in the game interface, above the players' buttons and below the players' words.
The words presented to players, as well as the score system of the game is fully parameterizable. In particular, the score attributed to a player after performing an action depends on the action itself (*give* or *take*), whether the letter is useful for the player who performed the action or the other player, and whether the player who performed the action had lower, equal or higher score than the other player. Other aspects of the game, such as the players' colors or number of possible actions per level can also be changed dynamically.

*Note: This repository was originally created for a modified version of an earlier game called CrossAnt, as the project structure served as a base for the development of Message Across. Indeed, the development of such version is still present in the log of the project. However, all of the code was redone and the Assets rebuilt, which, in reality, eliminated the relation between the two games. The repository of CrossAnt is included [here](https://github.com/SamGomes/interaction-mechanics-cross-ant).*

## Executing the game
To execute the game (in Windows), only two steps are required:
- Download the latest version of Message Across through the [GitHub Pages](https://samgomes.github.io/message-across/) of this repository, or through [itch.io](https://samgomes.itch.io/message-across/);
- Extract the folder and click on the executable (messageAcross.exe).

For other platforms, the code can be also downloaded and re-built.

## Message Across for research
This game was used in the following research works:

- [Gomes S., Alves T., Dias J., Martinho C. (2022) Message Across: A word matching game for reward-based in-game behavior change. In: Videogame Sciences and Arts. VJ 2020.](http://videojogos2020.ipb.pt/docs/ProceedingsVJ2020.pdf)

- [Gomes S., Alves T., Dias J., Martinho C. (2022) Reward-Mediated Individual and Altruistic Behavior. In: Videogame Sciences and Arts. VJ 2020. Communications in Computer and Information Science, vol 1531. Springer, Cham.](https://doi.org/10.1007/978-3-030-95305-8_7) 

- [T. Alves, S. Gomes, J. Dias, and C. Martinho. In: IEEE Conference on Games (IEEE CoG 2020). The Influence of Reward on the Social Valence of Interactions.](https://ieee-cog.org/2020/papers/paper_61.pdf)

Driven by its use in research, some [R](https://www.r-project.org/) and [SPSS](https://www.ibm.com/products/spss-statistics) material applied to perform statistical analyses using the game logs is also included, in the `messageAcrossScripts` directory.


## License

### Game designed and developed by:
- Samuel Gomes - [@SamGomes](https://github.com/SamGomes)
- Tomás Alves - [@SirMastermind](https://github.com/SirMastermind)
  
The current and previous versions of the code are licensed according to Attribution 4.0 International (CC BY 4.0).  
 
 <a rel="license" href="http://creativecommons.org/licenses/by/4.0/"><img alt="Creative Commons License" style="border-width:0" src="https://i.creativecommons.org/l/by/4.0/88x31.png" /></a><br />
