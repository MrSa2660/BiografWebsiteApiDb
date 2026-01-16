import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ScreenResponse, ShowtimeService } from '../services/showtime.service';

@Component({
  selector: 'app-add-screen',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './add-screen.html',
  styleUrl: './add-screen.css',
})
export class AddScreen implements OnInit {
  screens: ScreenResponse[] = [];

  name = '';
  city = 'Koebenhavn';
  rows = 10;
  seatsPerRow = 12;
  cityOptions = ['Koebenhavn', 'Stor Koebenhavn', 'Aarhus', 'Aalborg'];

  message = '';
  error = '';
  isSubmitting = false;
  isLoading = false;

  constructor(private showtimeService: ShowtimeService) {}

  ngOnInit(): void {
    this.loadScreens();
  }

  submit() {
    this.error = '';
    this.message = '';

    if (!this.name || !this.city || !this.rows || !this.seatsPerRow) {
      this.error = 'Please fill name, city, rows, and seats per row.';
      return;
    }

    this.isSubmitting = true;
    this.showtimeService
      .createScreen({
        name: this.name,
        city: this.city,
        rows: this.rows,
        seatsPerRow: this.seatsPerRow,
      })
      .subscribe({
        next: () => {
          this.isSubmitting = false;
          this.message = 'Screen created.';
          this.resetForm();
          this.loadScreens();
        },
        error: () => {
          this.isSubmitting = false;
          this.error = 'Could not create screen.';
        },
      });
  }

  loadScreens() {
    this.isLoading = true;
    this.showtimeService.getScreens().subscribe({
      next: (screens) => {
        this.screens = screens;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.error = 'Could not load screens.';
      },
    });
  }

  private resetForm() {
    this.name = '';
    this.city = this.cityOptions[0];
    this.rows = 10;
    this.seatsPerRow = 12;
  }
}
