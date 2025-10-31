import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { GamesDto, GameGenre, GameStatus } from '../../../Dtos/GamesDto';
import { MatSelectModule } from '@angular/material/select';
import { HttpGameService } from '../../../Services/HttpGameService';
import { MatInput } from '@angular/material/input';
import { FormControl } from '@angular/forms';
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
  private pageSize: number = 10;
  private currentPage: number = 0;
  searchValue='';
  orderValue='';
  categorieValue='';
  orderByOptions = [
    { value: 'publishDateLH', label: 'Publish Date Low to High' },
    { value: 'publishDateHL', label: 'Publish Date High to Low' },
    { value: 'titleA', label: 'Title A to Z' },
    { value: 'titleD', label: 'Title Z to A' },
    { value: 'genreA', label: 'Genre A to Z' },
    { value: 'genreD', label: 'Genre Z to A' }
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
      this.searchValue = value!;
      this.LoadGames();
    });
  }
  LoadGames(){
    this.gamesApi.getAllFiltered(this.orderValue,this.categorieValue,this.searchValue,this.currentPage,this.pageSize).subscribe(games => { 
      this.games=games;
    });
  }
  public onOrderByChange(selectedValue: string) {
    this.orderValue = selectedValue;
    this.LoadGames();
  }

  public onFilterByChange(selectedValue: string) {
    this.categorieValue = selectedValue;
    this.LoadGames();
  }

}
