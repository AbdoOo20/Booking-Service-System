import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { MatToolbarModule } from '@angular/material/toolbar';
import { AppService } from '@services/app.service';
import { SocialIconsComponent } from '../social-icons/social-icons.component';
import { CurrencyComponent } from '../currency/currency.component';
import { LangComponent } from '../lang/lang.component';
import { UserMenuComponent } from '../user-menu/user-menu.component';
import { LogoComponent } from '@shared-components/logo/logo.component';
import { ContactsComponent } from '../contacts/contacts.component';
import { MatIconModule } from '@angular/material/icon';
import { HorizontalMenuComponent } from '../menu/horizontal-menu/horizontal-menu.component';
import { MatBadgeModule } from '@angular/material/badge';
import { TranslateModule } from '@ngx-translate/core';
import { RouterModule } from '@angular/router';
import { FlexLayoutModule } from '@ngbracket/ngx-layout';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-toolbar2',
  standalone: true,
  imports: [
    FlexLayoutModule,
    RouterModule,
    MatToolbarModule,
    SocialIconsComponent,
    CurrencyComponent,
    LangComponent,
    UserMenuComponent,
    LogoComponent,
    ContactsComponent,
    MatIconModule,
    MatButtonModule,
    HorizontalMenuComponent,
    MatBadgeModule,
    TranslateModule
  ],
  templateUrl: './toolbar2.component.html'
})
export class Toolbar2Component implements OnInit {
  @Output() onMenuIconClick: EventEmitter<any> = new EventEmitter<any>();
  constructor(public appService: AppService) { }

  ngOnInit() { }

  public sidenavToggle() {
    this.onMenuIconClick.emit();
  }
}