import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { GamesDto, GameGenre, GameStatus } from '../../../Dtos/GamesDto';

@Component({
  selector: 'app-games-list',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule],
  templateUrl: './games-list.html',
  styleUrls: ['./games-list.css']
})
export class GamesListComponent{
  @Input() games: GamesDto[] = [];
  @Input() showActions: boolean = false;
  @Output() edit = new EventEmitter<GamesDto>();
  @Output() delete = new EventEmitter<number>();
  readonly GameGenre = GameGenre;
  readonly GameStatus = GameStatus;

}
