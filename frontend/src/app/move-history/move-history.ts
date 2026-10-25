import { Component, input } from '@angular/core';
import { Move } from '../models/game';

@Component({
  selector: 'app-move-history',
  templateUrl: './move-history.html',
  styleUrl: './move-history.css',
})
export class MoveHistory {
  moves = input.required<Move[]>();
}
