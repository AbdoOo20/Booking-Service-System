import { Component, OnInit, Input } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Settings, SettingsService } from '@services/settings.service';

@Component({
  selector: 'app-load-more',
  standalone: true,
  imports: [
    MatButtonModule,
    MatProgressSpinnerModule,
    MatChipsModule,
    MatButtonModule
  ],
  templateUrl: './load-more.component.html'
})
export class LoadMoreComponent implements OnInit {
  @Input() step: number = 1;
  public settings: Settings;
  constructor(public settingsService: SettingsService) {
    this.settings = this.settingsService.settings;
  }

  ngOnInit() {
    this.settings.loadMore.step = this.step;
  }

  public startLoad() {
    this.settings.loadMore.start = true;
    this.settings.loadMore.load = true;
  }

}
