import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CinemaSelector } from './cinema-selector';

describe('CinemaSelector', () => {
  let component: CinemaSelector;
  let fixture: ComponentFixture<CinemaSelector>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CinemaSelector]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CinemaSelector);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
