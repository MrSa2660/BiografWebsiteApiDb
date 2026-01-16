export interface Movie {
  id: number;
  title: string;
  durationMinutes: number;
  genres: string[];
  rating: string;          // e.g. "11", "PG-13", etc.
  language: string;        // "Original", "Danish dub", etc.
  posterUrl: string;
  showtimes: string[];     // ["16:00", "18:30", ...]
  cities: string[];        // which cities this movie is available in
  isHighlight?: boolean;   // optional: mark one as hero movie
}