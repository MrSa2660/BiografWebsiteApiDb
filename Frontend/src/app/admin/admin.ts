// src/app/admin/admin.ts
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MovieService, CreateMovieRequest } from '../services/movie.service';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './admin.html',
  styleUrls: ['./admin.css'],
})
export class Admin {
  // Form fields
  title = '';
  description = '';
  posterUrl = '';
  trailerUrl = '';
  rating = '';
  durationMinutes: number | null = null;
  genresText = ''; // e.g. "Action, Sci-Fi"
  language = 'English';
  makeHighlight = false; // mark as highlight movie
  isNowShowing = true;
  releaseDate = '';

  // Feedback message shown in UI
  message: string | null = null;
  error: string | null = null;

  constructor(private movieService: MovieService) {}

  /** Validates input, builds movie object, and adds it to the service */
  addMovie() {
    this.error = null;
    this.message = null;

    // Check required fields
    if (!this.title || !this.posterUrl || !this.durationMinutes) {
      this.error = 'Title, poster URL, and duration are required.';
      return;
    }

    // Convert comma-separated text to arrays
    const genres = this.genresText
      .split(',')
      .map((g) => g.trim())
      .filter((g) => g);

    // Build a movie object without the ID (service generates it)
    const movieData: CreateMovieRequest = {
      title: this.title, // movie title from form
      description: this.description.trim(),
      posterUrl: this.posterUrl, // poster image URL
      trailerUrl: this.trailerUrl.trim(),
      rating: this.rating || 'PG', // fallback rating if empty
      durationMinutes: this.durationMinutes ?? 120, // default to 120 min
      genres, // genre list from input text
      language: this.language || 'English', // movie language
      isHighlight: this.makeHighlight, // mark as highlight movie
      isNowShowing: this.isNowShowing,
      releaseDate: this.releaseDate ? new Date(this.releaseDate).toISOString() : null,
    };

    // Save movie using service
    this.movieService.addMovie(movieData).subscribe({
      next: () => {
        // Show success message
        this.message = `Movie "${movieData.title}" added.`;
        this.error = null;

        // Reset form fields
        this.title = '';
        this.description = '';
        this.posterUrl = '';
        this.trailerUrl = '';
        this.rating = '';
        this.durationMinutes = null;
        this.genresText = '';
        this.language = 'English';
        this.makeHighlight = false;
        this.isNowShowing = true;
        this.releaseDate = '';
      },
      error: () => {
        this.error = 'Could not save the movie. Please try again.';
      },
    });
  }
}
