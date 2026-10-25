import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Board } from './board/board';
import { MoveHistory } from './move-history/move-history';
import { ScoreboardPanel } from './scoreboard-panel/scoreboard-panel';
import { Game, GameMode, Scoreboard } from './models/game';
import { GameApiService } from './services/game-api.service';

@Component({
  selector: 'app-root',
  imports: [Board, MoveHistory, ScoreboardPanel],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App implements OnInit {
  private api = inject(GameApiService);

  // The backend is the source of truth. We only keep the latest copy it sent us.
  game = signal<Game | null>(null);
  scoreboard = signal<Scoreboard>({ xWins: 0, oWins: 0, draws: 0 });
  mode = signal<GameMode>('TwoPlayer');
  errorMessage = signal('');
  loading = signal(false);

  statusMessage = computed(() => {
    const game = this.game();
    if (!game) {
      return 'Starting game...';
    }

    if (game.status === 'Won') {
      return `Player ${game.winner} wins!`;
    }

    if (game.status === 'Draw') {
      return "It's a draw!";
    }

    return `Player ${game.currentPlayer}'s turn`;
  });

  boardLocked = computed(() => this.loading() || this.game()?.status !== 'InProgress');

  ngOnInit(): void {
    this.startNewGame();
  }

  changeMode(mode: GameMode): void {
    if (mode === this.mode()) {
      return;
    }

    this.mode.set(mode);
    this.startNewGame();
  }

  startNewGame(): void {
    this.sendGameRequest(this.api.createGame(this.mode()));
  }

  playCell(index: number): void {
    const game = this.game();
    if (!game) {
      return;
    }

    const row = Math.floor(index / 3);
    const column = index % 3;
    this.sendGameRequest(this.api.makeMove(game.id, { player: game.currentPlayer, row, column }));
  }

  undo(): void {
    const game = this.game();
    if (game) {
      this.sendGameRequest(this.api.undo(game.id));
    }
  }

  resetGame(): void {
    const game = this.game();
    if (game) {
      this.sendGameRequest(this.api.resetGame(game.id));
    }
  }

  resetScoreboard(): void {
    this.errorMessage.set('');
    this.api.resetScoreboard().subscribe({
      next: (scoreboard) => this.scoreboard.set(scoreboard),
      error: (error) => this.showError(error),
    });
  }

  // Every game action returns the full game state, so they all share this handler.
  private sendGameRequest(request: Observable<Game>): void {
    this.errorMessage.set('');
    this.loading.set(true);

    request.subscribe({
      next: (game) => {
        this.game.set(game);
        this.scoreboard.set(game.scoreboard);
        this.loading.set(false);
      },
      error: (error) => {
        this.showError(error);
        this.loading.set(false);
      },
    });
  }

  private showError(error: HttpErrorResponse): void {
    if (error.status === 0) {
      this.errorMessage.set('Cannot reach the server. Is the backend running on port 5080?');
    } else {
      // The API sends errors as { "error": "message" }
      this.errorMessage.set(error.error?.error ?? 'Something went wrong. Please try again.');
    }
  }
}
