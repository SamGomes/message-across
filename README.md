# Message Across

<img src="./ReadmeImages/logo.png" width="300">

Message Across is a simple multiplayer game which focuses on word matching.
In the course of the game, the players try to complete words as they advance through the levels. Each level presents two words on the top of the screen, one for each player.
The words can be completed by selecting letters on a track positioned at the center.
The track contains three lanes where the letters move.
In order to select a letter, a player has to position his/her marker to the lane where the letter is rolling, and select an action.
When the letter collides with the marker, the selected action is performed.
If two players are in the same track, only the first player that selects an action is able to perform it.

The players can perform one of two possible actions at each moment.
They can either *take the letter* or *give the letter* to the other player.
The objective of each player is to *obtain the highest score*.

The game was developed exclusively for touch screens, and players interact with it through virtual buttons positioned in each bottom side of the screen.
The current scores and number of available actions are displayed in the game interface, above the players' buttons and below the players' words.
The words presented to players, as well as the score system of the game is fully parameterizable. In particular, the score attributed to a player after performing an action depends on the action itself (*give* or *take*), whether the letter is useful for the player who performed the action or the other player, and whether the player who performed the action had lower, equal or higher score than the other player. Other aspects of the game, such as the players' colors or number of possible actions per level can also be changed dynamically.


## Executing the game

To execute the game (in Windows), only two steps are required:
- Download the latest version of Message Across through git pages of this repository: https://samgomes.github.io/message-across/
- Extract the folder and click on the executable (messageAcross.exe)

For other platforms, the code can be also downloaded and re-built.

## Additional material
This program is part of a research project, and its iterations will be used as supplementary material for the several submissions.
As such, the R material which was used to perform statistical analysis in the game logs is also included.

## Game designed and developed by:
- Samuel Gomes - [@SamGomes](https://github.com/SamGomes)
- Tomás Alves - [@SirMastermind](https://github.com/SirMastermind)
