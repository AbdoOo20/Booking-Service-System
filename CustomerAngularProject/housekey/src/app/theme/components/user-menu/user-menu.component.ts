import { Component, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatBadgeModule } from '@angular/material/badge';
import { FlexLayoutModule } from '@ngbracket/ngx-layout';
import { TranslateModule } from '@ngx-translate/core';
import { AppService } from '@services/app.service';
import { RouterModule } from '@angular/router';
import { BidiModule } from '@angular/cdk/bidi';
import { Settings, SettingsService } from '@services/settings.service';

@Component({
  selector: 'app-user-menu',
  standalone: true,
  imports: [
    RouterModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatBadgeModule,
    FlexLayoutModule,
    TranslateModule,
    BidiModule
  ],
  templateUrl: './user-menu.component.html'
})
export class UserMenuComponent {
  public settings: Settings;
  constructor(public appService: AppService, public settingsService: SettingsService) {
    this.settings = this.settingsService.settings;
  }

}
