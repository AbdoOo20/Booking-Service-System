import { NgClass } from '@angular/common';
import { Component, Input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { FlexLayoutModule } from '@ngbracket/ngx-layout';

@Component({
  selector: 'app-contacts',
  standalone: true,
  imports: [
    NgClass,
    FlexLayoutModule,
    MatIconModule
  ],
  templateUrl: './contacts.component.html'
})
export class ContactsComponent {
  @Input() dividers: boolean = true;
  @Input() iconSize: string = 'sm';
  @Input() iconColor: string = ''; 
}
