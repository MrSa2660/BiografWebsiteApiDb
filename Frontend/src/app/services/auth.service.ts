import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { map, Observable, tap } from 'rxjs';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  user: UserResponse;
  token: string;
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
    return this.http.post<LoginResponse>(`${this.baseUrl}/login`, request).pipe(
      tap((resp) => this.setSession(resp)),
      map((resp) => resp.user)
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
      const parsed = JSON.parse(stored) as { user: UserResponse };
      return parsed.user ?? null;
    } catch {
      return null;
    }
  }

  getToken(): string | null {
    const stored = localStorage.getItem(this.storageKey);
    if (!stored) return null;
    try {
      const parsed = JSON.parse(stored) as { token: string };
      return parsed.token ?? null;
    } catch {
      return null;
    }
  }

  authHeaders(requireAdmin = false): HttpHeaders {
    const token = this.getToken();
    const user = this.getCurrentUser();
    if (!token || !user) return new HttpHeaders();
    if (requireAdmin && user.role !== 'Admin') return new HttpHeaders();

    return new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
  }

  private setSession(resp: LoginResponse) {
    localStorage.setItem(this.storageKey, JSON.stringify(resp));
  }
}
