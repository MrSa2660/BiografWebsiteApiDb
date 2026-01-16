import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-sign-up',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './sign-up.html',
  styleUrl: './sign-up.css',
})
export class SignUp {
  private authService = inject(AuthService);
  private router = inject(Router);
  showPassword = false;
  fullName = '';
  email = '';
  password = '';
  confirmPassword = '';
  errorMessage = '';
  isSubmitting = false;

  togglePassword() {
    this.showPassword = !this.showPassword;
  }

  submit() {
    if (!this.fullName || !this.email || !this.password) {
      this.errorMessage = 'Please fill in all required fields.';
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.errorMessage = 'Passwords do not match.';
      return;
    }

    this.errorMessage = '';
    this.isSubmitting = true;

    this.authService
      .signup({ email: this.email, fullName: this.fullName, password: this.password })
      .subscribe({
        next: () => {
          this.isSubmitting = false;
          this.router.navigate(['/login']);
        },
        error: () => {
          this.isSubmitting = false;
          this.errorMessage = 'Sign up failed. Please try again.';
        },
      });
  }
}
