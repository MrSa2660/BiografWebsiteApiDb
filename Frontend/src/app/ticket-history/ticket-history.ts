import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { forkJoin } from 'rxjs';
import { BookingResponse, BookingService, TicketResponse } from '../services/booking.service';
import { AuthService } from '../services/auth.service';
import { ShowtimeService, ShowtimeResponse, ScreenResponse } from '../services/showtime.service';
import { MovieService } from '../services/movie.service';
import { Movie } from '../models/movie.model';

interface TicketView {
  id: number;
  code: string;
  seatLabel: string;
  issuedAt: string;
  bookingId: number;
  bookingStatus: string;
  startTime: string;
  startDateLabel: string;
  startTimeLabel: string;
  movieTitle: string;
  screenName: string;
}

@Component({
  selector: 'app-ticket-history',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './ticket-history.html',
  styleUrl: './ticket-history.css',
})
export class TicketHistory implements OnInit {
  tickets: TicketView[] = [];
  isLoading = false;
  error = '';
  userName = '';

  constructor(
    private bookingService: BookingService,
    private showtimeService: ShowtimeService,
    private movieService: MovieService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    const user = this.authService.getCurrentUser();
    if (!user) {
      this.error = 'Log in to see your tickets.';
      this.isLoading = false;
      this.cdr.markForCheck();
      return;
    }

    this.userName = user.fullName;
    this.loadTickets(user.id);
  }

  trackById(_: number, item: TicketView) {
    return item.id;
  }

  private loadTickets(userId: number) {
    this.isLoading = true;
    this.error = '';

    forkJoin({
      bookings: this.bookingService.getUserBookings(userId),
      showtimes: this.showtimeService.getAllShowtimes(),
      screens: this.showtimeService.getScreens(),
      movies: this.movieService.getAllMovies(),
    }).subscribe({
      next: ({ bookings, showtimes, screens, movies }) => {
        this.tickets = this.buildTicketViews(bookings, showtimes, screens, movies);
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.error = 'Could not load your tickets.';
        this.isLoading = false;
        this.cdr.markForCheck();
      },
    });
  }

  private buildTicketViews(
    bookings: BookingResponse[],
    showtimes: ShowtimeResponse[],
    screens: ScreenResponse[],
    movies: Movie[]
  ): TicketView[] {
    const showtimeMap = new Map<number, ShowtimeResponse>(
      showtimes.map((s) => [s.id, s])
    );
    const screenMap = new Map<number, string>(screens.map((s) => [s.id, s.name]));
    const movieMap = new Map<number, Movie>(movies.map((m) => [m.id, m]));

    const views: TicketView[] = [];
    for (const booking of bookings) {
      const st = showtimeMap.get(booking.showtimeId);
      const movie = st ? movieMap.get(st.movieId) : undefined;
      const screenName = st ? (screenMap.get(st.screenId) ?? `Screen ${st.screenId}`) : 'Screen';
      const date = st ? new Date(st.startTime) : null;
      const startDateLabel = date
        ? date.toLocaleDateString('en-GB', { weekday: 'short', day: '2-digit', month: 'short' })
        : 'Date TBC';
      const startTimeLabel = date
        ? date.toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit' })
        : '--:--';

      for (const ticket of booking.tickets) {
        views.push({
          id: ticket.id,
          code: ticket.code,
          seatLabel: ticket.seatLabel,
          issuedAt: ticket.issuedAt,
          bookingId: booking.id,
          bookingStatus: booking.status,
          startTime: st?.startTime ?? '',
          startDateLabel,
          startTimeLabel,
          movieTitle: movie?.title ?? `Movie ${st?.movieId ?? ''}`.trim(),
          screenName,
        });
      }
    }

    return views.sort(
      (a, b) => new Date(b.issuedAt).getTime() - new Date(a.issuedAt).getTime()
    );
  }
}
