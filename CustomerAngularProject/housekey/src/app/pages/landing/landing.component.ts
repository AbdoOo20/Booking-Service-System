import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { Router } from '@angular/router';
import { FlexLayoutModule } from '@ngbracket/ngx-layout';
import { LogoComponent } from '@shared-components/logo/logo.component';
import { OurServicesComponent } from "../../shared-components/our-services/our-services.component";
import { MissionComponent } from "../../shared-components/mission/mission.component";
import { Settings, SettingsService } from '@services/settings.service';
import { DomHandlerService } from '@services/dom-handler.service';
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
    LogoComponent,
    OurServicesComponent,
    MissionComponent
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

  // Navigate to the customer's login page (internal)
  navigateToCustomerLogin() {
    this.router.navigate(['/login']);
  }

  // Navigate to the customer's registration page (internal)
  navigateToCustomerRegister() {
    this.router.navigate(['/register']);
  }

  // Navigate to the provider's login page (external)
  navigateToProviderLogin() {
    window.location.href = 'http://localhost:30480/Identity/Account/Login'; // Ensure the link is valid
  }

  // Navigate to the provider's registration page (external)
  navigateToProviderRegister() {
    window.location.href = 'http://localhost:30480/ProviderRegister/Create'; // Ensure the link is valid
  }
  
}