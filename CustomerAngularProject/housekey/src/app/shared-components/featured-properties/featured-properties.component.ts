import { Component, Input } from '@angular/core';
import { Property } from '@models/app.models';
import { PropertiesCarouselComponent } from '@shared-components/properties-carousel/properties-carousel.component';

@Component({
  selector: 'app-featured-properties',
  standalone: true,
  imports: [
    PropertiesCarouselComponent
  ],
  templateUrl: './featured-properties.component.html'
})
export class FeaturedPropertiesComponent {
  @Input('properties') featuredProperties: Property[]; 
}
