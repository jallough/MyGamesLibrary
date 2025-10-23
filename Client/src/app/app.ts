import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {MatSidenavModule} from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatDividerModule } from '@angular/material/divider';
import { MatButtonModule } from '@angular/material/button'; 
import { MatIconModule, MatIconRegistry } from '@angular/material/icon';
import { BreakpointObserver } from '@angular/cdk/layout';
import { MatDialog } from '@angular/material/dialog';
import { Login } from './Components/users/login/login';
import { MatTooltipModule } from '@angular/material/tooltip';
@Component({
  selector: 'app-root',
  imports: [RouterOutlet, MatSidenavModule, MatToolbarModule, MatDividerModule, MatButtonModule, MatIconModule, MatTooltipModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('AngularApp');
  drawerMode: "side" | "over" = "over";
  isOpened = false;

  constructor(private breakpointObserver: BreakpointObserver, private readonly dialog: MatDialog) {
    this.breakpointObserver.observe(['(max-width: 768px)']).subscribe(result => {
      if (result.matches) {
        this.drawerMode = "over";
        this.isOpened = false;
      } else {
        this.drawerMode = "side";
        this.isOpened = true;
      }
    });
  }

  login() {
        const ref = this.dialog.open(Login, { width: '680px' });
        ref.afterClosed().subscribe();
      }
}
