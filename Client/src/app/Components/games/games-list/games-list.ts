import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { GamesDto, GameGenre, GameStatus } from '../../../Dtos/GamesDto';
import { MatSelectModule } from '@angular/material/select';
import { HttpGameService } from '../../../Services/HttpGameService';
import { MatInput } from '@angular/material/input';
import { FormControl, StatusChangeEvent } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
@Component({
  selector: 'app-games-list',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule, MatSelectModule, MatInput, ReactiveFormsModule],
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
    { value: 'title', label: 'Title' },
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

  searchControl = new FormControl('');

  constructor(private readonly gamesApi: HttpGameService) {
    this.searchControl.valueChanges
    .pipe(
        debounceTime(300),             // Wait 300ms after the last keystroke
        distinctUntilChanged()         // Only emit if value changed
      )
    .subscribe(value => {
      this.gamesApi.getAll().subscribe(games => {
      const searchTextLower = value!.toLowerCase();
      this.games = games.filter(g => g.title.toLowerCase().includes(searchTextLower));
    });
    });
  }

  public onOrderByChange(selectedValue: string) {
    this.games.sort((a, b) => {
      if (selectedValue === 'title') {
        return a.title.localeCompare(b.title);
      } else if (selectedValue === 'genre') {
        return a.genre - b.genre;
      } else {
        return  new Date(b.releaseDate).getTime()-new Date(a.releaseDate).getTime();
      }
  });
  }

  public onFilterByChange(selectedValue: string) {
    this.gamesApi.getAll().subscribe(games => {
      let filteredGames = games;
      if (selectedValue !== 'all') {
        const statusMap: any = {
          completed: GameStatus.Completed,
          playing: GameStatus.Playing,
          onHold: GameStatus.OnHold,
          dropped: GameStatus.Dropped,
          planToPlay: GameStatus.PlanToPlay
        };
        filteredGames = filteredGames.filter(g => g.status === statusMap[selectedValue]);
      }
      if (selectedValue !== 'all') {
        const categoryMap: any = {
          action: GameGenre.Action,
          adventure: GameGenre.Adventure,
          rpg: GameGenre.RPG,
          strategy: GameGenre.Strategy, 
          simulation: GameGenre.Simulation,
          sports: GameGenre.Sports,
          puzzle: GameGenre.Puzzle,
          horror: GameGenre.Horror,
          mmo: GameGenre.MMO,
          indie: GameGenre.Indie
        };
        filteredGames = filteredGames.filter(g => g.genre === categoryMap[selectedValue]);
      }
      this.games = filteredGames;
    });
  }

}
