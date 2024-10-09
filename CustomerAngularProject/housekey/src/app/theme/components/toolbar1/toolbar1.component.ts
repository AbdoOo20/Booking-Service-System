import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { AppService } from '@services/app.service';
import { ContactsComponent } from '../contacts/contacts.component';
import { SocialIconsComponent } from '../social-icons/social-icons.component';
import { CurrencyComponent } from '../currency/currency.component';
import { LangComponent } from '../lang/lang.component';
import { UserMenuComponent } from '../user-menu/user-menu.component';
import { LogoComponent } from '@shared-components/logo/logo.component';
import { HorizontalMenuComponent } from '../menu/horizontal-menu/horizontal-menu.component';
import { MatBadgeModule } from '@angular/material/badge';
import { TranslateModule } from '@ngx-translate/core';
import { RouterModule } from '@angular/router';
import { FlexLayoutModule } from '@ngbracket/ngx-layout';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-toolbar1',
  standalone: true,
  imports: [
    FlexLayoutModule,
    RouterModule,
    MatToolbarModule,
    MatIconModule,
    MatButtonModule,
    MatBadgeModule,
    TranslateModule,
    ContactsComponent,
    SocialIconsComponent,
    CurrencyComponent,
    LangComponent,
    UserMenuComponent,
    LogoComponent,
    HorizontalMenuComponent
  ],
  templateUrl: './toolbar1.component.html'
})
export class Toolbar1Component implements OnInit {
  @Output() onMenuIconClick: EventEmitter<any> = new EventEmitter<any>();
  constructor(public appService: AppService) { }

  ngOnInit() { }

  public sidenavToggle() {
    this.onMenuIconClick.emit();
  }
}