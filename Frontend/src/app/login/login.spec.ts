import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Login } from './login';

// describe bruges til at gruppere tests for Login-komponenten
describe('Login', () => {

  // Reference til selve komponent-instansen
  let component: Login;

  // Fixture bruges til at håndtere komponentens DOM og lifecycle
  let fixture: ComponentFixture<Login>;

  // beforeEach kører før hver test
  // Bruges til at opsætte test-miljøet
  beforeEach(async () => {

    // Konfigurerer Angular test-modulet
    // Login er en standalone component, så den importeres direkte
    await TestBed.configureTestingModule({
      imports: [Login]
    })
    .compileComponents();

    // Opretter en instans af komponenten til test
    fixture = TestBed.createComponent(Login);

    // Henter selve komponent-klassen
    component = fixture.componentInstance;

    // Trigger Angular change detection
    fixture.detectChanges();
  });

  // En simpel test der tjekker om komponenten bliver oprettet korrekt
  it('should create', () => {
    expect(component).toBeTruthy();
  });
});