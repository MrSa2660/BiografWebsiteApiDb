import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

export interface CreateShowtimeRequest {
  movieId: number;
  screenId: number;
  startTime: string;
  basePrice: number;
  is3D: boolean;
  language: string | null;
}

export interface ScreenResponse {
  id: number;
  name: string;
  rows: number;
  seatsPerRow: number;
}

export interface ShowtimeResponse {
  id: number;
  movieId: number;
  screenId: number;
  startTime: string;
  basePrice: number;
  is3D: boolean;
  language: string | null;
}

@Injectable({ providedIn: 'root' })
export class ShowtimeService {
  private http = inject(HttpClient);
  private auth = inject(AuthService);
  private baseUrl = 'http://localhost:5104/api';

  createShowtime(payload: CreateShowtimeRequest): Observable<ShowtimeResponse> {
    return this.http.post<ShowtimeResponse>(`${this.baseUrl}/showtimes`, payload, {
      headers: this.auth.authHeaders(true),
    });
  }

  getScreens(): Observable<ScreenResponse[]> {
    return this.http.get<ScreenResponse[]>(`${this.baseUrl}/screens`);
  }

  getAllShowtimes(): Observable<ShowtimeResponse[]> {
    return this.http.get<ShowtimeResponse[]>(`${this.baseUrl}/showtimes`);
  }

  getShowtime(id: number): Observable<ShowtimeResponse> {
    return this.http.get<ShowtimeResponse>(`${this.baseUrl}/showtimes/${id}`);
  }
}
