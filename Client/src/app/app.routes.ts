import { Routes } from '@angular/router';
import { Home } from './Components/home/home';
import { GamesOverview } from './Components/games/games-overview/games-overview';

export const routes: Routes = [
    {path: '', component: Home},
    {path: 'Games', component: GamesOverview},

    
];
