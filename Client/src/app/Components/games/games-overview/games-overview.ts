import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GamesListComponent } from '../games-list/games-list';
import { HttpGameService } from '../../../Services/HttpGameService';
import { GamesDto } from '../../../Dtos/GamesDto';
import { firstValueFrom, Observable } from 'rxjs';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog } from '@angular/material/dialog';
import { GameDialog } from '../games-dialog/game-dialog';

@Component({
  selector: 'app-games-overview',
  standalone: true,
  imports: [CommonModule, GamesListComponent, MatButtonModule, MatIconModule],
  templateUrl: './games-overview.html',
  styleUrl: './games-overview.css'
})
export class GamesOverview implements OnInit{
  games: GamesDto[] = [];
  loading = signal(false);

  constructor(private readonly gamesApi: HttpGameService, private readonly dialog: MatDialog) {}

  ngOnInit(): void {
      this.loadGames();
    }
  
    async loadGames() {
      this.loading.set(true);
      this.gamesApi.getAll().subscribe(g=> {this.games = g; this.loading.set(false);} );
      
    }
  
    openAdd() {
      const ref = this.dialog.open(GameDialog, { data: { mode: 'add' }, width: '680px' });
      ref.afterClosed().subscribe(result => {
        if (result) this.loadGames();
      });
    }
  
    openEdit(game: GamesDto) {
      const ref = this.dialog.open(GameDialog, { data: { mode: 'edit', game }, width: '680px' });
      ref.afterClosed().subscribe(result => {
        if (result) this.loadGames();
      });
    }
  
    deleteGame(id: number) {
      if (!confirm('Delete this game?')) return;
      this.gamesApi.delete(id).subscribe(() => this.loadGames());
    }
}
