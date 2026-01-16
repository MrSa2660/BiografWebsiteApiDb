import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { BookingService, TicketResponse } from '../services/booking.service';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-ticket-history',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './ticket-history.html',
  styleUrl: './ticket-history.css',
})
export class TicketHistory implements OnInit {
  tickets: TicketResponse[] = [];
  isLoading = false;
  error = '';
  userName = '';

  constructor(private bookingService: BookingService, private authService: AuthService) {}

  ngOnInit(): void {
    const user = this.authService.getCurrentUser();
    if (!user) {
      this.error = 'Log in to see your tickets.';
      this.isLoading = false;
      return;
    }

    this.userName = user.fullName;
    this.isLoading = true;
    this.bookingService.getUserTickets(user.id).subscribe({
      next: (tickets) => {
        this.tickets = tickets;
        this.isLoading = false;
      },
      error: () => {
        this.error = 'Could not load your tickets.';
        this.isLoading = false;
      },
    });
  }

  trackById(_: number, item: TicketResponse) {
    return item.id;
  }
}
