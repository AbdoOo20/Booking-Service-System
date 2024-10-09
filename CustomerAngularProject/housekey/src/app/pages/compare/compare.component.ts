import { Component, OnInit, ViewChild } from '@angular/core';
import { SwiperConfigInterface, SwiperDirective, SwiperModule } from '../../theme/components/swiper/swiper.module';
import { filter, map, Subscription } from 'rxjs';
import { Settings, SettingsService } from '@services/settings.service';
import { AppService } from '@services/app.service';
import { FlexLayoutModule, MediaChange, MediaObserver } from '@ngbracket/ngx-layout';
import { Property } from '@models/app.models';
import { MatChipsModule } from '@angular/material/chips';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-compare',
  standalone: true,
  imports: [
    RouterModule,
    FlexLayoutModule,
    MatChipsModule,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    SwiperModule,
    CurrencyPipe,
    DatePipe
  ],
  templateUrl: './compare.component.html',
  styleUrl: './compare.component.scss'
})
export class CompareComponent implements OnInit {
  @ViewChild(SwiperDirective) directiveRef: SwiperDirective;
  public config: SwiperConfigInterface = {};
  public watcher: Subscription;
  public settings: Settings;
  
  constructor(public settingsService: SettingsService, public appService: AppService, public mediaObserver: MediaObserver) {
    this.settings = this.settingsService.settings;
  }

  ngOnInit() {
    this.config = {
      observer: true,
      slidesPerView: 1,
      spaceBetween: 16,
      keyboard: false,
      navigation: false,
      pagination: false,
      simulateTouch: false,
      grabCursor: true,
      loop: false,
      preloadImages: true,
      lazy: false,
      breakpoints: {
        600: {
          slidesPerView: 2
        },
        960: {
          slidesPerView: 3,
        },
        1280: {
          slidesPerView: 4,
        }
      }
    }
    this.watchForChanges();
  }

  ngOnDestroy() {
    this.watcher.unsubscribe();
  }

  public disableSwiper() {
    setTimeout(() => {
      if (this.directiveRef) {
        this.config.keyboard = false;
        this.config.navigation = false;
        this.config.simulateTouch = false;
        this.directiveRef.update();
      }
    });
  }
  public enableSwiper() {
    setTimeout(() => {
      if (this.directiveRef) {
        this.config.keyboard = true;
        this.config.navigation = { nextEl: '.carousel-next', prevEl: '.carousel-prev' };
        this.config.simulateTouch = true;
        this.directiveRef.update();
      }
    });
  }

  public clear() {
    this.appService.Data.compareList.length = 0;
  }

  public remove(property: Property) {
    const index: number = this.appService.Data.compareList.indexOf(property);
    if (index !== -1) {
      this.appService.Data.compareList.splice(index, 1);
    }
    this.watchForChanges();
  }

  public watchForChanges() {
    this.watcher = this.mediaObserver.asObservable()
      .pipe(filter((changes: MediaChange[]) => changes.length > 0), map((changes: MediaChange[]) => changes[0]))
      .subscribe((change: MediaChange) => {
        if (change.mqAlias == 'xs' && this.appService.Data.compareList.length > 1) {
          this.enableSwiper();
        }
        else if (change.mqAlias == 'sm' && this.appService.Data.compareList.length > 2) {
          this.enableSwiper();
        }
        else if (change.mqAlias == 'md' && this.appService.Data.compareList.length > 3) {
          this.enableSwiper();
        }
        else if (change.mqAlias == 'lg' && this.appService.Data.compareList.length > 4) {
          this.enableSwiper();
        }
        else if (change.mqAlias == 'xl' && this.appService.Data.compareList.length > 4) {
          this.enableSwiper();
        }
        else {
          this.disableSwiper();
        }
      });
  }

}