import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MovieService } from '../services/movie.service';
import { ShowtimeService, ScreenResponse } from '../services/showtime.service';

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

  movieId: number | null = null;
  screenId: number | null = null;
  date = '';
  time = '';
  basePrice = 95;
  is3D = false;
  language = 'English';

  message = '';
  error = '';
  isSubmitting = false;

  constructor(private movieService: MovieService, private showtimeService: ShowtimeService) {}

  ngOnInit(): void {
    this.movieService.getAllMovies().subscribe({
      next: (movies) => {
        this.movies = movies.map((m) => ({ id: m.id, title: m.title }));
      },
      error: () => (this.error = 'Could not load movies'),
    });

    this.showtimeService.getScreens().subscribe({
      next: (screens) => (this.screens = screens),
      error: () => (this.error = 'Could not load screens'),
    });
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
        },
        error: () => {
          this.isSubmitting = false;
          this.error = 'Could not create showtime.';
        },
      });
  }

  private combineDateTime(date: string, time: string): string {
    const iso = new Date(`${date}T${time}`).toISOString();
    return iso;
  }
}
