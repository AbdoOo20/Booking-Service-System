import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { emailValidator } from '../../theme/utils/app-validators';
import { HeaderImageComponent } from '@shared-components/header-image/header-image.component';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { GoogleMapsModule } from '@angular/google-maps';
import { FlexLayoutModule } from '@ngbracket/ngx-layout';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-contact',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatInputModule,
    GoogleMapsModule,
    HeaderImageComponent,
    MatCardModule,
    MatIconModule,
    FlexLayoutModule,
    MatButtonModule
  ],
  templateUrl: './contact.component.html'
})
export class ContactComponent implements OnInit {
  contactForm: FormGroup;
  center: google.maps.LatLngLiteral = { lat: 40.678178, lng: -73.944158 };
  zoom: number = 12;
  markerOptions: google.maps.MarkerOptions = { draggable: false };
  markerPositions: google.maps.LatLngLiteral[] = [
    { lat: 40.678178, lng: -73.944158 }
  ];
  mapOptions: google.maps.MapOptions = {
    fullscreenControl: true,
    mapTypeControl: true
  }

  constructor(public formBuilder: FormBuilder) { }

  ngOnInit() {
    this.contactForm = this.formBuilder.group({
      name: ['', Validators.required],
      email: ['', Validators.compose([Validators.required, emailValidator])],
      phone: ['', Validators.required],
      message: ['', Validators.required]
    });
  }

  public onContactFormSubmit(): void {
    if (this.contactForm.valid) {
      console.log(this.contactForm.value);
    }
  }

}
