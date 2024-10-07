import { Component } from '@angular/core';
import { MatTabsModule } from '@angular/material/tabs';
import { MonthlyPricingComponent } from './monthly-pricing/monthly-pricing.component';
import { YearlyPricingComponent } from './yearly-pricing/yearly-pricing.component';
import { FlexLayoutModule } from '@ngbracket/ngx-layout';

@Component({
  selector: 'app-pricing',
  standalone: true,
  imports: [
    MatTabsModule,
    FlexLayoutModule,
    MonthlyPricingComponent,
    YearlyPricingComponent
  ],
  templateUrl: './pricing.component.html' 
})
export class PricingComponent {

}
