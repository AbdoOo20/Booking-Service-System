import { Component, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';
import { FlexLayoutModule } from '@ngbracket/ngx-layout';

@Component({
  selector: 'app-get-in-touch',
  standalone: true,
  imports: [
    RouterModule,
    FlexLayoutModule,
    MatIconModule,
    MatButtonModule
  ],
  templateUrl: './get-in-touch.component.html',
  styleUrls: ['./get-in-touch.component.scss']
})
export class GetInTouchComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
