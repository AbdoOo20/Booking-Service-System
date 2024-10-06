import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { ActivatedRoute } from '@angular/router';
import { Pagination, Property } from '@models/app.models';
import { FlexLayoutModule, MediaChange, MediaObserver } from '@ngbracket/ngx-layout';
import { AppService } from '@services/app.service';
import { DomHandlerService } from '@services/dom-handler.service';
import { Settings, SettingsService } from '@services/settings.service';
import { debounceTime, distinctUntilChanged, filter, map, Subscription } from 'rxjs';
import { emailValidator } from '../../../theme/utils/app-validators';
import { MatCardModule } from '@angular/material/card';
import { RatingComponent } from '@shared-components/rating/rating.component';
import { MatIconModule } from '@angular/material/icon';
import { MatBadgeModule } from '@angular/material/badge';
import { MatInputModule } from '@angular/material/input';
import { MatSidenavModule } from '@angular/material/sidenav';
import { NgScrollbarModule } from 'ngx-scrollbar';
import { PropertiesSearchComponent } from '@shared-components/properties-search/properties-search.component';
import { PropertiesToolbarComponent } from '@shared-components/properties-toolbar/properties-toolbar.component';
import { MatChipsModule } from '@angular/material/chips';
import { NgClass } from '@angular/common';
import { PropertiesSearchResultsFiltersComponent } from '@shared-components/properties-search-results-filters/properties-search-results-filters.component';
import { PropertyItemComponent } from '@shared-components/property-item/property-item.component';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { GetInTouchComponent } from '@shared-components/get-in-touch/get-in-touch.component';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-agent',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    FlexLayoutModule,
    MatInputModule,
    MatSidenavModule,
    MatCardModule,
    MatIconModule,
    MatChipsModule,
    MatBadgeModule,
    MatProgressSpinnerModule,
    MatButtonModule,
    MatPaginatorModule,
    NgScrollbarModule,
    RatingComponent,
    PropertiesSearchComponent,
    PropertiesToolbarComponent,
    PropertiesSearchResultsFiltersComponent,
    PropertyItemComponent,
    GetInTouchComponent,
    NgClass
  ],
  templateUrl: './agent.component.html',
  styleUrl: './agent.component.scss'
})
export class AgentComponent implements OnInit {
  private sub: any;
  public agent: any;
  public agentId: any;
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
  public contactForm: FormGroup;

  constructor(public settingsService: SettingsService,
              public appService: AppService,
              private activatedRoute: ActivatedRoute,
              public mediaObserver: MediaObserver,
              public fb: FormBuilder,
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
    this.sub = this.activatedRoute.params.subscribe(params => {
      this.agentId = params['id'];
      this.getAgentById(params['id']);
      this.getProperties();
    });
    this.contactForm = this.fb.group({
      name: ['', Validators.required],
      email: ['', Validators.compose([Validators.required, emailValidator])],
      phone: ['', Validators.required],
      message: ['', Validators.required]
    });
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
    this.watcher.unsubscribe();
  }

  public getAgentById(id) {
    this.agent = this.appService.getAgents().filter(agent => agent.id == id)[0];
  }


  public getProperties() {
    this.appService.getPropertiesByAgentId(this.agentId).subscribe((data: any) => {
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

  public onContactFormSubmit(values: Object) {
    if (this.contactForm.valid) {
      console.log(values);
    }
  }

}