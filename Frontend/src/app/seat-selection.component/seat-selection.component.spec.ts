import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { ActivatedRoute, convertToParamMap, Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { of } from 'rxjs';
import { SeatSelectionComponent } from './seat-selection.component';
import { MovieService } from '../services/movie.service';
import { BookingService } from '../services/booking.service';
import { AuthService } from '../services/auth.service';

describe('SeatSelectionComponent', () => {
  let component: SeatSelectionComponent;
  let fixture: ComponentFixture<SeatSelectionComponent>;
  let router: Router;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SeatSelectionComponent, RouterTestingModule],
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

    router = TestBed.inject(Router);
    spyOn(router, 'navigate').and.returnValue(Promise.resolve(true));

    fixture = TestBed.createComponent(SeatSelectionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('redirects to tickets after a successful booking', fakeAsync(() => {
    component.selectedSeats = new Set(['1-1']);

    component.purchase();
    tick(1600);

    expect(router.navigate).toHaveBeenCalledWith(['/tickets']);
  }));
});
