import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { Router } from '@angular/router';
import { FlexLayoutModule } from '@ngbracket/ngx-layout';
import { DomHandlerService } from '@services/dom-handler.service';
import { Settings, SettingsService } from '@services/settings.service';
import { LogoComponent } from '@shared-components/logo/logo.component';

@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [
    FlexLayoutModule,
    MatSidenavModule,
    MatCardModule,
    MatIconModule,
    MatToolbarModule,
    MatButtonModule,
    LogoComponent
  ],
  templateUrl: './landing.component.html',
  styleUrl: './landing.component.scss'
})
export class LandingComponent {
  public settings: Settings;
  constructor(public settingsService: SettingsService, public router: Router, private domHandlerService: DomHandlerService) {
    this.settings = this.settingsService.settings;
  }

  ngAfterViewInit() {
    this.domHandlerService.hidePreloader();
  }

  public getDemo(number: number) {
    if (number == 1) {
      this.settings.toolbar = 1;
      this.settings.header = 'default';
      this.settings.theme = 'blue';
      this.settings.rtl = false;
    }
    if (number == 2) {
      this.settings.toolbar = 1;
      this.settings.header = 'image';
      this.settings.theme = 'blue';
      this.settings.rtl = false;
    }
    if (number == 3) {
      this.settings.toolbar = 1;
      this.settings.header = 'carousel';
      this.settings.theme = 'blue';
      this.settings.rtl = false;
    }
    if (number == 4) {
      this.settings.toolbar = 1;
      this.settings.header = 'video';
      this.settings.theme = 'blue';
      this.settings.rtl = false;
    }
    if (number == 5) {
      this.settings.toolbar = 1;
      this.settings.header = 'map';
      this.settings.theme = 'blue';
      this.settings.rtl = false;
    }
    if (number == 6) {
      this.settings.toolbar = 1;
      this.settings.header = 'map';
      this.settings.theme = 'orange-dark';
      this.settings.rtl = false;
    }
    if (number == 7) {
      this.settings.toolbar = 2;
      this.settings.header = 'image';
      this.settings.theme = 'blue';
      this.settings.rtl = false;
    }
    if (number == 8) {
      this.settings.toolbar = 2;
      this.settings.header = 'image';
      this.settings.theme = 'orange-dark';
      this.settings.rtl = false;
    }
    if (number == 9) {
      this.settings.toolbar = 1;
      this.settings.header = 'image';
      this.settings.theme = 'blue';
      this.settings.rtl = true;
    }
    this.router.navigate(['/']);
  }

  public getSkin(num: number) {
    if (num == 1) {
      this.settings.theme = 'blue';
      this.settings.header = "carousel";
    }
    if (num == 2) {
      this.settings.theme = 'green';
      this.settings.header = "carousel";
    }
    if (num == 3) {
      this.settings.theme = 'red';
      this.settings.header = "carousel";
    }
    if (num == 4) {
      this.settings.theme = 'pink';
      this.settings.header = "carousel";
    }
    if (num == 5) {
      this.settings.theme = 'purple';
      this.settings.header = "carousel";
    }
    if (num == 6) {
      this.settings.theme = 'grey';
      this.settings.header = "carousel";
    }
    this.settings.toolbar = 1;
    this.settings.rtl = false;
    this.router.navigate(['/']);
  }

  public scrollToDemos() {
    var elmnt = this.domHandlerService.winDocument.getElementById("demos");
    elmnt?.scrollIntoView({ behavior: "smooth" });
  }

  public goToTop() {
    var elmnt = this.domHandlerService.winDocument.getElementById("top");
    elmnt?.scrollIntoView({ behavior: "smooth" });
  }

}
