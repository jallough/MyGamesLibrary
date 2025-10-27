import { Injectable } from "@angular/core";
import { map, Observable, tap } from "rxjs";
import { HttpService } from "./HttpService";
import { GamesDto } from "../Dtos/GamesDto";

@Injectable({ providedIn: 'root' })
export class HttpGameService {
    constructor(private readonly http: HttpService) {}

    // GET api/games
    getAll(): Observable<GamesDto[]> {
        return this.http.get<GamesDto[]>('Games');
    }
    getAllFiltered(orderBy?:string,filterByCategory?: string, search?:string,page?:number,batch?:number): Observable<GamesDto[]> {
        return this.http.get<GamesDto[]>('Games/Filtered?orderBy=' + (orderBy ?? '') + '&filterByCategory=' + (filterByCategory ?? '') + '&search=' + (search ?? '') + '&page=' + (page ?? '') + '&batch=' + (batch ?? ''));
    }
    getAllFilteredByUser(orderBy?:string,filterByCategory?: string, filterByStatus?:string, search?:string,page?:number,batch?:number,userId?:number): Observable<GamesDto[]> {
        return this.http.get<any[]>('Games/user/Filtered?orderBy=' + (orderBy ?? '') + '&filterByCategory=' + (filterByCategory ?? '') + '&filterByStatus=' + (filterByStatus ?? '') + '&search=' + (search ?? '') + '&page=' + (page ?? '') + '&batch=' + (batch ?? '') + '&userId=' + (userId))
        .pipe(
            map(relations =>
                relations.map(r => ({
                id: r.game.id,
                title: r.game.title,
                genre: r.game.genre,
                releaseDate: r.game.releaseDate,
                imageUrl: r.game.imageUrl,
                status: r.status
                }) as GamesDto)
            )
        );
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
