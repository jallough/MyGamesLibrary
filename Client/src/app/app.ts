import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {MatSidenavModule} from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatDividerModule } from '@angular/material/divider';
import { MatButtonModule } from '@angular/material/button'; 
import { MatIconModule, MatIconRegistry } from '@angular/material/icon';
import { BreakpointObserver } from '@angular/cdk/layout';
@Component({
  selector: 'app-root',
  imports: [RouterOutlet, MatSidenavModule, MatToolbarModule, MatDividerModule, MatButtonModule, MatIconModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('AngularApp');
  drawerMode: 'side' | 'over' = 'side';
  isOpened = true;

  constructor(private breakpointObserver: BreakpointObserver) {
    this.breakpointObserver.observe(['(max-width: 768px)']).subscribe(result => {
      if (result.matches) {
        this.drawerMode = 'over';
        this.isOpened = false;
      } else {
        this.drawerMode = 'side';
        this.isOpened = true;
      }
    });
  }
}
