export interface Movie {
  id: number;
  title: string;
  description?: string;
  durationMinutes: number;
  genres: string[];
  rating: string;          // e.g. "11", "PG-13", etc.
  language: string;        // "Original", "Danish dub", etc.
  posterUrl: string;
  trailerUrl?: string;
  releaseDate?: string | null;
  showtimes: string[];     // ["16:00", "18:30", ...]
  isHighlight?: boolean;   // optional: mark one as hero movie
  isNowShowing?: boolean;
}
