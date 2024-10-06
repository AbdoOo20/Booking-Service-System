import { NgClass } from '@angular/common';
import { Component, OnInit, Input } from '@angular/core'; 
import { DomSanitizer, SafeStyle } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { FlexLayoutModule } from '@ngbracket/ngx-layout';
import { TranslateModule } from '@ngx-translate/core';
import { MatButtonModule } from '@angular/material/button';
import { Settings, SettingsService } from '@services/settings.service';

@Component({
  selector: 'app-header-image',
  standalone: true,
  imports: [
    RouterModule, 
    NgClass,
    TranslateModule,
    FlexLayoutModule,
    MatButtonModule
  ],
  templateUrl: './header-image.component.html',
  styleUrls: ['./header-image.component.scss']
})
export class HeaderImageComponent implements OnInit {
  @Input('backgroundImage') backgroundImage: string;
  @Input('bgImageAnimate') bgImageAnimate: boolean;
  @Input('contentOffsetToTop') contentOffsetToTop: boolean;
  @Input('contentMinHeight') contentMinHeight: number;
  @Input('title') title: string;
  @Input('desc') desc: string;
  @Input('isHomePage') isHomePage: boolean = false;
  @Input('fullscreen') fullscreen: boolean = false;
  public bgImage: SafeStyle;
  public settings: Settings;
  constructor(public settingsService: SettingsService, private sanitizer: DomSanitizer) {
    this.settings = this.settingsService.settings;
    setTimeout(() => {
      this.settings.headerBgImage = true;
    });
  }

  ngOnInit() {
    if (this.contentOffsetToTop) {
      setTimeout(() => {
        this.settings.contentOffsetToTop = this.contentOffsetToTop;
      });
    }
    if (this.backgroundImage) {
      this.bgImage = this.sanitizer.bypassSecurityTrustStyle('url(' + this.backgroundImage + ')');
    }
  }

  ngOnDestroy() {
    setTimeout(() => {
      this.settings.headerBgImage = false;
      this.settings.contentOffsetToTop = false;
    });
  }

}
