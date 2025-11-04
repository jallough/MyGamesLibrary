import { ChangeDetectorRef, Component, OnInit, signal } from '@angular/core';
import { MatDialogRef, MatDialogModule, MatDialog } from '@angular/material/dialog';
import { HttpGameService } from '../../../Services/HttpGameService';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule, DatePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { GamesDto, GameGenre } from '../../../Dtos/GamesDto';
import { MatSelectModule } from '@angular/material/select';
import { FormControl } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { MatInput } from '@angular/material/input';
import { ReactiveFormsModule } from '@angular/forms';
import { GameStatusDialog } from '../game-status-dialog/game-status-dialog';
import { UserGameRelationDto } from '../../../Dtos/UserGameRelationDto';
import { HttpUserService } from '../../../Services/HttpUserService';
@Component({
  selector: 'app-games-catalog-dialog',
  imports: [CommonModule,ReactiveFormsModule,MatInput,MatCardModule,DatePipe,MatButtonModule,MatDialogModule,MatSelectModule],
  templateUrl: './games-catalog-dialog.html',
  styleUrl: './games-catalog-dialog.css'
})
export class GamesCatalogDialog implements OnInit {
  games:GamesDto[]=[];
  readonly GameGenre = GameGenre;
  searchControl = new FormControl('');
  private pageSize: number = 10;
  private currentPage: number = 0;
  searchValue='';
  orderValue='';
  categorieValue='';
  laodMoreVisible = signal(true);
  constructor(private readonly dialogRef: MatDialogRef<GamesCatalogDialog>,
    private readonly gamesApi:HttpGameService, 
    private readonly dialog: MatDialog,
    private readonly userService:HttpUserService,
    private cdr: ChangeDetectorRef) {
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


  ngOnInit(): void {
    this.LoadGames();
  }

  LoadGames(){
    this.currentPage=0;
    this.gamesApi.getAvailableGames(this.orderValue,this.categorieValue,this.searchValue,this.currentPage,this.pageSize,this.userService.currentUserSig()?.id).subscribe(games => { 
      this.games=games;
      this.cdr.detectChanges();
      if(games.length<this.pageSize){ 
        this.laodMoreVisible.set(false);
      }
    });
  }
  LoadMoreGames(){
    this.currentPage++;
    this.gamesApi.getAvailableGames(this.orderValue,this.categorieValue,this.searchValue,this.currentPage,this.pageSize,this.userService.currentUserSig()?.id).subscribe(games => { 
      this.games = this.games.concat(games);
      this.cdr.detectChanges();
      if(games.length<this.pageSize){ 
        this.laodMoreVisible.set(false);
      }
    });
  }
  orderByOptions = [
    { value: 'publishDateLH', label: 'Publish Date Low to High' },
    { value: 'publishDateHL', label: 'Publish Date High to Low' },
    { value: 'titleA', label: 'Title A to Z' },
    { value: 'titleD', label: 'Title Z to A' },
    { value: 'genreA', label: 'Genre A to Z' },
    { value: 'genreD', label: 'Genre Z to A' }
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

  public onOrderByChange(selectedValue: string) {
    this.orderValue = selectedValue;
    this.LoadGames();
  }

  public onFilterByChange(selectedValue: string) {
    this.categorieValue = selectedValue;
    this.LoadGames();
  }
  selectGame(g:GamesDto){
    const usergamerelation: UserGameRelationDto={
      game:g,
      gameId:g.id
    };
    const ref = this.dialog.open(GameStatusDialog, { data: { mode: 'add',game:usergamerelation }, width: '680px' });
          ref.afterClosed().subscribe(result => {
            if (result) this.LoadGames();
          });
  }

  close() {
    this.dialogRef.close(false);
  }
}
