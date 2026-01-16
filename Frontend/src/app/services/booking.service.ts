import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ApiShowtime {
  id: number;
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

export interface BookingSeatPayload {
  row: number;
  number: number;
  price: number;
}

export interface BookingRequest {
  userId: number;
  showtimeId: number;
  seats: BookingSeatPayload[];
}

export interface TicketResponse {
  id: number;
  bookingId: number;
  code: string;
  seatLabel: string;
  issuedAt: string;
}

export interface BookingResponse {
  id: number;
  userId: number;
  showtimeId: number;
  totalPrice: number;
  status: string;
  createdAt: string;
  seats: BookingSeatPayload[];
  tickets: TicketResponse[];
}

export interface BookedSeat {
  row: number;
  number: number;
  price: number;
}

@Injectable({ providedIn: 'root' })
export class BookingService {
  private http = inject(HttpClient);
  private baseUrl = 'http://localhost:5104/api';

  getShowtime(showtimeId: number): Observable<ApiShowtime> {
    return this.http.get<ApiShowtime>(`${this.baseUrl}/showtimes/${showtimeId}`);
  }

  getScreen(screenId: number): Observable<ScreenResponse> {
    return this.http.get<ScreenResponse>(`${this.baseUrl}/screens/${screenId}`);
  }

  getBookedSeats(showtimeId: number): Observable<BookedSeat[]> {
    return this.http.get<BookedSeat[]>(`${this.baseUrl}/bookings/showtime/${showtimeId}/seats`);
  }

  createBooking(request: BookingRequest): Observable<BookingResponse> {
    return this.http.post<BookingResponse>(`${this.baseUrl}/bookings`, request);
  }

  getUserBookings(userId: number): Observable<BookingResponse[]> {
    return this.http.get<BookingResponse[]>(`${this.baseUrl}/bookings/user/${userId}`);
  }

  getUserTickets(userId: number): Observable<TicketResponse[]> {
    return this.http.get<TicketResponse[]>(`${this.baseUrl}/tickets/user/${userId}`);
  }
}
