// These types match the JSON returned by the .NET API.

export type Player = 'X' | 'O';
export type GameMode = 'TwoPlayer' | 'Computer';
export type GameStatus = 'InProgress' | 'Won' | 'Draw';

export interface Move {
  moveNumber: number;
  player: Player;
  row: number; // 0 to 2
  column: number; // 0 to 2
}

export interface Scoreboard {
  xWins: number;
  oWins: number;
  draws: number;
}

export interface Game {
  id: string;
  mode: GameMode;
  board: (Player | null)[]; // 9 cells, left to right, top to bottom
  currentPlayer: Player;
  status: GameStatus;
  winner: Player | null;
  winningCells: number[];
  moves: Move[];
  canUndo: boolean;
  scoreboard: Scoreboard;
}

export interface MoveRequest {
  player: Player;
  row: number;
  column: number;
}
