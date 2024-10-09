import { NgClass } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSidenavModule } from '@angular/material/sidenav';
import { Pagination, Property } from '@models/app.models';
import { FlexLayoutModule, MediaChange, MediaObserver } from '@ngbracket/ngx-layout';
import { AppService } from '@services/app.service';
import { DomHandlerService } from '@services/dom-handler.service';
import { Settings, SettingsService } from '@services/settings.service';
import { ClientsComponent } from '@shared-components/clients/clients.component';
import { GetInTouchComponent } from '@shared-components/get-in-touch/get-in-touch.component';
import { HeaderImageComponent } from '@shared-components/header-image/header-image.component';
import { PropertiesSearchResultsFiltersComponent } from '@shared-components/properties-search-results-filters/properties-search-results-filters.component';
import { PropertiesSearchComponent } from '@shared-components/properties-search/properties-search.component';
import { PropertiesToolbarComponent } from '@shared-components/properties-toolbar/properties-toolbar.component';
import { PropertyItemComponent } from '@shared-components/property-item/property-item.component';
import { NgScrollbarModule } from 'ngx-scrollbar';
import { debounceTime, distinctUntilChanged, filter, map, Subscription } from 'rxjs';

@Component({
  selector: 'app-properties',
  standalone: true,
  imports: [
    HeaderImageComponent,
    MatSidenavModule,
    NgScrollbarModule,
    MatCardModule,
    MatIconModule,
    PropertiesSearchComponent,
    PropertiesToolbarComponent,
    PropertiesSearchResultsFiltersComponent,
    PropertyItemComponent,
    MatProgressSpinnerModule,
    MatPaginatorModule,
    MatChipsModule,
    NgClass,
    FlexLayoutModule,
    ClientsComponent,
    GetInTouchComponent,
    FlexLayoutModule,
    MatButtonModule
  ],
  templateUrl: './properties.component.html'
})
export class PropertiesComponent implements OnInit {
  @ViewChild('sidenav') sidenav: any;
  public sidenavOpen: boolean = true;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  public properties: Property[];
  public viewType: string = 'grid';
  public viewCol: number = 33.3;
  public count: number = 12;
  public sort: string;
  public searchFields: any;
  public removedSearchField: string | null;
  public pagination: Pagination = new Pagination(1, this.count, null, 2, 0, 0);
  public message: string | null;
  public watcher: Subscription;
  public settings: Settings
  
  constructor(public settingsService: SettingsService,
              public appService: AppService,
              public mediaObserver: MediaObserver,
              private domHandlerService: DomHandlerService) {
    this.settings = this.settingsService.settings;
    this.watcher = mediaObserver.asObservable()
      .pipe(filter((changes: MediaChange[]) => changes.length > 0), map((changes: MediaChange[]) => changes[0]))
      .subscribe((change: MediaChange) => {
        if (change.mqAlias == 'xs') {
          this.sidenavOpen = false;
          this.viewCol = 100;
        }
        else if (change.mqAlias == 'sm') {
          this.sidenavOpen = false;
          this.viewCol = 50;
        }
        else if (change.mqAlias == 'md') {
          this.viewCol = 50;
          this.sidenavOpen = true;
        }
        else {
          this.viewCol = 33.3;
          this.sidenavOpen = true;
        }
      });

  }

  ngOnInit() {
    this.getProperties();
  }

  ngOnDestroy() {
    this.watcher.unsubscribe();
  }

  public getProperties() {
    this.appService.getProperties().subscribe(data => {
      let result = this.filterData(data);
      if (result.data.length == 0) {
        this.properties.length = 0;
        this.pagination = new Pagination(1, this.count, null, 2, 0, 0);
        this.message = 'No Results Found';
      } else {
        this.properties = result.data;
        this.pagination = result.pagination;
        this.message = null;
      }

    })
  }

  public resetPagination() {
    if (this.paginator) {
      this.paginator.pageIndex = 0;
    }
    this.pagination = new Pagination(1, this.count, null, null, this.pagination.total, this.pagination.totalPages);
  }

  public filterData(data) {
    return this.appService.filterData(data, this.searchFields, this.sort, this.pagination.page, this.pagination.perPage);
  }

  public searchClicked() {
    this.properties.length = 0;
    this.getProperties();
    this.domHandlerService.winScroll(0, 0);
  }

  public searchChanged(event) {
    event.valueChanges.subscribe(() => {
      this.resetPagination();
      this.searchFields = event.value;
      setTimeout(() => {
        this.removedSearchField = null;
      });
      if (!this.settings.searchOnBtnClick) {
        this.properties.length = 0;
      }
    });
    event.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe(() => {
      if (!this.settings.searchOnBtnClick) {
        this.getProperties();
      }
    });
  }

  public removeSearchField(field) {
    this.message = null;
    this.removedSearchField = field;
  }

  public changeCount(count) {
    this.count = count;
    this.properties.length = 0;
    this.resetPagination();
    this.getProperties();
  }

  public changeSorting(sort) {
    this.sort = sort;
    this.properties.length = 0;
    this.getProperties();
  }

  public changeViewType(obj) {
    this.viewType = obj.viewType;
    this.viewCol = obj.viewCol;
  }

  public onPageChange(e) {
    this.pagination.page = e.pageIndex + 1;
    this.getProperties();
    this.domHandlerService.winScroll(0, 0);
  }

}