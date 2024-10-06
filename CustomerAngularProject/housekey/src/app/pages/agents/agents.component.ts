import { Component, OnInit } from '@angular/core';
import { Agent } from '../../common/interfaces/agent';
import { AppService } from '@services/app.service';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { FlexLayoutModule } from '@ngbracket/ngx-layout';
import { MatButtonModule } from '@angular/material/button';
import { HeaderImageComponent } from '@shared-components/header-image/header-image.component';
import { RatingComponent } from '@shared-components/rating/rating.component';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';

@Component({
  selector: 'app-agents',
  standalone: true,
  imports: [
    RouterModule,
    FlexLayoutModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatDividerModule,
    HeaderImageComponent,
    RatingComponent
  ],
  templateUrl: './agents.component.html',
  styleUrl: './agents.component.scss'
})
export class AgentsComponent implements OnInit {
  public agents: Agent[];
  constructor(public appService: AppService) { }

  ngOnInit() {
    this.agents = this.appService.getAgents();
  }

}

