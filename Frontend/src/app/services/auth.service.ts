import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface SignupRequest {
  email: string;
  fullName: string;
  password: string;
}

export interface UserResponse {
  id: number;
  email: string;
  fullName: string;
  role: string;
  createdAt: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private baseUrl = 'http://localhost:5104/api/users';
  private readonly storageKey = 'biograf.currentUser';

  login(request: LoginRequest): Observable<UserResponse> {
    return this.http.post<UserResponse>(`${this.baseUrl}/login`, request).pipe(
      tap((user) => this.setSession(user))
    );
  }

  signup(request: SignupRequest): Observable<UserResponse> {
    return this.http.post<UserResponse>(this.baseUrl, request);
  }

  logout(): void {
    localStorage.removeItem(this.storageKey);
  }

  getCurrentUser(): UserResponse | null {
    const stored = localStorage.getItem(this.storageKey);
    if (!stored) return null;
    try {
      return JSON.parse(stored) as UserResponse;
    } catch {
      return null;
    }
  }

  authHeaders(requireAdmin = false): HttpHeaders {
    const user = this.getCurrentUser();
    if (!user) return new HttpHeaders();

    if (requireAdmin && user.role !== 'Admin') {
      return new HttpHeaders();
    }

    return new HttpHeaders({
      'X-User-Id': user.id.toString(),
      'X-User-Role': user.role,
    });
  }

  private setSession(user: UserResponse) {
    localStorage.setItem(this.storageKey, JSON.stringify(user));
  }
}
