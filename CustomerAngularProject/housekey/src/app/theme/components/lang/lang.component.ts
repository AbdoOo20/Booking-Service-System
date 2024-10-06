import { Component, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { FlexLayoutModule } from '@ngbracket/ngx-layout';
import { TranslateService } from '@ngx-translate/core';
import { Settings, SettingsService } from '@services/settings.service';

@Component({
  selector: 'app-lang',
  standalone: true,
  imports: [
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    FlexLayoutModule
  ],
  templateUrl: './lang.component.html'
})
export class LangComponent implements OnInit {
  public langName = '';
  public settings: Settings;
  constructor(public translateService: TranslateService, public settingsService: SettingsService) { 
    this.settings = this.settingsService.settings;
  }

  ngOnInit() {
    this.langName = this.getLangName(this.translateService.getDefaultLang());
  }

  public changeLang(lang: string) {
    this.translateService.use(lang);
    this.langName = this.getLangName(lang);
  }

  public getLangName(lang: string) {
    if (lang == 'en') {
      return 'English';
    }
    else if (lang == 'de') {
      return 'German';
    }
    else if (lang == 'fr') {
      return 'French';
    }
    else if (lang == 'ru') {
      return 'Russian';
    }
    else if (lang == 'tr') {
      return 'Turkish';
    }
    else {
      return 'English';
    }
  }

}
