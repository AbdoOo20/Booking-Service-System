import { Component, OnInit, Input } from '@angular/core'; 
import { Property } from '@models/app.models';
import { SwiperConfigInterface, SwiperModule } from '../../theme/components/swiper/swiper.module';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { PropertyItemComponent } from '@shared-components/property-item/property-item.component';

@Component({
  selector: 'app-properties-carousel',
  standalone: true,
  imports: [
    SwiperModule,
    MatButtonModule,
    MatIconModule,
    PropertyItemComponent
  ],
  templateUrl: './properties-carousel.component.html' 
})
export class PropertiesCarouselComponent implements OnInit {
  @Input('properties') properties: Array<Property> = [];
  public config: SwiperConfigInterface = {}; 

  constructor() { }

  ngOnInit() {
  }

  ngAfterViewInit(){
    this.config = {
      observer: true,
      slidesPerView: 4,
      spaceBetween: 16,       
      keyboard: true,
      navigation: { nextEl: '.prop-next', prevEl: '.prop-prev'},
      pagination: true,
      grabCursor: true,        
      loop: false,
      preloadImages: true,
      lazy: false,    
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