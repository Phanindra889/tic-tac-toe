import { Component, input, output } from '@angular/core';
import { Player } from '../models/game';

@Component({
  selector: 'app-board',
  templateUrl: './board.html',
  styleUrl: './board.css',
})
export class Board {
  cells = input.required<(Player | null)[]>();
  winningCells = input<number[]>([]);
  locked = input(false); // true when the game is over or a request is in progress

  cellClicked = output<number>();

  isWinningCell(index: number): boolean {
    return this.winningCells().includes(index);
  }

  canClick(index: number): boolean {
    return !this.locked() && this.cells()[index] === null;
  }

  cellLabel(index: number): string {
    const row = Math.floor(index / 3) + 1;
    const column = (index % 3) + 1;
    const value = this.cells()[index] ?? 'empty';
    return `Row ${row}, Column ${column}: ${value}`;
  }
}
