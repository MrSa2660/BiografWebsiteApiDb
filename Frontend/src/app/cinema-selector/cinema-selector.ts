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
      name: 'København',
      tagline: 'IMAX, Dolby Atmos og luksussæder i hovedstaden.',
      highlight: '13 biografer · 120 sale',
    },
    {
      name: 'Stor København',
      tagline: 'Nem adgang, fri parkering og komfortable sale.',
      highlight: '6 biografer · 45 sale',
    },
    {
      name: 'Aarhus',
      tagline: 'Moderne sale midt i byen.',
      highlight: '4 biografer · 30 sale',
    },
    {
      name: 'Aalborg',
      tagline: 'Hygge og skarp projektion i nord.',
      highlight: '3 biografer · 20 sale',
    },
    {
      name: 'Fyn',
      tagline: 'Familievenlige biografer på øen.',
      highlight: '3 biografer · 18 sale',
    },
    {
      name: 'Nykøbing Falster',
      tagline: 'Lokal perle med nye sæder.',
      highlight: '1 biograf · 6 sale',
    },
  ];
}
