import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-properties-search-results-filters',
  standalone: true,
  imports: [
    MatChipsModule,
    MatIconModule
  ],
  templateUrl: './properties-search-results-filters.component.html'
})
export class PropertiesSearchResultsFiltersComponent implements OnInit {
  @Input() searchFields: any;
  @Output() onRemoveSearchField: EventEmitter<any> = new EventEmitter<any>();
  constructor() { }

  ngOnInit() { }

  public remove(field){
    this.onRemoveSearchField.emit(field);
  }

}