# ConsoleMineSweeper

This program is something I wanted to do for a while. I quickly put it together in less than 24 hours, and spent some time here and there in the 
following weeks implementing more stuff and fixing bugs. I decided to go with the standard CS library and limiting myself to the console instead
of making something fancy with 3rd party libraries like SFML. Also, I decided to program the entire thing in a functionnal paradigm instead of using 
object oriented programming, which I reconsidered multiple times during the project.

The design is quite interesting. It uses a multidimensional 2D array of characters for the display, that keeps the information on what is going to 
be displayed on the screen without recalculating it everytime. it has the dimensions of the game grid + 2 vertically and horizontally, allowing it
to display cursors on each side indicating which element is selected. There is 2 other boolean arrays of the same dimensions, one for the minefield,
which serves to know where the mines are located, and another tells which locations have already been visited (useful for recursive uncovering 
algorithm and for tracking the win condition). Why are they the same size as the display grid? Because it allows for easy conversion of the position
index, and, it also circumvent the problem of going out of boundaries of the arrays to check surrounding tiles. This way, instead of preventing the
check, it simply allows it. Mines are never generated on the margin around the game, and those tiles are marked as already visited by default too.

One of the features of the game is that the first tile uncovered will always be an empty tile with no surrounding mines. This is done by removing the
mines on the first tile uncovered and all the 8 surrounding tiles, and redistributing them randomly on the rest of the minefield. This is done to make
the game easier, as at minimum 9 tiles will always be uncovered on the first turn.

The most limiting aspect of the project was definitely the console, which is mostly because I don't know that much about it. If I had to redo this
project, I would do it differently, mainly because after finishing this, I am a lot more knowledgeable of the console than I was. I would probably make
it so that instead of cursors, the selected tile would be displayed with different console background color. I would also display the game in a more optimised
manner, regenerating only lines where the display has changed. Performance is a big aspect im not fond of in this app, but I noticed it is mostly due to
the Windows console, which has an abysmal refresh rate (it might just be that other consoles are more optimised). The addition of the colored numbers feature
affected the performance of the code too, but I think it adds a lot of flavor, so I decided to keep that feature in the project.

Have fun playing it, and feel free to reuse or modify this project to your heart content!
