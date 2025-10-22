import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { HttpService } from "./HttpService";
import { GamesDto } from "../Dtos/GamesDto";

@Injectable({ providedIn: 'root' })
export class HttpGameService {
    constructor(private readonly http: HttpService) {}

    // GET api/games
    getAll(): Observable<GamesDto[]> {
        return this.http.get<GamesDto[]>('Games');
    }

    getAvailableGames(): Observable<GamesDto[]> {
        return this.http.get<GamesDto[]>('Games/available');
    }

    getGamesByUser(userId: number): Observable<GamesDto[]> {
        return this.http.get<GamesDto[]>(`Games/user/${userId}`);
    }
    getGamesByCategory(categoryId: number): Observable<GamesDto[]> {
        return this.http.get<GamesDto[]>(`Games/category/${categoryId}`);
    }

    // GET api/games/{id}
    getById(id: number): Observable<GamesDto> {
        return this.http.get<GamesDto>(`Games/${id}`);
    }

    // POST api/games
    create(game: Partial<GamesDto>): Observable<GamesDto> {
        return this.http.post<GamesDto>('Games', game);
    }

    // PUT api/games/{id}
    update(id: number, game: Partial<GamesDto>): Observable<GamesDto> {
        return this.http.put<GamesDto>(`Games/${id}`, game);
    }

    // DELETE api/games/{id}
    delete(id: number): Observable<void> {
        return this.http.delete<void>(`Games/${id}`);
    }
}
