import { NgClass } from '@angular/common';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { FloatLabelType, MatFormFieldAppearance } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { FlexLayoutModule } from '@ngbracket/ngx-layout';
import { TranslateModule } from '@ngx-translate/core';
import { AppService } from '@services/app.service';
import { PipesModule } from '../../theme/pipes/pipes.module';

@Component({
  selector: 'app-properties-search',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    FlexLayoutModule,
    NgClass,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatCheckboxModule,
    TranslateModule,
    PipesModule
  ],
  templateUrl: './properties-search.component.html'
})
export class PropertiesSearchComponent implements OnInit {
  @Input() variant: number = 1;
  @Input() vertical: boolean = false;
  @Input() searchOnBtnClick: boolean = false;
  @Input() removedSearchField: string;
  @Output() onSearchChange: EventEmitter<any> = new EventEmitter<any>();
  @Output() onSearchClick: EventEmitter<any> = new EventEmitter<any>();
  public showMore: boolean = false;
  public form: FormGroup;
  public propertyTypes: any[] = [];
  public propertyStatuses: any[] = [];
  public cities: any[] = [];
  public neighborhoods: any[] = [];
  public streets: any[] = [];
  public features: any[] = [];

  constructor(public appService: AppService, public fb: FormBuilder) { }

  ngOnInit() {
    if (this.vertical) {
      this.showMore = true;
    };
    this.propertyTypes = this.appService.getPropertyTypes();
    this.propertyStatuses = this.appService.getPropertyStatuses();
    this.cities = this.appService.getCities();
    this.neighborhoods = this.appService.getNeighborhoods();
    this.streets = this.appService.getStreets();
    this.features = this.appService.getFeatures();
    this.form = this.fb.group({
      propertyType: null,
      propertyStatus: null,
      price: this.fb.group({
        from: null,
        to: null
      }),
      city: null,
      zipCode: null,
      neighborhood: null,
      street: null,
      bedrooms: this.fb.group({
        from: null,
        to: null
      }),
      bathrooms: this.fb.group({
        from: null,
        to: null
      }),
      garages: this.fb.group({
        from: null,
        to: null
      }),
      area: this.fb.group({
        from: null,
        to: null
      }),
      yearBuilt: this.fb.group({
        from: null,
        to: null
      }),
      features: this.buildFeatures()
    });

    this.onSearchChange.emit(this.form);
  }

  public buildFeatures() {
    const arr = this.features.map(feature => {
      return this.fb.group({
        id: feature.id,
        name: feature.name,
        selected: feature.selected
      });
    })
    return this.fb.array(arr);
  }


  ngOnChanges() {
    if (this.removedSearchField) {
      if (this.removedSearchField.indexOf(".") > -1) {
        let arr = this.removedSearchField.split(".");
        this.form.controls[arr[0]]['controls'][arr[1]].reset();
      }
      else if (this.removedSearchField.indexOf(",") > -1) {
        let arr = this.removedSearchField.split(",");
        this.form.controls[arr[0]]['controls'][arr[1]]['controls']['selected'].setValue(false);
      }
      else {
        this.form.controls[this.removedSearchField].reset();
      }
    }
  }

  public reset() {
    this.form.reset({
      propertyType: null,
      propertyStatus: null,
      price: {
        from: null,
        to: null
      },
      city: null,
      zipCode: null,
      neighborhood: null,
      street: null,
      bedrooms: {
        from: null,
        to: null
      },
      bathrooms: {
        from: null,
        to: null
      },
      garages: {
        from: null,
        to: null
      },
      area: {
        from: null,
        to: null
      },
      yearBuilt: {
        from: null,
        to: null
      },
      features: this.features
    });
  }

  public search() {
    this.onSearchClick.emit();
  }

  public onSelectCity() {
    this.form.controls['neighborhood'].setValue(null, { emitEvent: false });
    this.form.controls['street'].setValue(null, { emitEvent: false });
  }
  public onSelectNeighborhood() {
    this.form.controls['street'].setValue(null, { emitEvent: false });
  }

  public getAppearance(): MatFormFieldAppearance {
    return (this.variant != 3) ? 'outline' : 'fill';
  }
  public getFloatLabel(): FloatLabelType {
    return (this.variant == 1) ? 'always' : 'auto';
  }


}
