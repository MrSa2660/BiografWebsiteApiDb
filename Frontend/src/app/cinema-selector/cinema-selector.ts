import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';

interface CityOption {
  name: string;
  tagline: string;
  highlight: string;
}

@Component({
  selector: 'app-cinema-selector',
  imports: [CommonModule, RouterLink],
  templateUrl: './cinema-selector.html',
  styleUrl: './cinema-selector.css',
})
export class CinemaSelector {
  cities: CityOption[] = [
    {
      name: 'Koebenhavn',
      tagline: 'IMAX, Dolby Atmos og luksussaeder i hovedstaden.',
      highlight: '13 biografer a 120 sale',
    },
    {
      name: 'Stor Koebenhavn',
      tagline: 'Nem adgang, fri parkering og komfortable sale.',
      highlight: '6 biografer a 45 sale',
    },
    {
      name: 'Aarhus',
      tagline: 'Moderne sale midt i byen.',
      highlight: '4 biografer a 30 sale',
    },
    {
      name: 'Aalborg',
      tagline: 'Hygge og skarp projektion i nord.',
      highlight: '3 biografer a 20 sale',
    },
    {
      name: 'Fyn',
      tagline: 'Familievenlige biografer paa oeen.',
      highlight: '3 biografer a 18 sale',
    },
    {
      name: 'Nykobing Falster',
      tagline: 'Lokal perle med nye saeder.',
      highlight: '1 biograf a 6 sale',
    },
  ];
}
