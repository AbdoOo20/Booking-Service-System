import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenavModule } from '@angular/material/sidenav';
import { NavigationEnd, Router, RouterModule } from '@angular/router';
import { FlexLayoutModule } from '@ngbracket/ngx-layout';
import { DomHandlerService } from '@services/dom-handler.service';
import { Settings, SettingsService } from '@services/settings.service';

@Component({
  selector: 'app-account',
  standalone: true,
  imports: [ 
    MatSidenavModule,
    RouterModule,
    FlexLayoutModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './account.component.html' 
})
export class AccountComponent implements OnInit {
  @ViewChild('sidenav') sidenav: any;
  public sidenavOpen: boolean = true;
  public links = [
    { name: 'Profile', href: 'profile', icon: 'person' },
    { name: 'My Properties', href: 'my-properties', icon: 'view_list' },
    { name: 'Favorites', href: 'favorites', icon: 'favorite' },
    { name: 'Submit Property', href: '/submit-property', icon: 'add_circle' },
    { name: 'Logout', href: '/login', icon: 'power_settings_new' },
  ];
  public settings: Settings;
  
  constructor(public router: Router, private domHandlerService: DomHandlerService, public settingsService: SettingsService) { 
    this.settings = this.settingsService.settings;
  }

  ngOnInit() {
    if (this.domHandlerService.window?.innerWidth < 960) {
      this.sidenavOpen = false;
    };
  }

  @HostListener('window:resize')
  public onWindowResize(): void {
    (this.domHandlerService.window?.innerWidth < 960) ? this.sidenavOpen = false : this.sidenavOpen = true;
  }

  ngAfterViewInit() {
    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        if (this.domHandlerService.window?.innerWidth < 960) {
          this.sidenav.close();
        }
      }
    });
  }

}
