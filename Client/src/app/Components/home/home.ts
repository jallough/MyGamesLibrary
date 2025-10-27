
import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { HttpGameService } from '../../Services/HttpGameService';
import { Observable, finalize } from 'rxjs';
import { GamesDto } from '../../Dtos/GamesDto';
import { GamesListComponent } from '../games/games-list/games-list';
@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule, GamesListComponent],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home implements OnInit {
  games$!: GamesDto[];
  loading = signal(false);

  constructor(private readonly gamesApi: HttpGameService) {}

  ngOnInit(): void {
    this.loadGames();
  }

  loadGames(): void {
    this.loading.set(true);
    this.gamesApi.getAllFiltered(undefined,undefined,undefined,0,10).subscribe(games => {
      this.games$ = games;
      this.loading.set(false);
    });
  }

  deleteGame(id: number) {
    this.gamesApi.delete(id).subscribe(() => this.loadGames());
  }
}
