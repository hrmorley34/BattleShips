* `Game`
    * `Players`: `IPlayer[]`
    * `Boards`: `Board[]`
* Interface `IPlayer`
    * `void SetBoards(Board boatsBoard, Board enemyBoard)`
    * `void PlaceBoats()`
    * `Boat[] GetBoats()`
    * `Coordinates AskShoot()`
    * `bool HasWon()`: `GetBoats().All(b => b.Sunk)`
* `ComputerPlayer`: `IPlayer`
* `HumanPlayer`: `IPlayer`
* `Board`
    * `Elements`: `IBoardElement[,]`
* Flag enum `DisplayEnum`
    ```cs
    [Flags]
    enum DisplayEnum
    {
        None = 0,
        Shot = 1,
        Boat = 2,
        mHit = 3,
        Sunk = 4,
        mSunk = 7,
    }
    ```
* `IBoardElement`
    * `bool Shoot()`
        * true for hit a boat, false for not
    * `bool HasShot()`
    * `DisplayEnum GetDisplay()`
* `EmptyBoardElement`: `IBoardElement`
    * `Shot`: `bool`
    * `bool Shoot()` sets `Shot` always returns `false`
    * `DisplayEnum GetDisplay()`
        * if `Shot`, `DisplayEnum.Fired`
        * else `DisplayEnum.None`
* `BoatElement`: `IBoardElement`
    * `Shot`: `bool`
    * `bool Shoot()` sets `Shot`
    * `Boat`: `Boat`
    * `DisplayEnum GetDisplay()`
        * `DisplayEnum.Boat`
        * if `Shot`, `| DisplayEnum.Fired`
        * if `Boat.IsSunk()`, `| DisplayEnum.Sunk`
* `Boat`
    * `BoatElements`: `BoatElement[]`
    * `bool IsSunk()`: `BoatElements.All(b => b.Shot)`
* `Coordinates`
    * `operator +`
    * `Deconstruct(out int x, out int y)`
