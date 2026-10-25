import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { GameApiService } from './game-api.service';

describe('GameApiService', () => {
  let service: GameApiService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(GameApiService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('creates a game with the selected mode', () => {
    service.createGame('Computer').subscribe();

    const request = http.expectOne('http://localhost:5080/api/games');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({ mode: 'Computer' });
  });

  it('posts a move to the game', () => {
    service.makeMove('abc', { player: 'X', row: 2, column: 1 }).subscribe();

    const request = http.expectOne('http://localhost:5080/api/games/abc/moves');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({ player: 'X', row: 2, column: 1 });
  });

  it('calls the undo and reset endpoints', () => {
    service.undo('abc').subscribe();
    service.resetGame('abc').subscribe();

    expect(http.expectOne('http://localhost:5080/api/games/abc/undo').request.method).toBe('POST');
    expect(http.expectOne('http://localhost:5080/api/games/abc/reset').request.method).toBe('POST');
  });

  it('resets the scoreboard', () => {
    service.resetScoreboard().subscribe();

    expect(http.expectOne('http://localhost:5080/api/scoreboard/reset').request.method).toBe('POST');
  });
});
