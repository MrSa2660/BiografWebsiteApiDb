// src/app/admin/admin.ts
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MovieService } from '../services/movie.service';
import { Movie } from '../models/movie.model';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './admin.html',
  styleUrls: ['./admin.css'],
})
  export class Admin {
    // Selected city for the new movie
  city: string = 'København';

  // Form fields
  title: string = '';
  posterUrl: string = '';
  rating: string = '';
  durationMinutes: number | null = null;
  genresText: string = '';          // e.g. "Action, Sci-Fi"
  language: string = 'English';
  showtimesText: string = '';       // e.g. "18:30, 21:00"
  makeHighlight: boolean = false;   // mark as highlight movie

  // Feedback message shown in UI
  message: string | null = null;

  // Dropdown list of available cities
  cities = [
    'København',
    'Stor København',
    'Aarhus',
    'Aalborg',
    'Fyn',
    'Nykøbing Falster',
  ];

  constructor(private movieService: MovieService) {}

  /** Validates input, builds movie object, and adds it to the service */
  addMovie() {
    // Check required fields
    if (!this.title || !this.posterUrl) {
      this.message = 'Title and poster URL are required.';
      return;
    }

    // Convert comma-separated text to arrays
    const genres = this.genresText
      .split(',')
      .map((g) => g.trim())
      .filter((g) => g);

    const showtimes = this.showtimesText
      .split(',')
      .map((t) => t.trim())
      .filter((t) => t);

      // Build a movie object without the ID (service generates it)
    const movieData: Omit<Movie, 'id'> = {
      title: this.title,                     // movie title from form
      posterUrl: this.posterUrl,             // poster image URL
      rating: this.rating || 'PG',           // fallback rating if empty
      durationMinutes: this.durationMinutes ?? 120,  // default to 120 min
      genres,                                // genre list from input text
      showtimes,                             // showtimes list from input text
      language: this.language || 'English',  // movie language
      cities: [this.city],                   // selected city (as array)
      isHighlight: this.makeHighlight,       // mark as highlight movie
    };

    // Save movie using service
    this.movieService.addMovie(movieData).subscribe({
      next: () => {
        // Show success message
        this.message = `Movie "${this.title}" added for ${this.city}.`;

        // Reset form fields
        this.title = '';
        this.posterUrl = '';
        this.rating = '';
        this.durationMinutes = null;
        this.genresText = '';
        this.language = 'English';
        this.showtimesText = '';
        this.makeHighlight = false;
      },
      error: () => {
        this.message = 'Could not save the movie. Please try again.';
      },
    });
  }
}
