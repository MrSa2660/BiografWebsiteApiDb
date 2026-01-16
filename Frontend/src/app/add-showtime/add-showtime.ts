import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MovieService } from '../services/movie.service';
import { ShowtimeService, ScreenResponse, ShowtimeResponse } from '../services/showtime.service';
import { ChangeDetectorRef } from '@angular/core';

interface MovieOption {
  id: number;
  title: string;
}

@Component({
  selector: 'app-add-showtime',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './add-showtime.html',
  styleUrl: './add-showtime.css',
})
export class AddShowtime implements OnInit {
  movies: MovieOption[] = [];
  screens: ScreenResponse[] = [];
  showtimes: ShowtimeResponse[] = [];

  movieId: number | null = null;
  screenId: number | null = null;
  date = '';
  time = '';
  basePrice = 95;
  is3D = false;
  language = 'English';

  message = '';
  error = '';
  showtimesError = '';
  isSubmitting = false;
  isLoadingShowtimes = false;

  constructor(
    private movieService: MovieService,
    private showtimeService: ShowtimeService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    // load movies + screens immediately
    this.movieService.getAllMovies().subscribe({
      next: (movies) => {
        this.movies = movies.map((m) => ({ id: m.id, title: m.title }));
        this.cdr.markForCheck();
      },
      error: () => {
        this.error = 'Could not load movies';
        this.cdr.markForCheck();
      },
    });

    this.showtimeService.getScreens().subscribe({
      next: (screens) => {
        this.screens = screens;
        this.cdr.markForCheck();
      },
      error: () => {
        this.error = 'Could not load screens';
        this.cdr.markForCheck();
      },
    });

    // auto-load showtimes immediately
    this.isLoadingShowtimes = true;
    this.loadShowtimes();
  }

  submit() {
    this.error = '';
    this.message = '';

    if (!this.movieId || !this.screenId || !this.date || !this.time) {
      this.error = 'Please fill movie, screen, date, and time.';
      return;
    }

    const start = this.combineDateTime(this.date, this.time);
    this.isSubmitting = true;
    this.showtimeService
      .createShowtime({
        movieId: this.movieId,
        screenId: this.screenId,
        startTime: start,
        basePrice: this.basePrice,
        is3D: this.is3D,
        language: this.language,
      })
      .subscribe({
        next: () => {
          this.isSubmitting = false;
          this.message = 'Showtime created.';
          this.time = '';
          this.loadShowtimes();
          this.cdr.markForCheck();
        },
        error: () => {
          this.isSubmitting = false;
          this.error = 'Could not create showtime.';
          this.cdr.markForCheck();
        },
      });
  }

  loadShowtimes() {
    this.showtimesError = '';
    this.isLoadingShowtimes = true;
    this.showtimeService.getAllShowtimes().subscribe({
      next: (showtimes) => {
        this.showtimes = [...showtimes].sort(
          (a, b) => new Date(a.startTime).getTime() - new Date(b.startTime).getTime()
        );
        this.isLoadingShowtimes = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.isLoadingShowtimes = false;
        this.showtimesError = 'Could not load showtimes.';
        this.cdr.markForCheck();
      },
    });
  }

  getMovieTitle(movieId: number): string {
    return this.movies.find((movie) => movie.id === movieId)?.title ?? `Movie ${movieId}`;
  }

  getScreenName(screenId: number): string {
    return this.screens.find((screen) => screen.id === screenId)?.name ?? `Screen ${screenId}`;
  }

  formatDate(value: string): string {
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) return value;
    return date.toLocaleDateString('en-GB', {
      day: '2-digit',
      month: 'short',
      year: 'numeric',
    });
  }

  formatTime(value: string): string {
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) return value;
    return date.toLocaleTimeString('en-GB', {
      hour: '2-digit',
      minute: '2-digit',
    });
  }

  private combineDateTime(date: string, time: string): string {
    const iso = new Date(`${date}T${time}`).toISOString();
    return iso;
  }
}
