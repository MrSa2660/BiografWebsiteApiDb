import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, convertToParamMap } from '@angular/router';
import { of } from 'rxjs';
import { SeatSelectionComponent } from './seat-selection.component';
import { MovieService } from '../services/movie.service';
import { BookingService } from '../services/booking.service';
import { AuthService } from '../services/auth.service';

describe('SeatSelectionComponent', () => {
  let component: SeatSelectionComponent;
  let fixture: ComponentFixture<SeatSelectionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SeatSelectionComponent],
      providers: [
        { provide: ActivatedRoute, useValue: { paramMap: of(convertToParamMap({ showtimeId: '1' })) } },
        { provide: MovieService, useValue: { getShowtimeById: () => undefined } },
        {
          provide: BookingService,
          useValue: {
            getShowtime: () =>
              of({
                id: 1,
                movieId: 1,
                screenId: 1,
                startTime: new Date().toISOString(),
                basePrice: 100,
                is3D: false,
                language: null,
              }),
            getScreen: () => of({ id: 1, name: 'Test', rows: 1, seatsPerRow: 1 }),
            getBookedSeats: () => of([]),
            createBooking: () =>
              of({
                id: 1,
                userId: 1,
                showtimeId: 1,
                totalPrice: 100,
                status: 'Confirmed',
                createdAt: new Date().toISOString(),
                seats: [],
                tickets: [],
              }),
          },
        },
        {
          provide: AuthService,
          useValue: { getCurrentUser: () => ({ id: 1, email: 'a', fullName: 'Test', role: 'User', createdAt: '' }) },
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(SeatSelectionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
