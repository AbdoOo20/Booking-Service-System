import { Component, OnInit, Input } from '@angular/core';
import { Property } from '@models/app.models';
import { AppService } from '@services/app.service';
import { PropertyItemComponent } from '@shared-components/property-item/property-item.component';

@Component({
  selector: 'app-hot-offer-today',
  standalone: true,
  imports: [
    PropertyItemComponent
  ],
  templateUrl: './hot-offer-today.component.html' 
})
export class HotOfferTodayComponent implements OnInit {
  @Input('propertyId') propertyId: number;
  public property: Property;
  constructor(public appService: AppService) { }

  ngOnInit() {
    this.appService.getPropertyById(this.propertyId).subscribe(property => {
      this.property = property;
    })
  }

}
