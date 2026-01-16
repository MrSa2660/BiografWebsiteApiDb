// src/app/models/showtime.model.ts
export interface Showtime {
  id: number;
  movieId: number;
  cinemaName: string; // e.g. "Lyngby Kinopalæet"
  city: string; // "Storkøbenhavn", etc.
  hall: string; // "Bio 12"
  date: string; // ISO date: "2025-11-27"
  time: string; // "16.00"
  format: string; // "2D, Eng. tale"
  availability: 'HIGH' | 'MEDIUM' | 'LOW' | 'SOLD_OUT';
}
