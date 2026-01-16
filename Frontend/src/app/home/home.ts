// src/app/home/home.component.ts
import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { Movie } from '../models/movie.model';
import { MovieService } from '../services/movie.service';
import { ScreenResponse, ShowtimeResponse, ShowtimeService } from '../services/showtime.service';

interface HomeShowtime {
  id: number;
  movieId: number;
  screenId: number;
  startTime: string;
  timeLabel: string;
  dateLabel: string;
  price: number;
  is3D: boolean;
  language: string | null;
  screenName: string;
  movieTitle: string;
  moviePoster: string;
  movieDuration: number;
  movieRating: string;
  movieLanguage: string;
}

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
  screens: ScreenResponse[] = [];
  todayShowtimes: HomeShowtime[] = [];
  upcomingShowtimes: HomeShowtime[] = [];
  defaultPoster =
    'https://placehold.co/400x600/111827/ffffff?text=Poster+coming+soon';
  showtimesDateLabel = 'today';
  upcomingByMovie: { movie: Movie; times: string[] }[] = [];

  constructor(
    private route: ActivatedRoute,
    private movieService: MovieService,
    private showtimeService: ShowtimeService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      this.city = params.get('city');
      this.loadHomeData();
    });
  }

  private loadHomeData(): void {
    const cityKey = this.normalizeCity(this.city);
    forkJoin({
      movies: this.movieService.getAllMovies(),
      showtimes: this.showtimeService.getAllShowtimes(),
      screens: this.showtimeService.getScreens(),
    }).subscribe({
      next: ({ movies, showtimes, screens }) => {
        this.screens = screens;
        const screenCityMap = this.buildScreenCityMap(screens);
        const filteredByCity = this.filterShowtimesByCity(showtimes, cityKey, screenCityMap);
        const effectiveByCity = cityKey && !filteredByCity.length ? showtimes : filteredByCity;
        const withoutPast = this.filterOutPastShowtimes(effectiveByCity);
        const todayList = this.filterShowtimesForToday(withoutPast);
        const futureList = withoutPast.filter((s) => !todayList.includes(s));

        this.showtimesDateLabel = 'today';
        const showtimesByMovie = this.groupShowtimesByMovie(todayList);
        this.todayShowtimes = this.buildShowtimeCards(todayList, movies, screens);
        this.upcomingShowtimes = this.buildShowtimeCards(futureList, movies, screens);
        const allUpcomingGrouped = this.groupShowtimesByMovie(withoutPast);
        this.upcomingByMovie = movies
          .map((movie) => ({
            movie,
            times: allUpcomingGrouped.get(movie.id) ?? [],
          }))
          .filter((entry) => entry.times.length)
          .sort((a, b) => a.movie.title.localeCompare(b.movie.title));

        const moviesWithTimes = movies
          .map((movie) => ({
            ...movie,
            showtimes: showtimesByMovie.get(movie.id) ?? [],
          }))
          .filter((movie) => movie.showtimes.length);

        const highlight = moviesWithTimes.find((movie) => movie.isHighlight)
          ?? moviesWithTimes[0]
          ?? movies[0]
          ?? null;
        this.highlightMovie = highlight;
        this.movies = highlight
          ? moviesWithTimes.filter((movie) => movie.id !== highlight.id)
          : moviesWithTimes;
        this.cdr.markForCheck();
      },
      error: () => {
        this.highlightMovie = null;
        this.movies = [];
        this.upcomingShowtimes = [];
        this.todayShowtimes = [];
        this.upcomingByMovie = [];
        this.cdr.markForCheck();
      },
      complete: () => this.cdr.markForCheck(),
    });
  }

  private groupShowtimesByMovie(showtimes: ShowtimeResponse[]): Map<number, string[]> {
    const grouped = new Map<number, Set<string>>();
    for (const showtime of showtimes) {
      if (!grouped.has(showtime.movieId)) grouped.set(showtime.movieId, new Set<string>());
      grouped.get(showtime.movieId)?.add(this.formatShowtimeTime(showtime.startTime));
    }

    const result = new Map<number, string[]>();
    grouped.forEach((times, movieId) => {
      const sorted = Array.from(times).sort((a, b) => a.localeCompare(b));
      result.set(movieId, sorted);
    });
    return result;
  }

  private buildScreenCityMap(screens: ScreenResponse[]): Map<number, string> {
    const map = new Map<number, string>();
    for (const screen of screens) {
      const city = this.normalizeCity(screen.city) || this.getScreenCityKey(screen.name);
      map.set(screen.id, city);
    }
    return map;
  }

  private filterShowtimesByCity(
    showtimes: ShowtimeResponse[],
    cityKey: string,
    screenCityMap: Map<number, string>
  ): ShowtimeResponse[] {
    return showtimes.filter((showtime) => {
      const screenCity = screenCityMap.get(showtime.screenId) ?? '';
      if (!cityKey) return true;
      return screenCity === cityKey;
    });
  }

  private filterShowtimesForToday(showtimes: ShowtimeResponse[]): ShowtimeResponse[] {
    const today = new Date();
    const y = today.getFullYear();
    const m = today.getMonth();
    const d = today.getDate();
    return showtimes.filter((s) => {
      const dt = new Date(s.startTime);
      return (
        !Number.isNaN(dt.getTime()) &&
        dt.getFullYear() === y &&
        dt.getMonth() === m &&
        dt.getDate() === d
      );
    });
  }

  private filterOutPastShowtimes(showtimes: ShowtimeResponse[]): ShowtimeResponse[] {
    const now = new Date();
    return showtimes.filter((s) => {
      const dt = new Date(s.startTime);
      return !Number.isNaN(dt.getTime()) && dt >= now;
    });
  }

  private dateKey(date: Date): string {
    return `${date.getFullYear()}-${date.getMonth()}-${date.getDate()}`;
  }

  private buildShowtimeCards(
    showtimes: ShowtimeResponse[],
    movies: Movie[],
    screens: ScreenResponse[]
  ): HomeShowtime[] {
    const screenNameMap = new Map<number, string>(screens.map((screen) => [screen.id, screen.name]));
    const movieMap = new Map<number, Movie>(movies.map((movie) => [movie.id, movie]));

    return showtimes
      .map((showtime) => {
        const movie = movieMap.get(showtime.movieId);
        if (!movie) return null;
        return {
          id: showtime.id,
          movieId: showtime.movieId,
          screenId: showtime.screenId,
          startTime: showtime.startTime,
          timeLabel: this.formatShowtimeTime(showtime.startTime),
          dateLabel: this.formatShowtimeDate(showtime.startTime),
          price: showtime.basePrice,
          is3D: showtime.is3D,
          language: showtime.language,
          screenName: screenNameMap.get(showtime.screenId) ?? `Screen ${showtime.screenId}`,
          movieTitle: movie.title,
          moviePoster: movie.posterUrl,
          movieDuration: movie.durationMinutes,
          movieRating: movie.rating,
          movieLanguage: movie.language,
        } as HomeShowtime;
      })
      .filter((item): item is HomeShowtime => !!item)
      .sort(
        (a, b) => new Date(a.startTime).getTime() - new Date(b.startTime).getTime()
      );
  }

  private getScreenCityKey(screenName: string): string {
    // Allow variations like "Koebenhavn Sal 1" / "Koebenhavn sal1"
    const lower = screenName.toLowerCase();
    const beforeSal = lower.split(' sal')[0].trim();
    return this.normalizeCity(beforeSal || lower.trim());
  }

  private formatShowtimeDate(value: string): string {
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) return value;
    return date.toLocaleDateString('en-GB', {
      weekday: 'short',
      day: '2-digit',
      month: 'short',
    });
  }

  private formatShowtimeTime(value: string): string {
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) return value;
    return date.toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit' });
  }

  onImgError(event: Event): void {
    const target = event.target as HTMLImageElement | null;
    if (target && target.src !== this.defaultPoster) {
      target.src = this.defaultPoster;
    }
  }

  private normalizeCity(value: string | null | undefined): string {
    if (!value) return '';
    const replaced = value
      .replace(/[\u00e6\u00c6\u00e4\u00c4]/g, 'ae')
      .replace(/[\u00f8\u00d8\u00f6\u00d6]/g, 'oe')
      .replace(/[\u00e5\u00c5]/g, 'aa');

    const ascii = replaced
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
    if (compact.includes('aarhus') || compact.includes('arhus')) return 'aarhus';
    if (compact.includes('aalborg') || compact.includes('alborg')) return 'aalborg';
    if (compact.includes('fyn')) return 'fyn';
    if (
      compact.includes('nykobingfalster')
      || compact.includes('nykobingf')
      || (compact.includes('nykobing') && compact.includes('falster'))
    ) {
      return 'nykobing falster';
    }

    return cleaned;
  }
}

