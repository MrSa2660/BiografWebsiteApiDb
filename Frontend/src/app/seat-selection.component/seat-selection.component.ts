// src/app/seat-selection/seat-selection.component.ts
import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MovieService } from '../services/movie.service';
import { Showtime } from '../models/showtime.model';
import {
  ApiShowtime,
  BookingService,
  ScreenResponse,
} from '../services/booking.service';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-seat-selection',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './seat-selection.component.html',
  styleUrls: ['./seat-selection.component.css'],
})
export class SeatSelectionComponent implements OnInit {
  showtimeId = 0;
  uiShowtime: Showtime | undefined;
  apiShowtime: ApiShowtime | undefined;
  screen: ScreenResponse | undefined;
  reservedSeats = new Set<string>();
  selectedSeats = new Set<string>();
  isLoading = false;
  isSubmitting = false;
  error = '';
  successMessage = '';

  constructor(
    private route: ActivatedRoute,
    private movieService: MovieService,
    private bookingService: BookingService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      this.resetMessages();
      this.showtimeId = Number(params.get('showtimeId'));
      if (!this.showtimeId) {
        this.error = 'Missing showtime id.';
        return;
      }
      this.loadShowtime();
    });
  }

  get rows(): number[] {
    const count = this.screen?.rows ?? 8;
    return Array.from({ length: count }, (_, idx) => idx + 1);
  }

  get seatsPerRow(): number[] {
    const count = this.screen?.seatsPerRow ?? 10;
    return Array.from({ length: count }, (_, idx) => idx + 1);
  }

  get totalPrice(): number {
    const basePrice = this.apiShowtime?.basePrice ?? 95;
    return this.selectedSeats.size * basePrice;
  }

  isSeatTaken(row: number, seat: number): boolean {
    return this.reservedSeats.has(this.seatKey(row, seat));
  }

  isSeatSelected(row: number, seat: number): boolean {
    return this.selectedSeats.has(this.seatKey(row, seat));
  }

  toggleSeat(row: number, seat: number) {
    if (this.isSeatTaken(row, seat) || this.isSubmitting) return;

    const key = this.seatKey(row, seat);
    if (this.selectedSeats.has(key)) {
      this.selectedSeats.delete(key);
    } else {
      this.selectedSeats.add(key);
    }
  }

  seatLabel(row: number, seat: number): string {
    const letter = String.fromCharCode(64 + row);
    return `${letter}${seat}`;
  }

  purchase() {
    this.resetMessages();
    if (!this.apiShowtime) {
      this.error = 'Showtime could not be loaded.';
      return;
    }
    const user = this.authService.getCurrentUser();
    if (!user) {
      this.error = 'Please log in before booking seats.';
      return;
    }
    if (!this.selectedSeats.size) {
      this.error = 'Choose at least one seat.';
      return;
    }

    const price = this.apiShowtime.basePrice || 95;
    const seats = Array.from(this.selectedSeats).map((key) => {
      const [row, number] = key.split('-').map((v) => Number(v));
      return { row, number, price };
    });

    this.isSubmitting = true;
    this.bookingService
      .createBooking({
        userId: user.id,
        showtimeId: this.apiShowtime.id,
        seats,
      })
      .subscribe({
        next: (booking) => {
          this.isSubmitting = false;
          this.successMessage = `Booking confirmed. ${booking.tickets.length} ticket(s) saved.`;
          seats.forEach((s) => this.reservedSeats.add(this.seatKey(s.row, s.number)));
          this.selectedSeats.clear();
        },
        error: () => {
          this.isSubmitting = false;
          this.error = 'Seats were just taken or booking failed. Please pick different seats and try again.';
        },
      });
  }

  private loadShowtime() {
    this.isLoading = true;
    this.bookingService.getShowtime(this.showtimeId).subscribe({
      next: (showtime) => {
        this.apiShowtime = showtime;
        this.isLoading = false;
        this.loadScreen(showtime.screenId);
        this.loadReservedSeats();
      },
      error: () => {
        this.isLoading = false;
        this.error = 'Could not load showtime details from the server.';
      },
    });
  }

  private loadScreen(screenId: number) {
    this.bookingService.getScreen(screenId).subscribe({
      next: (screen) => (this.screen = screen),
      error: () => {
        this.screen = { id: 0, name: 'Standard', rows: 8, seatsPerRow: 10 };
      },
    });
  }

  private loadReservedSeats() {
    this.bookingService.getBookedSeats(this.showtimeId).subscribe({
      next: (seats) => {
        this.reservedSeats = new Set(seats.map((s) => this.seatKey(s.row, s.number)));
      },
      error: () => {
        this.reservedSeats = new Set();
      },
    });
  }

  private seatKey(row: number, seat: number): string {
    return `${row}-${seat}`;
  }

  private resetMessages() {
    this.error = '';
    this.successMessage = '';
  }
}
