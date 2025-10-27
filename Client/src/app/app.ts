import { Component, signal } from '@angular/core';
import { RouterOutlet,RouterModule } from '@angular/router';
import {MatSidenavModule} from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatDividerModule } from '@angular/material/divider';
import { MatButtonModule } from '@angular/material/button'; 
import { MatIconModule } from '@angular/material/icon';
import { BreakpointObserver } from '@angular/cdk/layout';
import { MatDialog } from '@angular/material/dialog';
import { Login } from './Components/users/login/login';
import { MatTooltipModule } from '@angular/material/tooltip';
import { HttpUserService } from './Services/HttpUserService';
@Component({
  selector: 'app-root',
  imports: [RouterModule,RouterOutlet, MatSidenavModule, MatToolbarModule, MatDividerModule, MatButtonModule, MatIconModule, MatTooltipModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('Games Library');
  protected currentUser;
  drawerMode: "side" | "over" = "over";
  isOpened = false;

  constructor(private breakpointObserver: BreakpointObserver, private readonly dialog: MatDialog, private readonly userService: HttpUserService) {
    this.breakpointObserver.observe(['(max-width: 768px)']).subscribe(result => {
      if (result.matches) {
        this.drawerMode = "over";
        this.isOpened = false;
      } else {
        this.drawerMode = "side";
        this.isOpened = true;
      }
    });
    this.userService.loadUserFromStorage();
    this.currentUser = this.userService.currentUserSig;
  }

  login() {
        const ref = this.dialog.open(Login, { width: '680px' });
        ref.afterClosed().subscribe();
      }
  logout() {
        this.userService.logout();
      }
}
