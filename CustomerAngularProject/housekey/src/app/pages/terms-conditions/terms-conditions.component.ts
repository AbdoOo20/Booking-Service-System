import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { FlexLayoutModule } from '@ngbracket/ngx-layout';

@Component({
  selector: 'app-terms-conditions',
  standalone: true,
  imports: [
    FlexLayoutModule,
    MatCardModule
  ],
  templateUrl: './terms-conditions.component.html' 
})
export class TermsConditionsComponent {

}
