import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SignUp } from './sign-up';

// describe bruges til at samle alle tests for SignUp-komponenten
describe('SignUp', () => {

  // Reference til komponent-instansen
  let component: SignUp;

  // Fixture styrer komponentens lifecycle og DOM i test-miljøet
  let fixture: ComponentFixture<SignUp>;

  // beforeEach kører før hver test
  // Opsætter Angular test-miljøet
  beforeEach(async () => {

    // Konfigurerer test-modulet
    // SignUp er en standalone component og importeres direkte
    await TestBed.configureTestingModule({
      imports: [SignUp]
    })
    .compileComponents();

    // Opretter komponent-instansen
    fixture = TestBed.createComponent(SignUp);

    // Henter komponent-klassen
    component = fixture.componentInstance;

    // Kører Angular change detection
    fixture.detectChanges();
  });

  // Simpel test der sikrer at komponenten kan oprettes uden fejl
  it('should create', () => {
    expect(component).toBeTruthy();
  });
});