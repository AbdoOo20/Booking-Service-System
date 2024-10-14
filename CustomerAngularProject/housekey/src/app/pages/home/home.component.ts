import { Component, Input, OnInit, Output } from "@angular/core";
import { filter, map } from "rxjs/operators";
import { Subscription } from "rxjs";
import {
    FlexLayoutModule,
    MediaChange,
    MediaObserver,
} from "@ngbracket/ngx-layout";
import { debounceTime, distinctUntilChanged } from "rxjs/operators";
import { Pagination, Property, Location } from "@models/app.models";
import { Settings, SettingsService } from "@services/settings.service";
import { AppService } from "@services/app.service";
import { HeaderImageComponent } from "@shared-components/header-image/header-image.component";
import { HeaderCarouselComponent } from "@shared-components/header-carousel/header-carousel.component";
import { HeaderMapComponent } from "@shared-components/header-map/header-map.component";
import { HeaderVideoComponent } from "@shared-components/header-video/header-video.component";
import { MatCardModule } from "@angular/material/card";
import { MatChipsModule } from "@angular/material/chips";
import { PropertiesSearchComponent } from "@shared-components/properties-search/properties-search.component";
import { NgClass } from "@angular/common";
import { PropertiesSearchResultsFiltersComponent } from "@shared-components/properties-search-results-filters/properties-search-results-filters.component";
import { PropertiesToolbarComponent } from "@shared-components/properties-toolbar/properties-toolbar.component";
import { LoadMoreComponent } from "@shared-components/load-more/load-more.component";
import { PropertyItemComponent } from "@shared-components/property-item/property-item.component";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { MissionComponent } from "@shared-components/mission/mission.component";
import { OurServicesComponent } from "@shared-components/our-services/our-services.component";
import { TestimonialsComponent } from "@shared-components/testimonials/testimonials.component";
import { HotOfferTodayComponent } from "@shared-components/hot-offer-today/hot-offer-today.component";
import { FeaturedPropertiesComponent } from "@shared-components/featured-properties/featured-properties.component";
import { OurAgentsComponent } from "@shared-components/our-agents/our-agents.component";
import { ClientsComponent } from "@shared-components/clients/clients.component";
import { GetInTouchComponent } from "@shared-components/get-in-touch/get-in-touch.component";
import { ServicesService } from "@services/services.service";
import { Service } from "../../common/interfaces/service";
import { Category } from "../../common/interfaces/category";
import { CategoriesService } from "@services/categories.service";
import { MatSidenavModule } from "@angular/material/sidenav";
import { RecommendationBookingComponent } from "../../shared-components/recommendation-booking/recommendation-booking.component";

@Component({
    selector: "app-home",
    standalone: true,
    imports: [
        HeaderImageComponent,
        HeaderCarouselComponent,
        HeaderMapComponent,
        HeaderVideoComponent,
        MatCardModule,
        MatChipsModule,
        PropertiesSearchComponent,
        PropertiesSearchResultsFiltersComponent,
        PropertiesToolbarComponent,
        LoadMoreComponent,
        PropertyItemComponent,
        NgClass,
        FlexLayoutModule,
        MatProgressSpinnerModule,
        MissionComponent,
        OurServicesComponent,
        TestimonialsComponent,
        HotOfferTodayComponent,
        FeaturedPropertiesComponent,
        OurAgentsComponent,
        ClientsComponent,
        GetInTouchComponent,
        RecommendationBookingComponent,
        MatSidenavModule,
    ],
    providers: [CategoriesService, ServicesService], //,
    templateUrl: "./home.component.html",
    styleUrl: "./home.component.scss",
})
export class HomeComponent implements OnInit {
    watcher: Subscription;
    activeMediaQuery = "";
    catChangeName = "";
    locChangeName = "";
    public slides: any[] = [];
    public properties: Property[];
    public viewType: string = "grid";
    public viewCol: number = 25;
    public count: number = 8;
    public sort: string;
    public searchFields: any;
    public removedSearchField: string | null;
    public pagination: Pagination = new Pagination(1, 8, null, 2, 0, 0);
    public message: string | null;
    public featuredProperties: Property[];
    public locations: Location[];
    public settings: Settings;
    // My Property
    public AllServices: Service[];
    public ServicesItems: Service[];
    public itemsToShow = 8;
    public step = 8;
    public SrvBookingRec: Service[];
    public CatsItems: Category[];
    public resultView: number;
    public fromPrice = 0;
    public toPrice = Number.MAX_VALUE;
    // Test
    public catTest: string;
    public locTest: string;
    public fromTest: number;
    public toTest: number;

    constructor(
        public settingsService: SettingsService,
        public appService: AppService,
        public mediaObserver: MediaObserver,
        public myServ: ServicesService,
        public CatServ: CategoriesService
    ) {
        this.settings = this.settingsService.settings;

        this.watcher = mediaObserver
            .asObservable()
            .pipe(
                filter((changes: MediaChange[]) => changes.length > 0),
                map((changes: MediaChange[]) => changes[0])
            )
            .subscribe((change: MediaChange) => {
                if (change.mqAlias == "xs") {
                    this.viewCol = 100;
                } else if (change.mqAlias == "sm") {
                    this.viewCol = 50;
                } else if (change.mqAlias == "md") {
                    this.viewCol = 33.3;
                } else {
                    this.viewCol = 25;
                }
            });
    }
    //   this.watcher = mediaObserver
    //   .asObservable()
    //   .pipe(
    //       filter((changes: MediaChange[]) => changes.length > 0),
    //       map((changes: MediaChange[]) => changes[0])
    //   )
    //   .subscribe((change: MediaChange) => {
    //       if (change.mqAlias == "xs") {
    //           this.viewCol = 100;
    //       } else if (change.mqAlias == "sm") {
    //           this.viewCol = 50;
    //       } else if (change.mqAlias == "md") {
    //           this.viewCol = 33.3;
    //       } else {
    //           this.viewCol = 25;
    //       }
    //   });

    ngOnInit() {
        //Test
        this.getSlides();
        //this.getLocations();
        this.getProperties();
        this.getFeaturedProperties();
        // My Function
        // this.getServiceCategories(this.catChangeName);
        // this.getServiceLocation(this.locChangeName);
        // this.getServicePrice(this.fromPrice, this.toPrice);
        // this.getServices();
        this.getCategories();
        this.GetRecSrvForBooking();
        this.getServ(this.catTest, this.locTest, this.fromTest, this.toTest);
    }

    ngDoCheck() {
        if (this.settings.loadMore.load) {
            this.settings.loadMore.load = false;
            this.getProperties();
        }
    }

    ngOnDestroy() {
        this.resetLoadMore();
        this.watcher.unsubscribe();
    }

    public getSlides() {
        this.appService.getHomeCarouselSlides().subscribe((res) => {
            this.slides = res;
        });
    }

    public getLocations() {
        this.appService.getLocations().subscribe((res) => {
            this.locations = res;
        });
    }

    public getProperties() {
        this.appService.getProperties().subscribe((data) => {
            if (this.properties && this.properties.length > 0) {
                this.settings.loadMore.page++;
                this.pagination.page = this.settings.loadMore.page;
            }
            let result = this.filterData(data);
            if (result.data.length == 0) {
                this.properties.length = 0;
                this.pagination = new Pagination(1, this.count, null, 2, 0, 0);
                this.message = "No Results Found";
                return false;
            }
            if (this.properties && this.properties.length > 0) {
                this.properties = this.properties.concat(result.data);
            } else {
                this.properties = result.data;
            }
            this.pagination = result.pagination;
            this.message = null;

            if (this.properties.length == this.pagination.total) {
                this.settings.loadMore.complete = true;
                this.settings.loadMore.result = this.properties.length;
            } else {
                this.settings.loadMore.complete = false;
            }

            if (this.settings.header == "map") {
                this.locations.length = 0;
                this.properties.forEach((p) => {
                    let loc = new Location(
                        p.id,
                        p.location.lat,
                        p.location.lng
                    );
                    this.locations.push(loc);
                });
                this.locations = [...this.locations];
            }
            return true;
        });
    }

    public resetLoadMore() {
        this.settings.loadMore.complete = false;
        this.settings.loadMore.start = false;
        this.settings.loadMore.page = 1;
        this.pagination = new Pagination(
            1,
            this.count,
            null,
            null,
            this.pagination.total,
            this.pagination.totalPages
        );
    }

    public filterData(data: any) {
        return this.appService.filterData(
            data,
            this.searchFields,
            this.sort,
            this.pagination.page,
            this.pagination.perPage
        );
    }

    public searchClicked() {
        this.properties.length = 0;
        this.getProperties();
    }
    public searchChanged(event: any) {
        event.valueChanges.subscribe(() => {
            this.resetLoadMore();
            this.searchFields = event.value;

            setTimeout(() => {
                this.removedSearchField = null;
            });
            if (!this.settings.searchOnBtnClick) {
                this.properties.length = 0;
            }
        });
        event.valueChanges
            .pipe(debounceTime(500), distinctUntilChanged())
            .subscribe(() => {
                if (!this.settings.searchOnBtnClick) {
                    this.getProperties();
                }
            });
    }
    public removeSearchField(field: any) {
        this.message = null;
        this.removedSearchField = field;
        if (field == "propertyType") this.catTest = null;
        else if (field == "city") this.locTest = null;
        else if (field == "price.from") this.fromTest = null;
        else if (field == "price.to") this.toTest = null;
        else {
            this.catTest = null;
            this.locTest = null;
            this.fromTest = null;
            this.toTest = null;
        }
    }

    public changeCount(count: number) {
        this.count = count;
        this.resetLoadMore();
        this.properties.length = 0;
        this.getProperties();
    }
    public changeSorting(sort: string) {
        this.sort = sort;
        this.resetLoadMore();
        this.properties.length = 0;
        this.getProperties();
    }
    public changeViewType(obj: any) {
        this.viewType = obj.viewType;
        this.viewCol = obj.viewCol;
    }

    public getFeaturedProperties() {
        this.appService.getFeaturedProperties().subscribe((properties) => {
            this.featuredProperties = properties;
        });
    }

    public ServiceCatName: any;
    // Test
    showMore(): void {
        this.itemsToShow += this.step;
        this.ServicesItems = this.AllServices.slice(0, this.itemsToShow);
    }
    public getServ(cat: string, loc: string, from: number, to: number) {
        this.myServ.GetAllFilterion(cat, loc, from, to).subscribe({
            next: (data) => {
                this.AllServices = data;
                this.ServicesItems = this.AllServices.slice(
                    0,
                    this.itemsToShow
                );
            },
            error: (error) => {
                console.log(error);
            },
        });
    }
    //[1]
    // public getServiceCategories(catName) {
    //     this.myServ.GetAllServicesByCatName(catName).subscribe({
    //         next: (data) => {
    //             this.ServicesItems = data;
    //         },
    //         error: (err) => {
    //             console.log(err);
    //         },
    //     });
    // }

    //[2]
    // public getServiceLocation(locName) {
    //     this.myServ.GetAllServicesByLocation(locName).subscribe({
    //         next: (data) => {
    //             this.ServicesItems = data;
    //         },
    //         error: (err) => {
    //             console.log(err);
    //         },
    //     });
    // }
    // [3]
    public GetRecSrvForBooking() {
        this.myServ.GetRecomenditionServicesForBooking().subscribe({
            next: (data) => {
                this.SrvBookingRec = data;
            },
            error: (err) => {
                console.log(err);
            },
        });
    }
    //[4]
    // public getServicePrice(from: number, to: number) {
    //     this.myServ.GetAllServicesPrice(from, to).subscribe({
    //         next: (data) => {
    //             this.ServicesItems = data;
    //         },
    //         error: (err) => {
    //             console.log(err);
    //         },
    //     });
    // }
    // My Function
    public getServices() {
        this.myServ.GetAllServices().subscribe({
            next: (data) => {
                this.ServicesItems = data as Service[];
            },
            error: (err) => {
                console.log(err);
            },
        });
    }

    public getCategories() {
        this.CatServ.GetAllCategories().subscribe({
            next: (data) => {
                this.CatsItems = data as Category[];
            },
            error: (err) => {
                console.log(err);
            },
        });
    }
    // End
}
