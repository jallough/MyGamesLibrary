import { Injectable } from '@angular/core';
import { CanActivate, CanActivateChild, CanActivateFn, Router } from '@angular/router';
import { HttpUserService } from '../../../Services/HttpUserService';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate, CanActivateChild {
  constructor(private userService: HttpUserService, private router: Router) {}

  canActivate(): boolean {
    if (this.userService.isLoggedIn()) {
      return true;
    }
    this.router.navigate(['/login']);
    return false;
  }

  canActivateChild(): boolean {
    if (this.userService.isLoggedIn() && this.userService.currentUserSig()?.role === 'Admin') {
      return true;
    }
    return false;
  }
}