import { Component, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { Settings, SettingsService } from '@services/settings.service';

@Component({
  selector: 'app-currency',
  standalone: true,
  imports: [
    MatButtonModule,
    MatIconModule,
    MatMenuModule
  ],
  templateUrl: './currency.component.html'
})
export class CurrencyComponent implements OnInit {
  public currencies = ['USD', 'EUR'];
  public currency: string;
  public settings: Settings;
  constructor(public settingsService: SettingsService) {
    this.settings = this.settingsService.settings;
  }

  ngOnInit() {
    this.currency = this.settings.currency;
  }

  public changeCurrency(currency: string) {
    this.currency = currency;
    this.settings.currency = currency;
  }

}
