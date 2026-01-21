import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-login',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {

  // Dependency Injection via inject():
  // AuthService bruges til login mod backend
  private authService = inject(AuthService);

  // Router bruges til navigation efter login
  private router = inject(Router);

  // Styrer om password vises som tekst eller skjult
  showPassword = false;

  // Two-way binding til inputfelter i HTML ([(ngModel)])
  email = '';
  password = '';

  // Fejlbesked som vises i UI hvis login fejler
  errorMessage = '';

  // Bruges til at deaktivere knappen og vise loading-state
  isSubmitting = false;

  // Skifter mellem at vise/skjule password
  togglePassword() {
    this.showPassword = !this.showPassword;
  }

  // Kaldes når login-formen submitter
  submit() {

    // Simpel klient-side validering
    if (!this.email || !this.password) {
      this.errorMessage = 'Please enter email and password.';
      return;
    }

    // Nulstil fejl og sæt loading-state
    this.errorMessage = '';
    this.isSubmitting = true;

    // Kalder AuthService for at logge ind mod backend
    // login() returnerer en Observable (HTTP-kald)
    this.authService.login({ email: this.email, password: this.password }).subscribe({
      
      // Hvis login lykkes
      next: () => {
        this.isSubmitting = false;

        // Navigerer brugeren videre efter succesfuldt login
        this.router.navigate(['/cinema-selector']);
      },

      // Hvis login fejler
      error: () => {
        this.isSubmitting = false;
        this.errorMessage = 'Login failed. Please check your credentials.';
      }
    });
  }
}