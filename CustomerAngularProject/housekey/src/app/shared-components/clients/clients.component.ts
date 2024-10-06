import { Component, OnInit } from '@angular/core';
import { SwiperConfigInterface, SwiperModule } from '../../theme/components/swiper/swiper.module';
import { AppService } from '@services/app.service';
import { Client } from '../../common/interfaces/client';

@Component({
  selector: 'app-clients',
  standalone: true,
  imports: [
    SwiperModule
  ],
  templateUrl: './clients.component.html',
  styleUrls: ['./clients.component.scss']
})
export class ClientsComponent implements OnInit {
  public clients: Client[];
  public config: SwiperConfigInterface = {};
  constructor(public appService: AppService) { }

  ngOnInit() {
    this.clients = this.appService.getClients();
  }

  ngAfterViewInit() {
    this.config = {
      observer: true,
      slidesPerView: 7,
      spaceBetween: 16,
      keyboard: true,
      navigation: false,
      pagination: false,
      grabCursor: true,
      loop: false,
      preloadImages: false,
      lazy: true,
      autoplay: {
        delay: 6000,
        disableOnInteraction: false
      },
      speed: 500,
      effect: "slide",
      breakpoints: {
        320: {
          slidesPerView: 2
        },
        480: {
          slidesPerView: 3
        },
        600: {
          slidesPerView: 4
        },
        960: {
          slidesPerView: 5
        },
        1280: {
          slidesPerView: 6
        },
        1500: {
          slidesPerView: 7
        }
      }
    }
  }

}