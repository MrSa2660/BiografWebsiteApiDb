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
  private authService = inject(AuthService);
  private router = inject(Router);
  showPassword = false;
  email = '';
  password = '';
  errorMessage = '';
  isSubmitting = false;

  togglePassword() {
    this.showPassword = !this.showPassword;
  }

  submit() {
    if (!this.email || !this.password) {
      this.errorMessage = 'Please enter email and password.';
      return;
    }

    this.errorMessage = '';
    this.isSubmitting = true;

    this.authService.login({ email: this.email, password: this.password }).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.router.navigate(['/cinema-selector']);
      },
      error: () => {
        this.isSubmitting = false;
        this.errorMessage = 'Login failed. Please check your credentials.';
      }
    });
  }
}
