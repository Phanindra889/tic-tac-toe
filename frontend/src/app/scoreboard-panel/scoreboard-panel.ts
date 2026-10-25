import { Component, input, output } from '@angular/core';
import { Scoreboard } from '../models/game';

@Component({
  selector: 'app-scoreboard-panel',
  templateUrl: './scoreboard-panel.html',
  styleUrl: './scoreboard-panel.css',
})
export class ScoreboardPanel {
  scoreboard = input.required<Scoreboard>();
  disabled = input(false);

  resetClicked = output<void>();
}
