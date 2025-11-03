import { Injectable } from "@angular/core";
import { map, Observable, tap } from "rxjs";
import { HttpService } from "./HttpService";
import { GamesDto } from "../Dtos/GamesDto";
import { UserGameRelationDto } from "../Dtos/UserGameRelationDto";

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
    getAllFilteredByUser(orderBy?:string,filterByCategory?: string, filterByStatus?:string, search?:string,page?:number,batch?:number,userId?:number): Observable<UserGameRelationDto[]> {
        return this.http.get<any[]>('Games/user/Filtered?orderBy=' + (orderBy ?? '') + '&filterByCategory=' + (filterByCategory ?? '') + '&filterByStatus=' + (filterByStatus ?? '') + '&search=' + (search ?? '') + '&page=' + (page ?? '') + '&batch=' + (batch ?? '') + '&userId=' + (userId));
    }
    getAvailableGames(orderBy?:string,filterByCategory?: string, search?:string,page?:number,batch?:number,userId?:number): Observable<GamesDto[]> {
        return this.http.get<any[]>('Games/Available/Filtered?orderBy=' + (orderBy ?? '') + '&filterByCategory=' + (filterByCategory ?? '') + '&search=' + (search ?? '') + '&page=' + (page ?? '') + '&batch=' + (batch ?? '') + '&userId=' + (userId));
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
    update(game: Partial<GamesDto>): Observable<GamesDto> {
        return this.http.put<GamesDto>(`Games`, game);
    }

    // DELETE api/games/{id}
    delete(id: number): Observable<void> {
        return this.http.delete<void>(`Games/${id}`);
    }

    AddGameRelation(userGame: UserGameRelationDto):Observable<UserGameRelationDto> {
        return this.http.post<UserGameRelationDto>('Games/AddRelation',userGame);
    }
    
    UpdateGameRelation(userGame: UserGameRelationDto):Observable<UserGameRelationDto> {
        return this.http.put<UserGameRelationDto>('Games/UpdateRelation',userGame);
    }
    DeleteGameRelation(id: number):Observable<void> {

        return this.http.delete<void>(`Games/DeleteRelation/${id}`);
    }
}
