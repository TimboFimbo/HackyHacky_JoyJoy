Hacky Hacky Joy Joy
===================

Introduction
------------

Welcome to Hacky Hacky Joy Joy, a puzzle game about reverse engineering
and software exploitation. This demo was created as part of an MSc in
Cyber Security and Digital Forensics. The goal is to interact with
simplified assembly code and exploit vulnerabilities to open doors and
exit each level.

It was designed to demonstrate the viability of simplifying complex
cyber security concepts into a puzzle game format, suitable for those
with no prior knowledge of the topics. It is not meant to showcase my
programming skills, artistic skills, or puzzle design skills. If I carry
on with the game, these things will all be improved.

Controls
--------

HHJJ has only a few controls, which are as follows:

-   Cursor Keys: Move character

-   Letter i: Interact with terminals and start oLang running

-   Space: Play / Pause running oLang (can still move character)

-   Letter r: Reset level

-   Esc Key: Show / hide help menu

Debug controls also exist, but using them will allow you to bypass the
puzzles and finish the levels:

-   Numbers 1 and 2: Open level doors 1 and 2

-   Numbers 3 and 4: Close level doors 1 and 2

Gameplay
--------

Elements of the play screen will now be explained. Further details are
covered later:

[!ScreenShot](/Readme_Images/HHJJ_1_Playfield.png)

1.  The level that the player must solve. Within it are the terminal,
    which starts the oLang code running, a door, which must be opened,
    and the exit, which must be reached to end the level.

2.  The player character. The player can move around the grid-based
    levels and interact with the terminals and other objects.

3.  The input box. This box only appears when interacting with a
    terminal and input is required from the player.

4.  The output box. This is always on-screen, and informs the player
    when input is required, as well as outputting game messages, such as
    code errors.

5.  The Vars Grid. This contains variables that will be accessed by the
    oLang code. The elements within are mutable, as inputs from the
    player are stored here and existing values can be overwritten.

6.  The Code Grid. This contains the oLang code for the level, which
    starts running when a terminal is interacted with. The contents are
    immutable, as there is no way for the player to change them.

7.  The Stack Grid. This is used whenever input is required from the
    player, and mimics the structure of a real stack, with function
    arguments on the bottom, a return address above, and space for
    input above that. In this case, the player input (123456) is on
    top, the address to store it is on the bottom (\$1030), and the
    return address in the middle (\$2020). The values are mutable and
    can overflow into one another, which is required for some of the
    puzzles. Further explanation below.

oLang
-----

The assembly-style code is referred to as oLang, and contains only a few
commands and functions, with the possibility of adding more in a later
release. While the player cannot write the code themself, it is still
important to know how it works. Here\'s the basics (play around to
understand them better):

#### PRT

This is the Print command. It accepts one or two memory addresses and
prints the content within them to the Output Box. For example, if memory
address \$1020 contains the string \'Door Opened\', then the oLang line
\'PRT\$1020\' would print it.

#### INP

This is the Input command. It accepts a memory address and a string
length. When triggered, the Input Box will open for the player and they
will be prompted to enter something, such as name or password. This will
then be stored at the memory address listed, up to the set number of
characters. For example, the oLang line \'INP\$1030,16\' will accept
player input and store up to 16 characters of it at memory address
\$1030.

However, before being stored, it will first be input onto the Stack
Grid, which does not limit the size of input strings (so the
16-character limit set above doesn\'t apply here) Check the Gameplay
section for more details.

#### RUN

This is the Run command, and executes a pre-written function, either
directly by name (as in the image above), or by memory address. For
example, the line \'RUNopen\_door,1\' will open door number 1 on the
level. If the string \'open\_door,1\' was instead stored at memory
address \$1010, then the line \'RUN\$1010\' would do the same thing. The
functions are as follows:

-   open\_door,X : This command accepts a number and opens that door on
    the level. If it is passed a zero, it will instead open any door
    that the player is standing next to. Note that there is also a
    \'close\_door\' function, but it is currently not used.

-   gen\_code,X : This command generates a random code of length X and
    stores it at memory address \$1000.

-   check\_code,0 : This command checks the strings at memory addresses
    \$1000 and \$1010 and opens the level\'s door if they match. Note
    that this function always gets passed a zero, which does nothing,
    but all functions need an argument (remember that).

-   null\_func,0: This function does nothing. Does it need replacing?..

#### END

This is the End command and terminates the running program. However, it
doesn\'t reset anything, so interacting with the level\'s terminal will
restart it.

The Stack
---------

The stack is briefly mentioned above, but as it is such an important
aspect of gameplay, here\'s a slightly more expanded explanation:

[!ScreenShot](/Readme_Images/HHJJ_1_Stack.png)

1\. This line (\$3000) contains the string input by the player whenever
the \'INP\' command is triggered. It can hold up to 16 characters, with
any extra overflowing into line \$3010 below. Overflowing values is
important for solving puzzles.

2\. This line (\$3020) contains the address where the input string in
line \$3000 will be stored.

3\. This line (\$3010) contains the return address, which is where the
flow of code will return to after the input string has been stored. As
with line \$3000, any characters beyond 16 will overflow into line
\$3020, which is also important in puzzle solving.

So, in this instance, the string \'Timbo\' will be stored in the Vars
Grid, at memory address \$1030. After this, the flow will return to the
Code Grid, at line \$2020.

Error Messages
--------------

If you input something incorrectly and the oLang cannot run, you will
receive an error message in the Output Box, after which you can try
again. All attempts have been made to catch errors and prevent the game
from crashing, but some may have slipped through, which will require a
game reset. Sorry.

The error messages are as follows:

ERR\_NO\_PRINT = Nothing to Print

ERR\_NO\_ADDRESS = No Valid Function Address

ERR\_NO\_RUN = No Function to Run

ERR\_NO\_FUNC = No Valid Function Name or Argument

ERR\_RET\_LINE = Return Line Error (ensure address starts with \'\$\')

ERR\_ARGS\_LINE = Args Line Error (ensure address starts with \'\$\')

ERR\_STACK\_PARSE = Stack Parsing Error (general error - check all stack
lines)

Level Hints
-----------

Here are some hints to help you solve the levels:

1\. What is being generated in address \$1000?..

2\. There\'s an \'open\_door\' function, but it can\'t be reached. Or
can it?\...

3\. That \'null\_func\' doesn\'t do anything. If only it opened a door
instead\...

4\. Don\'t forget you can still move when the oLang is paused\...

5\. Have you considered using multiple exploits?.. (don\'t forget -
\'r\' to restart the level)

FAQ
---

Q. What\'s up with the name?

A. It was just a placeholder name that never got replaced. It doesn\'t
have any deeper meaning than that.

Q. Have you ever coded before?

A. Yes, but I\'m certainly not an expert. I\'ll admit that this is even
more messy than usual, but getting things running was the priority, not
code optimization, which can come later.

Q. Why do the textures all look like Minecraft?

A. Because the textures *are* from Minecraft. I needed some square
textures, and this was the first game that came to mind. Graphics will
be replaced in a later release.

Q. What\'s the story of the game?

A. I have some good ideas but implementing them wasn\'t a priority for
this release. Once more levels are finished, the narrative can start to
be added.

Known Issues / Needed Updates
-----------------------------

Here are some known issues or aspects that need updating. If I have
time, I\'ll work on them:

-   There are currently no sound effects

-   The Vars Grid does not highlight when being read

-   Placeholder graphics need changing

-   The return address can only jump to a line after the \'INP\' command, not before

-   An interactive tutorial is not included yet, just a help screen

-   Input addresses won\'t parse if they contain letters, even though they\'re in hex

-   Keystroke count to be added, for leaderboards

-   Input \'code\' should be changed to \'password\', to avoid confusion with oLang code

Credits
-------

Created by me - <https://github.com/TimboFimbo>

Textures by Faithful Pack - <https://www.faithfulpack.net/>

Cat sprite by skristi - <https://skristi.itch.io/>

Walkthrough
-----------

The following is a walkthrough of the first level. This will spoil it,
but is handy if you want an explanation of each step:

##### Step 1

[!ScreenShot](/Readme_Images/HHJJ_Step1.png)

The first line run is \$2000, and contains the following code:

RUNgen\_code,4

This line contains a few elements. The first is the command \'RUN\',
which will run a pre-written function or point to a memory address that
contains one. In this case, the function is \'gen\_code\', which
generates a random code and stores it at address \$1000. Every function
requires an argument, which is the number \'4\' after the comma,
resulting in a four-number generated code. The number \'3520\' at
address \$1000 is this generated code.

##### Step 2

[!ScreenShot](/Readme_Images/HHJJ_Step2.png)

The next line run is \$2020 and contains the following code:

PRT\$1020

This line starts with the command \'PRT\', which accepts a memory
address and prints whatever string is stored there. In this case, it is
pointing at address \$1020, which contains the string \'Enter Code\'.
This is then printed in the Output Box under the playfield.

##### Step 3

[!ScreenShot](/Readme_Images/HHJJ_Step3.png)

The next line run is \$2020 and contains the following code:

INP\$1010,16

This line starts with the command \'INP\', which accepts a memory
address, prompts the player to input a string (The Input Box beneath the
playfield) and stores it at the provided address. The argument \'16\'
means up to 16 characters will be set aside for the input. In this
instance, the player is inputting the incorrect code \'123456\', which
will be stored at address \$1010 in the next step.

##### Step 3.5

[!ScreenShot](/Readme_Images/HHJJ_Step3_5.png)

Once the player has entered their code attempt, the Stack grid comes
into play. The Stack grid is structured in the same way as a real stack
frame: The bottom of the grid contains the argument passed into the INP
function (\$1010), which is the location to store the input string. The
line above contains the return address (\$2030), which is the line of
code that will be run once the input is complete. The top of the Stack
grid is for the local parameters (123456), which contains the player
input from Step 3.

Once complete, the player input will be stored at address \$1010 then
the program will return to line \$2030.

[!ScreenShot](/Readme_Images/HHJJ_Step3_5_2.png)

##### Step 4

[!ScreenShot](/Readme_Images/HHJJ_Step4.png)

The next line run is \$2030 and contains the following code:

PRT\$1030

This line also contains the \'PRT\' command, which prints the string
stored at address \$1030. In this case, the string is \'Checking
Code\...\' and is displayed in the Output Box.

##### Step 5

[!ScreenShot](/Readme_Images/HHJJ_Step5.png)

The next line run is \$2040 and contains the following code:

RUNcheck\_code,0

As with line \$2000, the \'RUN\' command runs a pre-written function,
which is the \'check\_code\' function. This checks the strings stored at
memory addresses \$1000 and \$1010 and opens the level door if they
match. The argument (the number \'0\') passed in is not used but is
there because all functions require an argument. In this case, the
numbers do not match, so the player is informed, and the door does not
open.

The final line is \$2050, which contains the command \'END\'. This ends
the program, allowing the player to try again. The following step shows
the result of a correct code being entered.

##### Step 6

[!ScreenShot](/Readme_Images/HHJJ_Step6.png)

In this instance, the code \'0730\' has been generated in address
\$1000. Address \$1010 contains the player input, and since they match,
the \'check\_code\' function has opened the door, allowing the player to
reach the exit and complete the level.

Check the Level Hints section for help with the following levels.
