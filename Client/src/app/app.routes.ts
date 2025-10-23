import { Routes } from '@angular/router';
import { Home } from './Components/home/home';
import { GamesOverview } from './Components/games/games-overview/games-overview';
import { AuthGuard } from './Components/users/auth/auth-guard';
import { Login } from './Components/users/login/login';

export const routes: Routes = [
    {path: '', component: Home},
    {path: 'Games', component: GamesOverview, canActivate: [AuthGuard]},
    {path: 'login', component : Login},
    
];
