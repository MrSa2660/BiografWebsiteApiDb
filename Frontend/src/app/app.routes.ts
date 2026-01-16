import { Routes } from '@angular/router';
import { Home } from './home/home';
import { Login } from './login/login';
import { CinemaSelector } from './cinema-selector/cinema-selector';
import { TopNavbar } from './components/top-navbar/top-navbar';
import { SignUp } from './sign-up/sign-up';
import { Admin } from './admin/admin';
import { MovieDetailComponent } from './movie-detail.component/movie-detail.component';
import { SeatSelectionComponent } from './seat-selection.component/seat-selection.component';
import { TicketHistory } from './ticket-history/ticket-history';
import { AddShowtime } from './add-showtime/add-showtime';
import { AddScreen } from './add-screen/add-screen';
import { adminGuard } from './guards/admin.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'cinema-selector',
    pathMatch: 'full',
  },

  {
    path: 'cinema-selector',
    component: CinemaSelector,
    title: 'Select Cinema',
  },

  {
    path: '',
    component: TopNavbar,
    children: [
      { path: 'home/:city', component: Home, title: 'Home Page' },
      { path: 'login', component: Login, title: 'Login Page' },
      { path: 'sign-up', component: SignUp, title: 'Sign up' },
      { path: 'admin', component: Admin, title: 'Admin', canActivate: [adminGuard] },
      { path: 'admin/screens', component: AddScreen, title: 'Add screen', canActivate: [adminGuard] },
      { path: 'movie/:id/:city', component: MovieDetailComponent },
      { path: 'seats/:showtimeId', component: SeatSelectionComponent },
      { path: 'tickets', component: TicketHistory, title: 'My tickets' },
      { path: 'admin/showtimes', component: AddShowtime, title: 'Add showtime', canActivate: [adminGuard] },
    ],
  },

  // wildcard
  {
    path: '**',
    redirectTo: 'cinema-selector',
    pathMatch: 'full',
  },
];
