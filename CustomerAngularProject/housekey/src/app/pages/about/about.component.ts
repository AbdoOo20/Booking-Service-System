import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { FlexLayoutModule } from '@ngbracket/ngx-layout';
import { ClientsComponent } from '@shared-components/clients/clients.component';
import { GetInTouchComponent } from '@shared-components/get-in-touch/get-in-touch.component';
import { HeaderImageComponent } from '@shared-components/header-image/header-image.component';
import { MissionComponent } from '@shared-components/mission/mission.component';
import { OurAgentsComponent } from '@shared-components/our-agents/our-agents.component';
import { OurServicesComponent } from '@shared-components/our-services/our-services.component'; 
import { TestimonialsComponent } from '@shared-components/testimonials/testimonials.component';

@Component({
  selector: 'app-about',
  standalone: true,
  imports: [
    FlexLayoutModule,
    HeaderImageComponent,
    MatCardModule,
    MatIconModule,
    MissionComponent,
    OurServicesComponent,
    TestimonialsComponent,
    OurAgentsComponent,
    ClientsComponent,
    GetInTouchComponent
  ],
  templateUrl: './about.component.html',
  styleUrl: './about.component.scss'
})
export class AboutComponent {

}
