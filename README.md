# Tic Tac Toe (Angular + .NET)

A browser-based Tic Tac Toe game. The Angular frontend talks to a .NET Web API over REST.
The backend owns all the game rules and state: the board, turns, move history, win/draw
detection, undo, the computer opponent and the scoreboard. The frontend only shows
what the backend returns.

## Tech Stack

| Part      | Technology                                   |
| --------- | -------------------------------------------- |
| Frontend  | Angular 21, TypeScript, plain CSS            |
| Backend   | .NET 10 ASP.NET Core Web API (controllers)   |
| API style | REST + JSON                                  |
| Storage   | In-memory (singleton service)                |
| Tests     | xUnit (backend), Vitest (frontend)           |

## Features

- 3 × 3 board. Empty cells are clickable, filled cells are locked.
- Two Player mode (X and O take turns on the same screen).
- Play vs Computer mode (you are X, the computer is O and replies right after your move).
- Shows whose turn it is, the winner, or a draw message.
- Winning cells are highlighted.
- Move history table (move number, player, row and column).
- Undo Last Move (removes 1 move in Two Player mode, 2 moves in Computer mode).
- Reset Game (clears the board but keeps the scoreboard).
- Scoreboard (X wins, O wins, draws) stored on the backend, with its own Reset Scoreboard button.
- Invalid moves are rejected by the backend with a clear error message.

## Project Structure

```
tic-tac-toe/
├── backend/
│   ├── TicTacToe.slnx
│   ├── TicTacToe.Api/
│   │   ├── Controllers/     GamesController, ScoreboardController
│   │   ├── Dtos/            Request and response shapes for the API
│   │   ├── Models/          Game (rules), Move, Scoreboard, WinChecker, enums, exceptions
│   │   ├── Services/        GameService (stores games), ComputerPlayer (AI)
│   │   └── Program.cs       DI, CORS, JSON setup
│   └── TicTacToe.Tests/     xUnit tests for the rules, the computer and the service
└── frontend/
    └── src/app/
        ├── app.ts / .html   Main page: holds the game state and handles button clicks
        ├── board/           The 3 × 3 grid
        ├── move-history/    Move history table
        ├── scoreboard-panel/ Scoreboard + Reset Scoreboard button
        ├── services/        GameApiService: all HTTP calls
        └── models/          TypeScript types that match the API JSON
```

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) 20.19+ (or 22.12+) and npm

## How to Run the Backend

```bash
cd backend
dotnet run --project TicTacToe.Api
```

The API runs on **http://localhost:5080**. The OpenAPI document is at
http://localhost:5080/openapi/v1.json.

## How to Run the Frontend

In a second terminal:

```bash
cd frontend
npm install
npm start
```

Open **http://localhost:4200**. The backend must be running first.

## How to Run the Tests

Backend (35 tests):

```bash
cd backend
dotnet test
```

Frontend (9 tests):

```bash
cd frontend
npm test -- --watch=false
```

### What the tests cover

| Requirement               | Test(s)                                                                  |
| ------------------------- | ------------------------------------------------------------------------ |
| Valid move                | `GameTests.ValidMove_PlacesMarkAndAddsToHistory`                          |
| Invalid move              | `MoveOutsideBoard_IsRejected`, `MoveOnOccupiedCell_...`, `MoveByWrongPlayer_IsRejected` |
| Turn switching            | `GameTests.ValidMove_SwitchesTurn`                                        |
| Row / column / diagonal win | `RowWin_IsDetected`, `ColumnWin_IsDetected`, `DiagonalWin_IsDetected`, `AntiDiagonalWin_IsDetected` |
| Draw                      | `GameTests.FullBoardWithNoWinner_IsDraw`                                  |
| Reset game                | `GameTests.Reset_ClearsEverything`, `GameServiceTests.ResetGame_KeepsScoreboard` |
| Undo (two player)         | `GameTests.Undo_InTwoPlayerMode_RemovesOnlyLastMove`                      |
| Undo (computer)           | `GameTests.Undo_InComputerMode_...`, `GameServiceTests.ComputerMode_UndoRemovesBothMoves` |
| Scoreboard update         | `Scoreboard_IsUpdatedWhenGameIsWon`, `Scoreboard_IsUpdatedOnlyOncePerGame`, `Scoreboard_CountsDraws` |
| Computer move selection   | All tests in `ComputerPlayerTests` (win, block, center, corner, any cell) |
| Move after game completion | `GameTests.MoveAfterGameIsWon_IsRejected`, `ComputerMode_ComputerDoesNotMoveAfterGameEnds` |
| Frontend rendering        | `app.spec.ts` (status text, winning highlight, undo disabled, mode switch) |
| Frontend API calls        | `game-api.service.spec.ts` (URLs, methods and request bodies)             |

## API Endpoints

Base URL: `http://localhost:5080/api`

| Method | Endpoint                    | Body                                      | Purpose                     |
| ------ | --------------------------- | ----------------------------------------- | --------------------------- |
| POST   | `/games`                    | `{ "mode": "TwoPlayer" }` or `"Computer"` | Create a new game           |
| GET    | `/games/{id}`               | –                                         | Get the current game state  |
| POST   | `/games/{id}/moves`         | `{ "player": "X", "row": 0, "column": 2 }` | Make a move                 |
| POST   | `/games/{id}/undo`          | –                                         | Undo the last move          |
| POST   | `/games/{id}/reset`         | –                                         | Reset the current game      |
| GET    | `/scoreboard`               | –                                         | Get the scoreboard          |
| POST   | `/scoreboard/reset`         | –                                         | Reset the scoreboard        |

`row` and `column` are **zero-based** (0 to 2). The UI shows them as 1 to 3.

### Game state response

Every `/games` endpoint returns the full game state:

```json
{
  "id": "6f1c0e3a-...",
  "mode": "Computer",
  "board": ["X", null, null, null, "O", null, null, null, null],
  "currentPlayer": "X",
  "status": "InProgress",
  "winner": null,
  "winningCells": [],
  "moves": [
    { "moveNumber": 1, "player": "X", "row": 0, "column": 0 },
    { "moveNumber": 2, "player": "O", "row": 1, "column": 1 }
  ],
  "canUndo": true,
  "scoreboard": { "xWins": 0, "oWins": 0, "draws": 0 }
}
```

- `board` has 9 cells, left to right, top to bottom (index = row × 3 + column).
- `status` is `InProgress`, `Won` or `Draw`.
- `winningCells` holds the 3 board indexes of the winning line.

### Errors

| Status | When                                                                 | Body                                   |
| ------ | -------------------------------------------------------------------- | -------------------------------------- |
| 400    | Outside the board, cell taken, wrong player, game over, nothing to undo | `{ "error": "That cell is already taken." }` |
| 404    | Unknown game id                                                      | `{ "error": "Game ... was not found." }` |

## Design Decisions

- **Backend is the source of truth.** The frontend never decides if a move is valid or who
  won. After every action it simply shows the state the API sent back.
- **Rules live in one class.** `Game` handles moves, undo, reset and status. It has no
  knowledge of HTTP or the scoreboard, which makes it easy to unit test.
- **Status is recalculated from the board.** After every move or undo, `UpdateStatus()` checks
  the board again, and the current player is worked out from the number of moves
  (even = X, odd = O). This means undo can't leave the game in a wrong state.
- **The computer moves on the server, in the same request.** When you play in Computer mode,
  `POST /moves` applies your move and then the computer's reply, and returns both.
  The computer never moves if your move ended the game.
- **Each game is counted once.** A finished game rejects new moves and undo, so the
  scoreboard is only updated in the request that finished the game.
- **In-memory storage.** `GameService` is a singleton with a dictionary of games and one
  scoreboard. A single `lock` keeps it safe if two requests arrive at the same time.
- **Small components.** The page is split into `Board`, `MoveHistory` and `ScoreboardPanel`.
  Only `App` talks to the API service.

## Clarifications and Assumptions

- **Scoreboard and Undo — Option A chosen:** once a game is won or drawn, Undo is disabled.
  The scoreboard result for that game is final.
- **Reset Game keeps the same game id and mode.** It clears the board, history and result, and
  sets the turn back to X. The scoreboard is not touched.
- **Switching the mode starts a new game** (new id) in the new mode.
- **In Computer mode the human is always X** and X always goes first. Sending a move as O in
  Computer mode is rejected.
- The scoreboard is **shared by all games** while the API is running (one session = one API run).
- Row and column are zero-based in the API.

## Known Limitations

- All data is lost when the API restarts (in-memory only).
- There is one global scoreboard, not one per user or browser.
- Old games are never removed from memory.
- The computer is intentionally simple (rule-based). It can be beaten.
- The API URL (`http://localhost:5080/api`) is set directly in `GameApiService`, and CORS only
  allows `http://localhost:4200`.

## Future Improvements

- Save games and scores in SQLite with Entity Framework Core.
- Per-player or per-browser scoreboards.
- A stronger computer opponent (minimax) with difficulty levels.
- Option B for undo (allow undo after a finished game and adjust the scoreboard).
- Move the API URL into Angular environment files.
- End-to-end tests (for example with Playwright) and a GitHub Actions build.
