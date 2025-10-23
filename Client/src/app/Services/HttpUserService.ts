import { Injectable, signal } from "@angular/core";
import { HttpService } from "./HttpService";
import { Observable, tap } from "rxjs";
import { UserDto } from "../Dtos/UserDto";

@Injectable({ providedIn: 'root' })
export class HttpUserService {
    currentUserSig = signal<UserDto | undefined | null>(undefined);
    constructor(private readonly http: HttpService) {}

    login(user: Partial<UserDto>): Observable<any> {
        return this.http.post<{ user: UserDto; token: string } >('Users/login', user,)
        .pipe(
            tap((response: any) => {
            if (response.token) {
                localStorage.setItem('token', response.token); // store JWT in browser
                this.currentUserSig.set(response.user);
            }
        })
        );
    }
    register(user: UserDto): Observable<any> {
        return this.http.post<any>('Users/register', user);
    }
    removeUser(user: UserDto): Observable<void> {
        return this.http.put<void>(`Users/`, user);
    }
    
    logout() {
        localStorage.removeItem('token');
    }

    getToken(): string | null {
        return localStorage.getItem('token');
    }

    isLoggedIn(): boolean {
        return !!this.getToken();
    }
}