// src/app/movie-detail/movie-detail.component.ts
import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MovieService } from '../services/movie.service';
import { Movie } from '../models/movie.model';
import { Showtime } from '../models/showtime.model';
import { ShowtimeService, ScreenResponse, ShowtimeResponse } from '../services/showtime.service';

@Component({
  selector: 'app-movie-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './movie-detail.component.html',
  styleUrls: ['./movie-detail.component.css'],
})
export class MovieDetailComponent implements OnInit {
  movie: Movie | undefined;
  city: string | null = null;
  showtimesByCinema: { cinemaName: string; showtimes: ShowtimeResponse[] }[] = [];
  screens: ScreenResponse[] = [];
  defaultPoster = 'https://placehold.co/400x600/111827/ffffff?text=Poster+coming+soon';

  constructor(
    private route: ActivatedRoute,
    private movieService: MovieService,
    private showtimeService: ShowtimeService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      const id = Number(params.get('id'));
      this.city = params.get('city');

      this.movieService.getMovieById(id).subscribe((movie) => {
        this.movie = movie;
        this.cdr.markForCheck();
      });
      this.loadScreensAndShowtimes(id);
    });
  }

  scrollToShowtimes(event: Event) {
    event.preventDefault(); // stop the router / navigation
    const el = document.getElementById('showtimes');
    if (el) {
      el.scrollIntoView({ behavior: 'smooth' });
    }
  }

  // used to build a nice date label: e.g. "Tor, 27/11"
  formatDate(dateStr: string): string {
    const d = new Date(dateStr);
    return d.toLocaleDateString('da-DK', {
      weekday: 'short',
      day: '2-digit',
      month: '2-digit',
    });
  }

  getScreenName(id: number): string {
    const screen = this.screens.find((s) => s.id === id);
    return screen?.name ?? `Sal ${id}`;
  }

  private loadScreensAndShowtimes(movieId: number) {
    this.showtimeService.getScreens().subscribe({
      next: (screens) => {
        this.screens = screens;
        this.loadShowtimes(movieId);
        this.cdr.markForCheck();
      },
      error: () => {
        this.screens = [];
        this.loadShowtimes(movieId);
        this.cdr.markForCheck();
      },
    });
  }

  private loadShowtimes(movieId: number) {
    this.showtimeService.getAllShowtimes().subscribe((all) => {
      const filtered = all.filter((s) => s.movieId === movieId);
      const byScreen: Record<number, ShowtimeResponse[]> = {};
      for (const s of filtered) {
        if (!byScreen[s.screenId]) byScreen[s.screenId] = [];
        byScreen[s.screenId].push(s);
      }
      this.showtimesByCinema = Object.keys(byScreen).map((screenId) => {
        const name = this.getScreenName(Number(screenId));
        return { cinemaName: name, showtimes: byScreen[Number(screenId)] };
      });
      this.cdr.markForCheck();
    });
  }

  onImgError(event: Event): void {
    const target = event.target as HTMLImageElement | null;
    if (target && target.src !== this.defaultPoster) {
      target.src = this.defaultPoster;
    }
  }
}
