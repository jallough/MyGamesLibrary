import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { GamesDto, GameGenre, GameStatus } from '../../../Dtos/GamesDto';
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-games-list',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule, MatSelectModule],
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
  orderByOptions = [
    { value: 'publishDate', label: 'Publish Date' },
    { value: 'name', label: 'Name' },
    { value: 'genre', label: 'Genre' }
  ];
  filterByOptions = [
    { value: 'all', label: 'All' },
    { value: 'completed', label: 'Completed' },
    { value: 'playing', label: 'Playing' },
    { value: 'onHold', label: 'On Hold' },
    { value: 'dropped', label: 'Dropped' },
    { value: 'planToPlay', label: 'Plan to Play' }
  ];
  filterByCategoryOptions = [
    { value: 'all', label: 'All' },
    { value: 'action', label: 'Action' },
    { value: 'adventure', label: 'Adventure' },
    { value: 'rpg', label: 'RPG' },
    { value: 'strategy', label: 'Strategy' },
    { value: 'simulation', label: 'Simulation' },
    { value: 'sports', label: 'Sports' },
    { value: 'puzzle', label: 'Puzzle' },
    { value: 'horror', label: 'Horror' },
    { value: 'mmo', label: 'MMO' },
    { value: 'indie', label: 'Indie' }
  ];

  searchText: string = '';
}
