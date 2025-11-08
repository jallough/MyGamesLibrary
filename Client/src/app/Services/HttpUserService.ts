import { Injectable, signal } from "@angular/core";
import { HttpService } from "./HttpService";
import { map, Observable, tap } from "rxjs";
import { UserDto } from "../Dtos/UserDto";
import {jwtDecode} from "jwt-decode";
import { Router } from "@angular/router";
export interface JwtPayload {
    accessToken: string;
    refreshToken: string;
}
@Injectable({ providedIn: 'root' })
export class HttpUserService {
    currentUserSig = signal<UserDto | null>(null);
    constructor(private readonly http: HttpService, private readonly router : Router) {}

    login(user: Partial<UserDto>): Observable<any> {
        return this.http.post<JwtPayload>('Users/login', user,)
        .pipe(
            tap((response: any) => {
            if (response) {
                localStorage.setItem('token', response.accessToken);
                localStorage.setItem('refreshToken', response.refreshToken); // store JWT in browser
                const userFromToken = this.decodeUserFromToken(response.accessToken);
                this.currentUserSig.set(userFromToken);
            }
        })
        );
    }
    decodeUserFromToken(token: string): UserDto | null {
    try {
      const payload: any = jwtDecode(token);

    const user: UserDto = {
      id: payload['Id'] ?? payload['sub'] ?? '',
      username:
        payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ??
        payload['unique_name'] ??
        '',
      email:
        payload[
          'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'
        ] ?? '',
      role:
        payload[
          'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
        ] ?? '',
    };
        console.log('✅ Decoded user from token:', user);
        return user;
    } catch (err) {
      console.error('❌ Failed to decode token', err);
      return null;
    }
  }

  loadUserFromStorage() {
    const token = localStorage.getItem('token');
    if (token) {
      const user = this.decodeUserFromToken(token);
      this.currentUserSig.set(user);
    }
  }
  refreshToken(): Observable<boolean> {
        const refreshToken = localStorage.getItem('refreshToken');
        if (!refreshToken) return new Observable<boolean>(sub => { sub.next(false); sub.complete(); });
        return this.http.post<{ token: string; refreshToken: string }>('Users/refresh', refreshToken)
            .pipe(
                tap(resp => {
                    if (resp.token) {
                        localStorage.setItem('token', resp.token);
                        if (resp.refreshToken) localStorage.setItem('refreshToken', resp.refreshToken);
                        const userFromToken = this.decodeUserFromToken(resp.token);
                        this.currentUserSig.set(userFromToken);
                    }
                }),
                map(resp => !!resp?.token)
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
        this.currentUserSig.set(null);
        this.router.navigate(['/']);
    }

    getToken(): string | null {
        return localStorage.getItem('token');
    }

    isLoggedIn(): boolean {
        return !!this.getToken();
    }
}