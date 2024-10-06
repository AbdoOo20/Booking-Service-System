import { Component, OnInit } from '@angular/core';
import { SwiperConfigInterface, SwiperModule } from '../../theme/components/swiper/swiper.module';
import { AppService } from '@services/app.service';
import { Agent } from '../../common/interfaces/agent';
import { FlexLayoutModule } from '@ngbracket/ngx-layout';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MatButtonModule } from '@angular/material/button';
import { RouterModule } from '@angular/router';
import { RatingComponent } from '@shared-components/rating/rating.component';

@Component({
  selector: 'app-our-agents',
  standalone: true,
  imports: [
    RouterModule,
    FlexLayoutModule,
    SwiperModule,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatDividerModule,
    RatingComponent
  ],
  templateUrl: './our-agents.component.html'
})
export class OurAgentsComponent implements OnInit {
  public agents: Agent[];
  public config: SwiperConfigInterface = {};
  constructor(public appService: AppService) { }

  ngOnInit() {
    this.agents = this.appService.getAgents();
  }

  ngAfterViewInit() {
    this.config = {
      observer: true,
      slidesPerView: 4,
      spaceBetween: 16,
      keyboard: true,
      navigation: true,
      pagination: false,
      grabCursor: true,
      loop: false,
      preloadImages: false,
      lazy: true,
      breakpoints: {
        320: {
          slidesPerView: 1
        },
        600: {
          slidesPerView: 2
        },
        960: {
          slidesPerView: 3
        },
        1280: {
          slidesPerView: 4
        }
      }
    }
  }

}
