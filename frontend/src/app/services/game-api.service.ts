import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Game, GameMode, MoveRequest, Scoreboard } from '../models/game';

// All calls to the backend live here, so components don't deal with URLs.
@Injectable({ providedIn: 'root' })
export class GameApiService {
  private http = inject(HttpClient);
  private baseUrl = 'http://localhost:5080/api';

  createGame(mode: GameMode): Observable<Game> {
    return this.http.post<Game>(`${this.baseUrl}/games`, { mode });
  }

  getGame(gameId: string): Observable<Game> {
    return this.http.get<Game>(`${this.baseUrl}/games/${gameId}`);
  }

  makeMove(gameId: string, move: MoveRequest): Observable<Game> {
    return this.http.post<Game>(`${this.baseUrl}/games/${gameId}/moves`, move);
  }

  undo(gameId: string): Observable<Game> {
    return this.http.post<Game>(`${this.baseUrl}/games/${gameId}/undo`, {});
  }

  resetGame(gameId: string): Observable<Game> {
    return this.http.post<Game>(`${this.baseUrl}/games/${gameId}/reset`, {});
  }

  getScoreboard(): Observable<Scoreboard> {
    return this.http.get<Scoreboard>(`${this.baseUrl}/scoreboard`);
  }

  resetScoreboard(): Observable<Scoreboard> {
    return this.http.post<Scoreboard>(`${this.baseUrl}/scoreboard/reset`, {});
  }
}
