// src/app/home/home.component.ts
import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MovieService } from '../services/movie.service';
import { Movie } from '../models/movie.model';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './home.html',
  styleUrls: ['./home.css'],
})
export class Home implements OnInit {
  city: string | null = null;
  highlightMovie: Movie | null = null;
  movies: Movie[] = [];

  constructor(private route: ActivatedRoute, private movieService: MovieService) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      this.city = params.get('city');
      this.movieService.getMoviesByCity(this.city).subscribe((movies) => {
        const highlight = movies.find((movie) => movie.isHighlight) ?? movies[0] ?? null;
        this.highlightMovie = highlight;
        this.movies = highlight ? movies.filter((movie) => movie.id !== highlight.id) : movies;
      });
    });
  }
}
