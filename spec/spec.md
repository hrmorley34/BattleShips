1. Program should show the user a menu
    * Print out a menu
        * Use colour?
    * Option to play new game
    * Option to resume game
        * Grey out if no game available to resume
    * Option to read instructions
        * Use colour?
        * Use pages of information to switch between?
    * Option to quit
    * Use arrows to switch between options?
2. (option play new game ->) Show blank grid
    * Use standard grid subroutine
    a. Prompt player for boat coordinates
        * Use arrow keys to move (ext: and 'r' to rotate) the boat around the grid
        * Display boat preview
        * Use enter to confirm location of boat
        * Use backspace to go back to the previous boat?
    b. Check for existing boat in that position
        * Display boat in red on preview
    c. Display each boat on grid after entry
    d. Only allow the player to enter five boat locations
        * Have an array of 5 boats
3. (->) The computer randomly selects boat locations
    * Randomly generate coordinates
        * If coordinates are already occupied, re-generate coordinates
    * Store in hidden array (not displayed to user)
4. Display blank target tracker
    * Use standart target tracker grid subroutine
    * May use same 2D array for target tracker and boat, but display different data
5. Player takes their turn
    a. Show user the grid
        * In colour
    b. Prompt for target coordinates
        * Use Console.ReadKey to restrict what can be entered
        * Turn coordinates red if they have been hit before, and don't allow shooting then
        * Press enter to shoot
    c. Check for hit or miss
        * Show "H" for hit
            * Colour in green
        * Show "M" for miss
            * Colour in blue for water
    d. Check for win (7.)
    e. (ext: Save at end of turn (8.))
6. Computer takes its turn
    a. Randomly generate target coordinates
        * If they have already been shot, re-generate
    b. Display attack coordinates to the player
    c. Check for hit or miss
        * Show "H" for hit (over "B" of boat) on grid
            * Colour in red
        * Show "M" for miss
            * Colour in dark grey, so it isn't too obvious (not necessary for the user to know)
    d. Check for win (7.)
    e. (ext: Save at end of turn (8.))
7. Keep playing until there is a winner
    * Check if all boats have been hit
    * Display winner to player

8. Save the game
    * At end of each turn
    * Save the state of all grids to a file
    * Save current player to a file
9. (from menu ->) Resume game
    * Read data from file
    * Get current player turn
        * Go to either 5. or 6.

10. TODO
11. TODO
