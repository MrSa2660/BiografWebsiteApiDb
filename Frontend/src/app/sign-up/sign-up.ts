import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../services/auth.service';

// Angular component som håndterer bruger-registrering (sign up)
@Component({
  selector: 'app-sign-up',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './sign-up.html',
  styleUrl: './sign-up.css',
})
export class SignUp {

  // Dependency Injection:
  // AuthService bruges til at oprette brugere via backend
  private authService = inject(AuthService);

  // Router bruges til navigation efter succesfuld registrering
  private router = inject(Router);

  // Styrer om password vises eller skjules i UI
  showPassword = false;

  // Two-way binding til inputfelter i HTML
  fullName = '';
  email = '';
  password = '';
  confirmPassword = '';

  // Fejlbesked som vises hvis validering eller signup fejler
  errorMessage = '';

  // Loading-state til at deaktivere knappen og vise feedback
  isSubmitting = false;

  // Skifter mellem at vise/skjule password
  togglePassword() {
    this.showPassword = !this.showPassword;
  }

  // Kaldes når signup-formen bliver sendt
  submit() {

    // Simpel klient-side validering:
    // Alle påkrævede felter skal være udfyldt
    if (!this.fullName || !this.email || !this.password) {
      this.errorMessage = 'Please fill in all required fields.';
      return;
    }

    // Tjekker at password og bekræftelses-password matcher
    if (this.password !== this.confirmPassword) {
      this.errorMessage = 'Passwords do not match.';
      return;
    }

    // Nulstil fejlbesked og aktiver loading-state
    this.errorMessage = '';
    this.isSubmitting = true;

    // Kalder AuthService for at oprette en ny bruger
    // signup() laver et HTTP-kald til backend og returnerer en Observable
    this.authService
      .signup({ email: this.email, fullName: this.fullName, password: this.password })
      .subscribe({

        // Hvis signup lykkes
        next: () => {
          this.isSubmitting = false;

          // Navigerer brugeren til login-siden
          this.router.navigate(['/login']);
        },

        // Hvis signup fejler
        error: () => {
          this.isSubmitting = false;
          this.errorMessage = 'Sign up failed. Please try again.';
        },
      });
  }
}