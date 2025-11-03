import { Component, OnInit, signal } from '@angular/core';
import { UserGameRelationDto } from '../../../Dtos/UserGameRelationDto';
import { HttpGameService } from '../../../Services/HttpGameService';
import { HttpUserService } from '../../../Services/HttpUserService';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog } from '@angular/material/dialog';
import { GamesCatalogDialog } from '../games-catalog-dialog/games-catalog-dialog';
import { GameStatusDialog } from '../game-status-dialog/game-status-dialog';
import { CommonModule, DatePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { GameGenre, GameStatus } from '../../../Dtos/GamesDto';
import { MatLabel, MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-user-game-overview',
  imports: [MatIconModule,ReactiveFormsModule,CommonModule,MatCardModule,MatButtonModule,MatLabel,MatFormFieldModule,MatSelectModule,MatInputModule,DatePipe],
  templateUrl: './user-game-overview.html',
  styleUrl: './user-game-overview.css'
})
export class UserGameOverview implements OnInit{
  constructor(private readonly gamesService:HttpGameService,private readonly userService:HttpUserService,private readonly dialog:MatDialog){
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
  loading = signal(false);
  private pageSize: number = 10;
  private currentPage: number = 0;
  readonly GameStatus=GameStatus;
  readonly GameGenre=GameGenre;
  searchValue='';
  orderValue='';
  categorieValue='';
  statusValue='';
  userGames:UserGameRelationDto[]=[];
  searchControl = new FormControl('');
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


  ngOnInit(): void {
    this.LoadGames();
  }

  LoadGames(){
    this.loading.set(true);
    this.currentPage=0;
    this.gamesService.getAllFilteredByUser(this.orderValue,this.categorieValue,this.statusValue,this.searchValue,this.currentPage,this.pageSize,this.userService.currentUserSig()?.id)
    .subscribe(games=>{
      this.userGames=games;
      this.loading.set(false);
      if(games.length<this.pageSize){ 
        this.loading.set(true);
      }
    }); 
    
  }
  LoadMoreUserGames(){
    this.currentPage++;
    this.loading.set(true);
    this.gamesService.getAllFilteredByUser(this.orderValue,this.categorieValue,this.statusValue,this.searchValue,this.currentPage,this.pageSize,this.userService.currentUserSig()?.id)
    .subscribe(games=>{
      this.userGames=this.userGames.concat(games);
      if(games.length==this.pageSize){ 
        this.loading.set(false);
      }
    });
  }
  public onOrderByChange(selectedValue: string) {
    this.orderValue = selectedValue;
    this.LoadGames();
  }

  public onFilterByStatusChange(selectedValue: string) {
    this.statusValue = selectedValue;
    this.LoadGames();
  }
  public onFilterCategorieByChange(selectedValue: string) {
    this.categorieValue = selectedValue;
    this.LoadGames();
  }

  openAdd() {
        const ref = this.dialog.open(GamesCatalogDialog, { data: { mode: 'add' }, width: '1000px', maxWidth: '100vw' });
        ref.afterClosed().subscribe(() => this.LoadGames());
      }

  openEdit(game: UserGameRelationDto) {
    const ref = this.dialog.open(GameStatusDialog, { data: { mode: 'edit', game }, width: '680px' });
    ref.afterClosed().subscribe(() => this.LoadGames());
  }

  deleteRelation(id: number) {
    if (!confirm('Delete this game?')) return;
    this.gamesService.DeleteGameRelation(id).subscribe(() => this.LoadGames());
  }   
}
