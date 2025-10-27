import { Injectable, signal } from "@angular/core";
import { HttpService } from "./HttpService";
import { Observable, tap } from "rxjs";
import { UserDto } from "../Dtos/UserDto";
import {jwtDecode} from "jwt-decode";
import { Router } from "@angular/router";
interface JwtPayload {
  sub?: string;
  name?: string;
  emailaddress?: string;
  role?: string;
  exp?: number;
  [key: string]: any;
}

@Injectable({ providedIn: 'root' })
export class HttpUserService {
    currentUserSig = signal<UserDto | null>(null);
    constructor(private readonly http: HttpService, private readonly router : Router) {}

    login(user: Partial<UserDto>): Observable<any> {
        return this.http.post<{token: string} >('Users/login', user,)
        .pipe(
            tap((response: any) => {
            if (response.token) {
                localStorage.setItem('token', response.token); // store JWT in browser
                const userFromToken = this.decodeUserFromToken(response.token);
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