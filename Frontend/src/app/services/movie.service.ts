import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { Movie } from '../models/movie.model';
import { Showtime } from '../models/showtime.model';
import { AuthService } from './auth.service';

interface MovieApiResponse {
  id: number;
  title: string;
  description: string | null;
  durationMinutes: number;
  genre: string | null;
  rating: string | null;
  language: string | null;
  posterUrl: string | null;
  trailerUrl: string | null;
  showtimes: string | null;
  isHighlight: boolean;
  isNowShowing: boolean;
  releaseDate: string | null;
}

interface MovieApiRequest {
  id?: number;
  title: string;
  description: string | null;
  durationMinutes: number;
  genre: string | null;
  rating: string | null;
  language: string | null;
  posterUrl: string | null;
  trailerUrl: string | null;
  showtimes: string | null;
  isHighlight: boolean;
  isNowShowing: boolean;
  releaseDate: string | null;
}

export interface CreateMovieRequest {
  title: string;
  description: string;
  durationMinutes: number;
  genres: string[];
  rating: string;
  language: string;
  posterUrl: string;
  trailerUrl: string;
  isHighlight: boolean;
  isNowShowing: boolean;
  releaseDate: string | null;
}

@Injectable({
  providedIn: 'root',
})
export class MovieService {
  private http = inject(HttpClient);
  private auth = inject(AuthService);
  private baseUrl = 'http://localhost:5104/api/movies';
  private showtimes: Showtime[] = [
    // København
    {
      id: 101,
      movieId: 1,
      cinemaName: 'Lyngby Kinopalæet',
      city: 'København',
      hall: 'Bio 12',
      date: '2025-11-27',
      time: '16.00',
      format: '2D, Eng. tale',
      availability: 'HIGH',
    },
    {
      id: 102,
      movieId: 2,
      cinemaName: 'Lyngby Kinopalæet',
      city: 'København',
      hall: 'Bio 6',
      date: '2025-11-27',
      time: '19.00',
      format: '2D, Eng. tale',
      availability: 'HIGH',
    },
    {
      id: 103,
      movieId: 1,
      cinemaName: 'Lyngby Kinopalæet',
      city: 'København',
      hall: 'Bio 10',
      date: '2025-11-27',
      time: '21.15',
      format: '2D, Eng. tale',
      availability: 'MEDIUM',
    },
    {
      id: 201,
      movieId: 3,
      cinemaName: 'Waves',
      city: 'København',
      hall: 'Bio 6',
      date: '2025-11-27',
      time: '16.15',
      format: '2D, Eng. tale',
      availability: 'HIGH',
    },
    {
      id: 202,
      movieId: 1,
      cinemaName: 'Waves',
      city: 'København',
      hall: 'Bio 3',
      date: '2025-11-28',
      time: '17.00',
      format: '2D, Eng. tale',
      availability: 'HIGH',
    },
    {
      id: 301,
      movieId: 2,
      cinemaName: 'Palads',
      city: 'København',
      hall: 'Bio 10',
      date: '2025-11-27',
      time: '16.30',
      format: '2D, Eng. tale',
      availability: 'HIGH',
    },
    {
      id: 302,
      movieId: 1,
      cinemaName: 'Palads',
      city: 'København',
      hall: 'Bio 10',
      date: '2025-11-27',
      time: '19.00',
      format: '2D, Eng. tale',
      availability: 'HIGH',
    },
    {
      id: 303,
      movieId: 2,
      cinemaName: 'Palads',
      city: 'København',
      hall: 'Bio 10',
      date: '2025-11-27',
      time: '21.30',
      format: '2D, Eng. tale',
      availability: 'MEDIUM',
    },
    // Aarhus
    {
      id: 401,
      movieId: 1,
      cinemaName: 'Bruuns Galleri',
      city: 'Aarhus',
      hall: 'Sal 4',
      date: '2025-11-27',
      time: '18.00',
      format: '2D, Eng. tale',
      availability: 'HIGH',
    },
    {
      id: 402,
      movieId: 2,
      cinemaName: 'Bruuns Galleri',
      city: 'Aarhus',
      hall: 'Sal 1',
      date: '2025-11-27',
      time: '20.30',
      format: '2D, Eng. tale',
      availability: 'MEDIUM',
    },
    // Aalborg
    {
      id: 501,
      movieId: 3,
      cinemaName: 'Kennedy Arkaden',
      city: 'Aalborg',
      hall: 'Sal 2',
      date: '2025-11-27',
      time: '17.30',
      format: '2D, Eng. tale',
      availability: 'HIGH',
    },
  ];

  /** Returns all movies */
  getAllMovies(): Observable<Movie[]> {
    return this.http.get<MovieApiResponse[]>(this.baseUrl).pipe(
      map((movies) => movies.map((movie) => this.toMovie(movie)))
    );
  }

  /** Returns movies (city filter no longer needed) */
  getMoviesByCity(_city: string | null): Observable<Movie[]> {
    return this.getAllMovies();
  }

  /** Returns the highlighted movie for a city (fallback to first movie) */
  getHighlightForCity(city: string | null): Observable<Movie | null> {
    return this.getMoviesByCity(city).pipe(
      map((movies) => {
        if (!movies.length) return null;
        const highlight = movies.find((movie) => movie.isHighlight);
        return highlight ?? movies[0];
      })
    );
  }

  /** Returns a movie by its ID */
  getMovieById(id: number): Observable<Movie | undefined> {
    return this.http
      .get<MovieApiResponse>(`${this.baseUrl}/${id}`)
      .pipe(map((movie) => this.toMovie(movie)));
  }

  /** Returns showtimes for a movie filtered by city */
  getShowtimesForMovieInCity(movieId: number, city: string | null): Showtime[] {
    const cityKey = this.normalizeCity(city);
    return this.showtimes.filter((s) => {
      if (s.movieId !== movieId) return false;
      if (!cityKey) return true;
      return this.normalizeCity(s.city) === cityKey;
    });
  }

  /** Returns a single showtime by ID */
  getShowtimeById(id: number): Showtime | undefined {
    return this.showtimes.find((s) => s.id === id);
  }

  /** Adds a new movie and assigns it a unique ID */
  addMovie(movieData: CreateMovieRequest): Observable<Movie> {
    const payload: MovieApiRequest = {
      title: movieData.title,
      description: movieData.description || '',
      durationMinutes: movieData.durationMinutes,
      genre: movieData.genres.join(', '),
      rating: movieData.rating,
      language: movieData.language,
      posterUrl: movieData.posterUrl,
      trailerUrl: movieData.trailerUrl || '',
      showtimes: '',
      isHighlight: movieData.isHighlight ?? false,
      isNowShowing: movieData.isNowShowing ?? true,
      releaseDate: movieData.releaseDate,
    };

    return this.http
      .post<MovieApiResponse>(this.baseUrl, payload, {
        headers: this.auth.authHeaders(true),
      })
      .pipe(map((movie) => this.toMovie(movie)));
  }

  private toMovie(movie: MovieApiResponse): Movie {
    return {
      id: movie.id,
      title: movie.title,
      description: movie.description ?? '',
      durationMinutes: movie.durationMinutes,
      genres: this.splitCsv(movie.genre),
      rating: movie.rating ?? '',
      language: movie.language ?? '',
      posterUrl: movie.posterUrl ?? '',
      trailerUrl: movie.trailerUrl ?? '',
      releaseDate: movie.releaseDate,
      showtimes: this.splitCsv(movie.showtimes),
      isHighlight: movie.isHighlight,
      isNowShowing: movie.isNowShowing,
    };
  }

  private splitCsv(value: string | null | undefined): string[] {
    if (!value) return [];
    return value
      .split(',')
      .map((item) => item.trim())
      .filter((item) => item);
  }

  private normalizeCity(value: string | null | undefined): string {
    if (!value) return '';
    const ascii = value
      .replace(/[øœ]/gi, 'oe')
      .replace(/å/gi, 'aa')
      .replace(/æ/gi, 'ae')
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '');

    const cleaned = ascii
      .replace(/[^a-zA-Z0-9 ]/g, ' ')
      .toLowerCase()
      .replace(/\s+/g, ' ')
      .trim();

    const compact = cleaned.replace(/\s+/g, '');
    if (
      compact.includes('storkoebenhavn')
      || compact.includes('storkobenhavn')
      || compact.includes('storkbenhaven')
      || compact.includes('storkbenhavn')
    ) {
      return 'stor koebenhavn';
    }
    if (
      compact.includes('koebenhavn')
      || compact.includes('kobenhavn')
      || compact.includes('kbenhaven')
      || compact.includes('kbenhavn')
      || compact.includes('copenhagen')
    ) {
      return 'koebenhavn';
    }
    if (compact.includes('aarhus')) return 'aarhus';
    if (compact.includes('aalborg')) return 'aalborg';
    if (compact.includes('fyn')) return 'fyn';
    if (compact.includes('nykobingfalster')) return 'nykobing falster';
    return cleaned;
  }
}
