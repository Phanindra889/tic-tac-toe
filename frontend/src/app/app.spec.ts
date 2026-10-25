import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { App } from './app';
import { Game } from './models/game';
import { GameApiService } from './services/game-api.service';

function makeGame(changes: Partial<Game> = {}): Game {
  return {
    id: 'game-1',
    mode: 'TwoPlayer',
    board: [null, null, null, null, null, null, null, null, null],
    currentPlayer: 'X',
    status: 'InProgress',
    winner: null,
    winningCells: [],
    moves: [],
    canUndo: false,
    scoreboard: { xWins: 0, oWins: 0, draws: 0 },
    ...changes,
  };
}

describe('App', () => {
  let api: {
    createGame: ReturnType<typeof vi.fn>;
    makeMove: ReturnType<typeof vi.fn>;
    undo: ReturnType<typeof vi.fn>;
    resetGame: ReturnType<typeof vi.fn>;
    resetScoreboard: ReturnType<typeof vi.fn>;
  };

  beforeEach(async () => {
    api = {
      createGame: vi.fn().mockReturnValue(of(makeGame())),
      makeMove: vi.fn(),
      undo: vi.fn(),
      resetGame: vi.fn(),
      resetScoreboard: vi.fn(),
    };

    await TestBed.configureTestingModule({
      imports: [App],
      providers: [{ provide: GameApiService, useValue: api }],
    }).compileComponents();
  });

  function render() {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    return fixture;
  }

  it('creates a game on start and shows whose turn it is', () => {
    const fixture = render();
    const page = fixture.nativeElement as HTMLElement;

    expect(api.createGame).toHaveBeenCalledWith('TwoPlayer');
    expect(page.querySelector('[data-testid="status"]')?.textContent).toContain("Player X's turn");
    expect(page.querySelectorAll('.cell').length).toBe(9);
  });

  it('sends a move when an empty cell is clicked', () => {
    api.makeMove.mockReturnValue(of(makeGame({ board: ['X', null, null, null, null, null, null, null, null] })));
    const fixture = render();
    const cells = (fixture.nativeElement as HTMLElement).querySelectorAll<HTMLButtonElement>('.cell');

    cells[4].click();

    expect(api.makeMove).toHaveBeenCalledWith('game-1', { player: 'X', row: 1, column: 1 });
  });

  it('shows the winner and highlights the winning cells', () => {
    api.createGame.mockReturnValue(
      of(
        makeGame({
          board: ['X', 'X', 'X', 'O', 'O', null, null, null, null],
          status: 'Won',
          winner: 'X',
          winningCells: [0, 1, 2],
          scoreboard: { xWins: 1, oWins: 0, draws: 0 },
        }),
      ),
    );

    const fixture = render();
    const page = fixture.nativeElement as HTMLElement;

    expect(page.querySelector('[data-testid="status"]')?.textContent).toContain('Player X wins!');
    expect(page.querySelectorAll('.cell.winning').length).toBe(3);
    expect(page.querySelector('[data-testid="x-wins"]')?.textContent).toContain('1');
  });

  it('disables Undo when there is nothing to undo', () => {
    const fixture = render();
    const undoButton = Array.from((fixture.nativeElement as HTMLElement).querySelectorAll('button')).find(
      (button) => button.textContent?.includes('Undo'),
    );

    expect(undoButton?.disabled).toBe(true);
  });

  it('starts a new computer game when the mode is changed', () => {
    const fixture = render();
    const computerButton = Array.from((fixture.nativeElement as HTMLElement).querySelectorAll('button')).find(
      (button) => button.textContent?.includes('Computer'),
    );

    computerButton?.click();

    expect(api.createGame).toHaveBeenLastCalledWith('Computer');
  });
});
