import { Component, OnInit } from '@angular/core';
import { MatBottomSheetRef } from '@angular/material/bottom-sheet';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { RouterModule } from '@angular/router';
import { Property } from '@models/app.models';
import { FlexLayoutModule } from '@ngbracket/ngx-layout';
import { AppService } from '@services/app.service';
import { Settings, SettingsService } from '@services/settings.service';
import { CurrencyPipe } from '@angular/common';

@Component({
  selector: 'app-compare-overview',
  standalone: true,
  imports: [
    RouterModule,
    MatToolbarModule,
    FlexLayoutModule,
    MatButtonModule,
    MatIconModule,
    MatListModule,
    CurrencyPipe
  ],
  templateUrl: './compare-overview.component.html' 
})
export class CompareOverviewComponent implements OnInit {
  public properties: Property[];
  public settings: Settings;
  constructor(public appService: AppService,
              public settingsService: SettingsService,
              private bottomSheetRef: MatBottomSheetRef<CompareOverviewComponent>) {
    this.settings = this.settingsService.settings;
  }

  ngOnInit() {
    this.properties = this.appService.Data.compareList;
  }

  public hideSheet(isRedirect: boolean) {
    this.bottomSheetRef.dismiss(isRedirect);
  }

  public remove(property, event: any) {
    const index: number = this.appService.Data.compareList.indexOf(property);
    if (index !== -1) {
      this.appService.Data.compareList.splice(index, 1);
    }
    if (this.appService.Data.compareList.length == 0) {
      this.hideSheet(false);
    }
    event.preventDefault();
  }

} 